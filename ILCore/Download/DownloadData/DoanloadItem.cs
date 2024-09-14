namespace ILCore.Download.DownloadData;

public class DownloadItem
{
    public string Name { get; set; }

    public string Path { get; set; }

    public string Url { get; set; }

    public int Size { get; set; }

    public int DownloadedBytes { get; set; }

    public bool IsPartialContentSupported { get; set; }

    public bool IsCompleted { get; set; }

    public int RetryCount { get; set; } = 0;
}