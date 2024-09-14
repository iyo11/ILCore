using ILCore.Download;
using ILCore.Download.DownloadData;
using ILCore.Minecraft.Assets;
using ILCore.Minecraft.Versions;
using ILCore.Util;

var downloadManager = new Downloader();

Assets assets = new();

var versions = new Versions();
var json = await File.ReadAllTextAsync(@"G:\Minecraft\.minecraft\versions\1.16.5\1.16.5.json");
var versionJObject = versions.GetVersionJObject(json);
var url = new OfficialUrl();

var jassets = await assets.ToJsonAsset(versionJObject, url);

IDownloadUrl urlApi = new BmclApiUrl();


var downloads = assets.ToDownloadItems(jassets.Objects.Values.ToList(), urlApi, @"E:\DownloadTest");

downloadManager.DownloadItemsInfoChanged += s =>
{
    Log.Info(
        $"Add: Count-{s.NewItemsCount} Bytes-{s.NewItemsBytes} Total: Count-{s.TotalCount} Bytes-{s.TotalBytes}");
};

downloadManager.ProgressChanged += s =>
{
    Log.Info(
        $"Speed:{s.Speed} Percent:{s.DownloadedBytes}/{s.TotalBytes} Complete:{s.CompletedCount}/{s.TotalCount} Failed:{s.FailedCount}");
};

downloadManager.Completed += s =>
{
    Console.WriteLine(s.ToString());
};

downloadManager.DownloadItemCompleted += s => { Log.Debug($"{s.Name} Completed [Retry Time {s.RetryCount}]"); };


downloadManager.Setup(downloads);

await downloadManager.StartAsync();