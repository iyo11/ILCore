using System.Buffers;
using System.Collections.Immutable;
using System.Threading.Tasks.Dataflow;
using ILCore.Download.DownloadData;

namespace ILCore.Download;

public class Downloader : IDisposable
{
    private const int MaxRetryCount = 3;
    private const int BufferSize = 4096; // byte
    private const int UpdateInterval = 1000; // second
    
    private readonly ArrayPool<byte> _bufferPool;
    private readonly HttpClient _httpClient;
    private readonly AutoResetEvent _autoResetEvent;
    private readonly ExecutionDataflowBlockOptions _parallelOptions;
    
    private CancellationTokenSource _cancellationTokenSource;
    
    public event Action<DownloadResult> Completed;
    public event Action<DownloadProgress> ProgressChanged;
    public event Action<DownloadItem> DownloadItemCompleted;
    
    private ImmutableList<DownloadItem> _downloadItems;

    private int _totalBytes;
    private int _downloadedBytes;
    private int _previousDownloadedBytes;
    
    private int _totalCount;
    private int _completedCount;
    private int _failedCount;
    
    private static Timer _timer;

    public Downloader()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        
        _bufferPool = ArrayPool<byte>.Create(BufferSize, Environment.ProcessorCount * 2);
        
        _autoResetEvent = new AutoResetEvent(true);
        
        _parallelOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2,
        };

        _cancellationTokenSource = new CancellationTokenSource();
        _parallelOptions.CancellationToken = _cancellationTokenSource.Token;
    }

    private void Observe()
    {
        _timer = new Timer((_) =>
        {
            UpdateDownloadProgress();
        }, null, 0, UpdateInterval);
    }


    public void Setup(IEnumerable<DownloadItem> downloadItems)
    {
        // Initialize states
        _downloadItems = downloadItems.ToImmutableList();
        _totalBytes = _downloadItems.Sum(item => item.Size);
        _downloadedBytes = 0;
        _previousDownloadedBytes = 0;

        _totalCount = _downloadItems.Count;
        _completedCount = 0;
        _failedCount = 0;

        if (_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            _parallelOptions.CancellationToken = _cancellationTokenSource.Token;
        }

        _autoResetEvent.Reset();
    }
        
    public async ValueTask<bool> StartAsync()
    {
        while (true)
        {
            Observe();
            try
            {
                var downloader = new ActionBlock<DownloadItem>(async item =>
                {
                    for (var i = 0; i < MaxRetryCount && !_cancellationTokenSource.IsCancellationRequested; i++)
                    {
                        if (await DownloadFileAsync(item, i))
                        {
                            break;
                        }
                    }
                }, _parallelOptions);

                foreach (var item in _downloadItems)
                {
                    downloader.Post(item);
                }

                downloader.Complete();
                await downloader.Completion;
            }
            catch (OperationCanceledException)
            {
                //_logService.Info(nameof(DownloadService), "Download canceled");
            }

            await _timer.DisposeAsync();
            // Ensure the last progress report is fired
            UpdateDownloadProgress();

            // Succeeded
            if (_completedCount == _totalCount)
            {
                //下载成功
                Completed?.Invoke(DownloadResult.Succeeded);
                return true;
            }


            // Clean incomplete files
            foreach (var item in _downloadItems.Where(item => !item.IsCompleted && File.Exists(item.Path)))
            {
                File.Delete(item.Path);
            }

            if (_failedCount > 0 && !_cancellationTokenSource.IsCancellationRequested)
            {
                //下载失败
                Completed?.Invoke(DownloadResult.Incomplete);
            }

            // Wait for retry or cancel
            _autoResetEvent.WaitOne();

            // Canceled
            if (!_cancellationTokenSource.IsCancellationRequested) continue;
            //下载取消

            Completed?.Invoke(DownloadResult.Canceled);
            return false;
        }
    }
    
    

    private async Task<bool> DownloadFileAsync(DownloadItem downloadItem, int retryTimes)
    {
        try
        {
            if (retryTimes > 0)
            {
                //retry
            }
            if (Path.IsPathRooted(downloadItem.Path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(downloadItem.Path) ?? string.Empty);
            }
            
            // 创建一个缓冲区，大小为64KB
            var buffer = _bufferPool.Rent(BufferSize);
            
            // 发送GET请求，并等待响应
            var request = new HttpRequestMessage(HttpMethod.Get, downloadItem.Url);
            var response =
                await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _cancellationTokenSource.Token);
            
            // 判断请求是否成功,如果失败则返回 false
            if (!response.IsSuccessStatusCode)
            {
                return (false);
            }
            
            // 获取响应内容长度
            var contentLength = response.Content.Headers.ContentLength ?? 0;
            if (downloadItem.Size == 0)
            {
                downloadItem.Size = (int)contentLength;
                Interlocked.Add(ref _totalBytes, downloadItem.Size);
            }
            downloadItem.IsPartialContentSupported = response.Headers.AcceptRanges.Contains("bytes");
            // 获取响应流
            await using var httpStream = await response.Content.ReadAsStreamAsync(_cancellationTokenSource.Token);
            // 创建一个文件流，并将响应内容写入文件流
            await using var fileStream = File.Open($@"{downloadItem.Path}\{downloadItem.Name}", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            var timeout = TimeSpan.FromSeconds(Math.Max(downloadItem.Size / 16384.0, 30.0));
            using var timeoutCts = new CancellationTokenSource(timeout);
            using var readCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, timeoutCts.Token);
            
            int bytesRead;
            while ((bytesRead = await httpStream.ReadAsync(buffer, readCts.Token)) > 0)
            {
                // 检查是否已经取消了任务
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    // 如果任务已经取消，关闭文件流，并删除已经下载的文件
                    fileStream.Close();
                    File.Delete(downloadItem.Path);
                    break;
                }
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), readCts.Token);
                downloadItem.DownloadedBytes += bytesRead;
                Interlocked.Add(ref _downloadedBytes, bytesRead);
            }
            //判断下载是否取消
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                fileStream.Close();
                response.Dispose();
                return (false);
            }

            // Download successful
            downloadItem.IsCompleted = true;
            DownloadItemCompleted?.Invoke(downloadItem);
            Interlocked.Increment(ref _completedCount);
            
            
            fileStream.Close();
            response.Dispose();
            return (true);
        }
        catch (Exception e)
        {
            // 返回false表示文件下载失败
            //return (false);
            await _timer.DisposeAsync();
            //throw new Exception(e.Message);
            // If is not caused by cancellation, mark as failure
            if (_cancellationTokenSource.IsCancellationRequested) throw new Exception(e.Message);
        }
        
        // If is not caused by cancellation, mark as failure
        if (_cancellationTokenSource.IsCancellationRequested) return (false);
        Interlocked.Increment(ref _failedCount);
        Interlocked.Add(ref _downloadedBytes, -downloadItem.DownloadedBytes);
        downloadItem.DownloadedBytes = 0;
        Interlocked.Exchange(ref _previousDownloadedBytes, _downloadedBytes);
        return false;
    }
    
    private void UpdateDownloadProgress()
    {
        // Calculate speed
        var diffBytes = _downloadedBytes - _previousDownloadedBytes;
        _previousDownloadedBytes = _downloadedBytes;

        var progress = new DownloadProgress
        {
            TotalCount = _totalCount,
            CompletedCount = _completedCount,
            FailedCount = _failedCount,
            TotalBytes = _totalBytes,
            DownloadedBytes = _downloadedBytes,
            Speed = diffBytes / (UpdateInterval / (double)1000),
        };
        ProgressChanged?.Invoke(progress);
    }
    
    public void Retry()
    {
        //"Retrying incomplete downloads"
        _downloadItems = _downloadItems.Where(item => !item.IsCompleted).ToImmutableList();
        _failedCount = 0;
        //var remainingCount = _totalCount - _completedCount;
        //var remainingBytes = _downloadItems.Sum(item => item.Size);
        //"Remaining items count: {remainingCount}.";
        //"Remaining items size (Bytes): {remainingBytes}.";
        _autoResetEvent.Set();
    }

    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
        _timer.Dispose();
        _autoResetEvent.Set();
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing) return;
        //"Disposed"
        _timer.Dispose();
        _httpClient.Dispose();
        _cancellationTokenSource.Dispose();
        _autoResetEvent.Dispose();
    }
    
}