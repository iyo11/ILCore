namespace ILCore.Download.DownloadData;

public enum DownloadResult
{
    Incomplete,
    Succeeded,
    Canceled
}

public class DownloadItemsInfo
{
    public int NewItemsCount { get; set; }
    public int NewItemsBytes { get; set; }
    public int TotalCount { get; set; }
    public int TotalBytes { get; set; }
}

public class DownloadProgress
{
    public int TotalCount { get; set; }

    public int CompletedCount { get; set; }

    public int FailedCount { get; set; }

    public int TotalBytes { get; set; }

    public int DownloadedBytes { get; set; }

    public double Speed { get; set; }
}