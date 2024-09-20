using ILCore.Download;
using ILCore.Download.DownloadData;
using ILCore.Minecraft.Versions;
using ILCore.Util;

namespace ConsoleApp3;

public class Test2
{
    public static async void VersionsTest()
    {
        var url = new OfficialUrl();
        var versions = new Versions();
        var versionDownload = versions.GetDownloadService(url);

        var versionList = versionDownload.GetAllVersion(url);

        var downloadManager = new Downloader();
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
        var a = await versionDownload.Install(versionList[553], "1.8.4", downloadManager,@"E:\MinecraftTest\.minecraft");
    }
}