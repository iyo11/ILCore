using System.Buffers;
using System.Collections.Immutable;
using System.Threading.Tasks.Dataflow;
using ILCore.Download.DownloadData;
using ILCore.Util;

namespace ILCore.Download;

public class Downloader : IDisposable
{
    private const int MaxRetryCount = 3;
    private const int BufferSize = 4096; // bytes
    private const int UpdateInterval = 1000; // milliseconds
    private readonly AutoResetEvent _autoResetEvent;

    private readonly ArrayPool<byte> _bufferPool;
    private readonly HttpClient _httpClient;
    private readonly ExecutionDataflowBlockOptions _parallelOptions;

    private CancellationTokenSource _cancellationTokenSource;

    private int _completedCount;
    private int _downloadedBytes;

    private List<DownloadItem> _downloadItems;
    private int _failedCount;
    private bool _isDisposed;
    private int _previousDownloadedBytes;
    private Timer _timer;
    private int _totalBytes;
    private int _totalCount;
    public bool IsCompleted;

    public Downloader()
    {
        _httpClient = new HttpClient
        {
            Timeout=TimeSpan.FromSeconds(10)
        };
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36");
        _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");

        _bufferPool = ArrayPool<byte>.Create(BufferSize, Environment.ProcessorCount * 2);
        _autoResetEvent = new AutoResetEvent(true);

        _parallelOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2
        };

        _cancellationTokenSource = new CancellationTokenSource();
        _parallelOptions.CancellationToken = _cancellationTokenSource.Token;

        Completed += _ => { IsCompleted = true; };
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    public event Action<DownloadResult> Completed;
    public event Action<DownloadProgress> ProgressChanged;
    public event Action<DownloadItem> DownloadItemCompleted;
    public event Action<DownloadItemsInfo> DownloadItemsInfoChanged;


    private void Observe()
    {
        _timer = new Timer(_ => UpdateDownloadProgress(), null, 0, UpdateInterval);
    }
    
    
    public Downloader Setup(DownloadItem downloadItem)
    {
        Setup([downloadItem]);
        return this;
    }


    public Downloader Setup(IEnumerable<DownloadItem> downloadItems)
    {
        ArgumentNullException.ThrowIfNull(downloadItems);

        _downloadItems = downloadItems.ToList();
        _totalBytes = _downloadItems.Sum(item => item.Size);
        _downloadedBytes = 0;
        _previousDownloadedBytes = 0;

        _totalCount = _downloadItems.Count;
        _completedCount = 0;
        _failedCount = 0;

        ResetCancellationTokenSource();
        _autoResetEvent.Reset();

        DownloadItemsInfoChanged?.Invoke(new DownloadItemsInfo
        {
            TotalBytes = _totalBytes,
            TotalCount = _totalCount,
            NewItemsBytes = _totalBytes,
            NewItemsCount = _totalCount
        });
        return this;
    }

    public Downloader Add(IEnumerable<DownloadItem> downloadItems)
    {
        ArgumentNullException.ThrowIfNull(downloadItems);

        if (_cancellationTokenSource.IsCancellationRequested) ResetCancellationTokenSource();

        var newDownloadItems = downloadItems.ToImmutableList();
        var newDownloadItemsBytes = newDownloadItems.Sum(item => item.Size);

        _downloadItems.AddRange(newDownloadItems);
        _totalCount += newDownloadItems.Count;
        _totalBytes += newDownloadItemsBytes;

        DownloadItemsInfoChanged?.Invoke(new DownloadItemsInfo
        {
            TotalBytes = _totalBytes,
            TotalCount = _totalCount,
            NewItemsBytes = newDownloadItemsBytes,
            NewItemsCount = newDownloadItems.Count
        });
        return this;
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
                    for (var i = 0; i < MaxRetryCount && !_cancellationTokenSource.Token.IsCancellationRequested; i++)
                        if (await DownloadFileAsync(item, i))
                            break;
                    if (!item.IsCompleted)
                    {
                        Interlocked.Increment(ref _failedCount);
                        Interlocked.Add(ref _downloadedBytes, -item.DownloadedBytes);
                        item.DownloadedBytes = 0;
                        Interlocked.Exchange(ref _previousDownloadedBytes, _downloadedBytes);
                    }
                }, _parallelOptions);

                foreach (var item in _downloadItems) downloader.Post(item);

                downloader.Complete();
                await downloader.Completion;
            }
            catch (OperationCanceledException e)
            {
                Log.Error(e.Message);
            }
            finally
            {
                await _timer.DisposeAsync();
                UpdateDownloadProgress();
            }

            if (_completedCount == _totalCount)
            {
                Completed?.Invoke(DownloadResult.Succeeded);
                return true;
            }

            CleanUpIncompleteFiles();

            if (_failedCount > 0 && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                Completed?.Invoke(DownloadResult.Incomplete);
                return false;
            }

            _autoResetEvent.WaitOne();

            if (!_cancellationTokenSource.Token.IsCancellationRequested) continue;
            Completed?.Invoke(DownloadResult.Canceled);
            return false;
        }
    }

    private void CleanUpIncompleteFiles()
    {
        foreach (var item in _downloadItems.Where(item => !item.IsCompleted && File.Exists(item.Path)))
            File.Delete(item.Path);
    }

    private async Task<bool> DownloadFileAsync(DownloadItem downloadItem, int retryTimes)
    {
        if (retryTimes > 0)
        {
            Log.Debug($"Retry download {downloadItem.Name} Time:{retryTimes}");
            downloadItem.RetryCount = retryTimes;
        }

        if (string.IsNullOrEmpty(downloadItem.Name) && !string.IsNullOrEmpty(downloadItem.Path))
        {
            var file = new FileInfo(downloadItem.Path);
            downloadItem.Name = file.Name;
            downloadItem.Path = file.DirectoryName;
        }

        var buffer = _bufferPool.Rent(BufferSize);
        try
        {
            if (!Directory.Exists(downloadItem.Path))
                if (downloadItem.Path != null)
                    Directory.CreateDirectory(downloadItem.Path);

            var response = await SendDownloadRequestAsync(downloadItem);

            if (!response.IsSuccessStatusCode) throw new Exception($"Response code {response.StatusCode}");

            await ProcessDownloadResponseAsync(downloadItem, response, buffer);
            return true;
        }
        catch (OperationCanceledException e)
        {
            Log.Error($"{downloadItem.Name} > {e.Message}");
        }
        catch (Exception e)
        {
            Log.Error($"{downloadItem.Name} > {e.Message}");
        }
        finally
        {
            _bufferPool.Return(buffer);
        }

        return false;
    }

    private async Task<HttpResponseMessage> SendDownloadRequestAsync(DownloadItem downloadItem)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, downloadItem.Url);
        return await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
            _cancellationTokenSource.Token);
    }

    private async Task ProcessDownloadResponseAsync(DownloadItem downloadItem, HttpResponseMessage response,
        byte[] buffer)
    {
        var contentLength = response.Content.Headers.ContentLength ?? 0;

        if (downloadItem.Size == 0)
        {
            downloadItem.Size = (int)contentLength;
            Interlocked.Add(ref _totalBytes, downloadItem.Size);
        }

        downloadItem.IsPartialContentSupported = response.Headers.AcceptRanges.Contains("bytes");

        await using var httpStream = await response.Content.ReadAsStreamAsync(_cancellationTokenSource.Token);
        await using var fileStream = File.Open($@"{downloadItem.Path}\{downloadItem.Name}", FileMode.Create,
            FileAccess.Write, FileShare.Read);

        var timeout = TimeSpan.FromSeconds(Math.Max(downloadItem.Size / 16384.0, 30.0));
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var readCts =
            CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, timeoutCts.Token);

        int bytesRead;
        while ((bytesRead = await httpStream.ReadAsync(buffer, readCts.Token)) > 0)
        {
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                fileStream.Close();
                File.Delete(downloadItem.Path);
                return;
            }

            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), readCts.Token);
            downloadItem.DownloadedBytes += bytesRead;
            Interlocked.Add(ref _downloadedBytes, bytesRead);
        }

        downloadItem.IsCompleted = true;
        DownloadItemCompleted?.Invoke(downloadItem);
        Interlocked.Increment(ref _completedCount);
    }

    private void UpdateDownloadProgress()
    {
        var diffBytes = _downloadedBytes - _previousDownloadedBytes;
        _previousDownloadedBytes = _downloadedBytes;

        var progress = new DownloadProgress
        {
            TotalCount = _totalCount,
            CompletedCount = _completedCount,
            FailedCount = _failedCount,
            TotalBytes = _totalBytes,
            DownloadedBytes = _downloadedBytes,
            Speed = diffBytes / (UpdateInterval / 1000.0) is double ? (long)(diffBytes / (UpdateInterval / 1000.0)) : 0
        };

        ProgressChanged?.Invoke(progress);
    }

    public void Retry()
    {
        _downloadItems = _downloadItems.Where(item => !item.IsCompleted).ToList();
        _failedCount = 0;
        _autoResetEvent.Set();
    }

    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
        _timer?.Dispose();
        _autoResetEvent.Set();
    }

    private void ResetCancellationTokenSource()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        _parallelOptions.CancellationToken = _cancellationTokenSource.Token;
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            _timer?.Dispose();
            _httpClient.Dispose();
            _cancellationTokenSource.Dispose();
            _autoResetEvent.Dispose();
        }

        _isDisposed = true;
    }
    
}