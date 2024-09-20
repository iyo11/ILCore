using ILCore.Download.DownloadData;
using ILCore.Minecraft.Assets;
using ILCore.Minecraft.Versions;
using Newtonsoft.Json.Linq;

var url = new OfficialUrl();
var versions = new Versions();

var versionDownloadService = versions.GetDownloadService(url);


var latestVersion = versionDownloadService.GetLatestVersion(LatestType.Release);

var versionsOldAlpha = versionDownloadService.GetVersions(VersionType.old_alpha);

var versionsRelease = versionDownloadService.GetVersions(VersionType.release);

var versionsSnapshot = versionDownloadService.GetVersions(VersionType.snapshot);

//Console.WriteLine(latestVersion.Url);


/*foreach (var versionsItem in versionsSnapshot)
{
    Console.WriteLine(versionsItem.Url);
}*/

var versionJson = await versionDownloadService.GetVersionJson(versionsRelease[0]);

var versionJObject = JObject.Parse(versionJson);

var assets = new Assets();

var assetsJson = await assets.ToJsonAsset(versionJObject);

foreach (var value in assetsJson.Objects.Values.ToList()) Console.WriteLine(url.Asset + value.Path);