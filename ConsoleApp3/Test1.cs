using ILCore.Download;
using ILCore.Download.DownloadData;
using ILCore.Util;

namespace ConsoleApp3;

public class Test1
{
    public static async void DoSomething()
    {
        var downloadManager = new Downloader();

        var downloadItems = new List<DownloadItem>
        {
            new()
            {
                Name = "Test0.pdf",
                Path = "E:/DownloadTest/Test1",
                Url = "https://www.leomay.com/upload/file/mmo-20170707165001.pdf"
            },
            new()
            {
                Name = "Test1.pdf",
                Path = "E:/DownloadTest/Test1",
                Url = "https://www.leomay.com/upload/file/mmo-20170707165001.pdf"
            },
            new()
            {
                Name = "Test2.pdf",
                Path = "E:/DownloadTest/Test1",
                Url = "https://www.leomay.com/upload/file/mmo-20170707165001.pdf"
            },
            new()
            {
                Name = "Test3.pdf",
                Path = "E:/DownloadTest/Test1",
                Url = "https://www.leomay.com/upload/file/mmo-20170707165001.pdf"
            },
            new()
            {
                Name = "Test5.pdf",
                Path = "E:/DownloadTest/Test1",
                Url = "https://www.leomay.com/upload/file/mmo-20170707165001.pdf"
            }
        };

        downloadManager.DownloadItemsInfoChanged += s =>
        {
            Log.Info(
                $"Add: Count-{s.NewItemsCount} Bytes-{Converters.ConvertBitsToBestUnit(s.NewItemsBytes)} Total: Count-{s.TotalCount} Bytes-{Converters.ConvertBitsToBestUnit(s.TotalBytes)}");
        };

        downloadManager.ProgressChanged += s =>
        {
            Log.Info(
                $"Speed:{Converters.ConvertBitsToBestUnit(s.Speed)} Percent:{Converters.ConvertBitsToBestUnit(s.DownloadedBytes)}/{Converters.ConvertBitsToBestUnit(s.TotalBytes)} Complete:{s.CompletedCount}/{s.TotalCount} Failed:{s.FailedCount}");
        };

        downloadManager.Completed += s => { Console.WriteLine(s.ToString()); };

        downloadManager.DownloadItemCompleted += s => { Log.Debug($"{s.Name} Completed [Retry Time {s.RetryCount}]"); };

        downloadManager.Setup(downloadItems);

        var a = downloadManager.StartAsync().Result;
    }
}