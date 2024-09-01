using System.Collections.Immutable;
using ILCore.Download;
using ILCore.Download.DownloadData;
using ILCore.Minecraft.Assets;
using ILCore.Minecraft.Versions;


var downloadManager = new Downloader();

Assets assets = new();

var versions = new Versions();
var json = await File.ReadAllTextAsync(@"G:\Minecraft\.minecraft\versions\1.12.2\1.12.2.json");
var versionJObject = versions.GetVersionJObject(json);
var url = new BmclApiUrl();

var jassets = await assets.ToJsonAsset(versionJObject,url);

IDownloadUrl urlApi = new BmclApiUrl();


var downloads = assets.ToDownloadItems(jassets.Objects.Values.ToList(), urlApi, @"E:\DownloadTest");

downloadManager.DownloadItemsInfoChanged += (s) =>
{
    Console.WriteLine($"Add: Count-{s.NewItemsCount} Bytes-{s.NewItemsBytes} Total: Count-{s.TotalCount} Bytes-{s.TotalBytes}");
};

downloadManager.ProgressChanged += (s) =>
{
    Console.WriteLine($"Speed:{s.Speed} Percent:{s.DownloadedBytes}/{s.TotalBytes} Complete:{s.CompletedCount}/{s.TotalCount} Failed:{s.FailedCount}");
};

downloadManager.Completed += (s) =>
{
    Console.WriteLine($"All Completed!");
};

downloadManager.DownloadItemCompleted += (s) =>
{
    Console.WriteLine($"{s.Name} Completed!");
};


downloadManager.Setup(downloads);

await downloadManager.StartAsync();

/*await Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith((_) =>
{
    Console.WriteLine("add");
    downloadManager.Add(newItems);
});

Console.ReadLine();*/



