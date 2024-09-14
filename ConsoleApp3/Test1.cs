using ILCore.Download;
using ILCore.Download.DownloadData;
using ILCore.Minecraft.Assets;
using ILCore.Minecraft.Versions;
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
                Url = "https://www.leomay.com/upload/file/mmo-20170707165001.pdf",
            },
            new()
            {
                Name = "Test1.pdf",
                Path = "E:/DownloadTest/Test1",
                Url = "https://www.leomay.com/upload/file/mmo-20170707165001.pdf",
            },
            new()
            {
                Name = "Test2.pdf",
                Path = "E:/DownloadTest/Test1",
                Url = "https://www.leomay.com/upload/file/mmo-20170707165001.pdf",
            },
            new()
            {
                Name = "Test3.pdf",
                Path = "E:/DownloadTest/Test1",
                Url = "https://www.leomay.com/upload/file/mmo-20170707165001.pdf",
            },
            new()
            {
                Name = "Test5.pdf",
                Path = "E:/DownloadTest/Test1",
                Url = "https://www.leomay.com/upload/file/mmo-20170707165001.pdf",
            }
        };

        downloadManager.DownloadItemsInfoChanged += s =>
        {
            Log.Debug(
                $"Add: Count-{s.NewItemsCount} Bytes-{s.NewItemsBytes} Total: Count-{s.TotalCount} Bytes-{s.TotalBytes}");
        };

        downloadManager.ProgressChanged += s =>
        {
            Log.Debug(
                $"Speed:{s.Speed} Percent:{s.DownloadedBytes}/{s.TotalBytes} Complete:{s.CompletedCount}/{s.TotalCount} Failed:{s.FailedCount}");
        };

        downloadManager.Completed += s =>
        {
            Console.WriteLine(s.ToString());
        };

        downloadManager.DownloadItemCompleted += s => { Console.WriteLine($"{s.Name} Completed Retry Time {s.RetryCount}"); };


        downloadManager.Setup(downloadItems);

        var a = downloadManager.StartAsync().Result;
    }
}