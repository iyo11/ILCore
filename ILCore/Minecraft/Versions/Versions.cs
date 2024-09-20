using ILCore.Download;
using ILCore.Download.DownloadData;
using ILCore.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ILCore.Minecraft.Versions;


public class Versions
{
    private VersionsDownload _downloadService;
    private VersionsLocal _versionsLocal;

    public IVersionsDownload GetDownloadService(IDownloadUrl url)
    {
        if (_downloadService is null || url.GetType() != _downloadService._url.GetType())
        {
            _downloadService = new VersionsDownload(url);
        }
        return  _downloadService;
    }

    public IVersionsLocal GetLocalService(string minecraftPath)
    {
        if (_versionsLocal is null || _versionsLocal._minecraftPath != minecraftPath)
        {
            _versionsLocal = new VersionsLocal(minecraftPath);
        }
        return  _versionsLocal;
    }

}

public class VersionsLocal(string minecraftPath) : IVersionsLocal
{

    public readonly string _minecraftPath = minecraftPath;

    public JObject ToVersionJObject(string json) => JObject.Parse(json);
}

public class VersionsDownload : IVersionsDownload
{
    private readonly JsonVersion _jsonVersion;
    public readonly IDownloadUrl _url;

    public VersionsDownload(IDownloadUrl url)
    {
        _jsonVersion = GetJsonVersion(url).Result;
        _url = url;
    } 

    public async Task<JsonVersion> GetJsonVersion(IDownloadUrl downloadUrl)
    {
        var jsonText = await NetWorkClient.GetAsync(downloadUrl.VersionList);
        var versions = JsonConvert.DeserializeObject<JsonVersion>(await jsonText.Content.ReadAsStringAsync());
        return versions;
    }

    public List<VersionsItem> GetAllVersion(IDownloadUrl downloadUrl) => _jsonVersion.Versions;
    

    public VersionsItem GetLatestVersion(LatestType latestType) =>
        latestType == LatestType.Release
            ? _jsonVersion.Versions.FirstOrDefault(v => _jsonVersion.Latest.Release == v.Id)
            : _jsonVersion.Versions.FirstOrDefault(v => _jsonVersion.Latest.Snapshot == v.Id);


    public List<VersionsItem> GetVersions(VersionType versionType) => _jsonVersion.Versions.Where(v => v.Type == versionType.ToString()).ToList();

    public async Task<string> GetVersionJson(VersionsItem item)
    {
        var responseMessage = await NetWorkClient.GetAsync(item.Url);
        return await responseMessage.Content.ReadAsStringAsync();
    }
    
    public async Task<DownloadItem> ToDownloadItems(string json, string minecraftPath, string VersionName)
    {
        var jObject = JObject.Parse(json);
        var jarUrl = jObject["downloads"]["client"]["url"].ToString();
        return new DownloadItem()
        {
            Url = jarUrl,
            Path = Path.Combine(minecraftPath, "versions", VersionName),
            Name = $"{VersionName}.jar",
            Size = int.Parse(jObject["downloads"]["client"]["size"].ToString()),
            IsPartialContentSupported = true
        };
    }

    public async Task<bool> Install(VersionsItem versionsItem, string versionName,Downloader downloadManager, string minecraftPath = null)
    {
        var json = await GetVersionJson(versionsItem);
        
        minecraftPath ??= Environment.CurrentDirectory + "/.minecraft";

        var directory = new DirectoryInfo($"{minecraftPath}/versions/{versionName}");

        if (!directory.Exists)
        {
            directory.Create();
        }
        
        File.WriteAllTextAsync(Path.Combine(directory.FullName,$"{versionName}.json"), json);
        
        var versionJObject = JObject.Parse(json);

        var libraries = new Libraries.Libraries();
        var assets = new Assets.Assets();
        
        var jassets = await assets.ToJsonAsset(versionJObject,minecraftPath);
        var jLibraries = versionJObject["libraries"];
        
        
        var versionDownloadItem = await ToDownloadItems(json, minecraftPath, versionName);
        
        var librariesDownloadItems 
            = libraries.ToDownloadItems(jLibraries, _url, minecraftPath);
        
        var assetDownloadItems 
            = assets.ToDownloadItems(jassets.Objects.Values.ToList(), _url, minecraftPath);

        downloadManager.Setup(versionDownloadItem).Add(librariesDownloadItems).Add(assetDownloadItems);
        await downloadManager.StartAsync();

        
        await assets.CopyToVirtualAsync(jassets, minecraftPath);
        
        return true;

    }

} 
