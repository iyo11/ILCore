using ILCore.Download.DownloadData;
using ILCore.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ILCore.Minecraft.Versions;

public class Versions
{
    private readonly IDownloadUrl _downloadUrl;
    private readonly JsonVersion _jsonVersion;

    public Versions(IDownloadUrl downloadUrl = null)
    {
        _downloadUrl = downloadUrl ?? new OfficialUrl();
        _jsonVersion = GetJsonVersion().Result;
    }

    private async Task<JsonVersion> GetJsonVersion()
    {
        var jsonText = await NetWorkClient.GetAsync(_downloadUrl.VersionList);
        var versions = JsonConvert.DeserializeObject<JsonVersion>(await jsonText.Content.ReadAsStringAsync());
        return versions;
    }

    public List<VersionsItem> GetVersions()
    {
        return _jsonVersion.Versions;
    }


    public VersionsItem GetLatestVersion(LatestType latestType)
    {
        return latestType == LatestType.Release
            ? _jsonVersion.Versions.FirstOrDefault(v => _jsonVersion.Latest.Release == v.Id)
            : _jsonVersion.Versions.FirstOrDefault(v => _jsonVersion.Latest.Snapshot == v.Id);
    }


    public List<VersionsItem> GetVersions(VersionType versionType)
    {
        return _jsonVersion.Versions.Where(v => v.Type == versionType.ToString()).ToList();
    }

    public async Task<string> GetVersionJson(string uri)
    {
        var responseMessage = await NetWorkClient.GetAsync(uri);
        return await responseMessage.Content.ReadAsStringAsync();
    }

    public JObject GetVersionJObject(string json)
    {
        return JObject.Parse(json);
    }
}