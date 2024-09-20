/*
using ILCore.Download;
using ILCore.Download.DownloadData;
using ILCore.Minecraft.Assets;
using ILCore.Minecraft.Libraries;
using ILCore.Minecraft.Versions;
using ILCore.Util;

var downloadManager = new Downloader();

var url = new OfficialUrl();
var versions = new Versions();
var versionDownload = versions.GetDownloadService(url);
var versionLocal = versions.GetLocalService(@"G:\Minecraft\.minecraft");
Assets assets = new();
Libraries libraries = new();


var json = await File.ReadAllTextAsync(@"G:\Minecraft\.minecraft\versions\1.16.5\1.16.5.json");

var versionJObject = versionLocal.ToVersionJObject(json);

var jassets = await assets.ToJsonAsset(versionJObject);
var jLibraries = versionJObject["libraries"];


var downloadAssets = assets.ToDownloadItems(jassets.Objects.Values.ToList(), url, @"E:\DownloadTest");

var downloadLibs = libraries.ToDownloadItems(jLibraries, url, @"E:\DownloadTest");

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


downloadManager.Setup(downloadLibs);
downloadManager.Add(downloadAssets);

await downloadManager.StartAsync();
*/

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
var a = await versionDownload.Install(versionList[4], "test1", downloadManager,@"E:\MinecraftTest\.minecraft");