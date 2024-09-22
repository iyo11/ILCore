using ILCore.Download;
using ILCore.Download.DownloadData;
using ILCore.Minecraft.Versions;
using ILCore.Util;

var url = new OfficialUrl();
var versions = new Versions();
var versionDownload = versions.GetDownloadService(url);

var versionList = versionDownload.GetVersions(VersionType.release);

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
await versionDownload.Install(versionList[0], "test2", downloadManager,@"E:\MinecraftTest\.minecraft");