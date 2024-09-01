using ILCore.Download.DownloadData;
using ILCore.Minecraft.Assets;
using ILCore.Minecraft.Versions;
using Newtonsoft.Json.Linq;



var api = new OfficialUrl();
var versions = new Versions(api);


var latestVersion = versions.GetLatestVersion(LatestType.Release);

var versionsOldAlpha = versions.GetVersions(VersionType.old_alpha);

var versionsRelease = versions.GetVersions(VersionType.release);

var versionsSnapshot = versions.GetVersions(VersionType.snapshot);

//Console.WriteLine(latestVersion.Url);


/*foreach (var versionsItem in versionsSnapshot)
{
    Console.WriteLine(versionsItem.Url);
}*/

var versionJson = await versions.GetVersionJson(versionsRelease[0].Url);

var versionJObject = JObject.Parse(versionJson);

var assets = new Assets();

var assetsJson = await assets.ToJsonAsset(versionJObject,api);

foreach (var value in assetsJson.Objects.Values.ToList())
{
    Console.WriteLine(api.Asset + value.Path);
}