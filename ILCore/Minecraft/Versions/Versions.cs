using ILCore.Download.DownloadData;
using ILCore.Minecraft.Libraries;
using ILCore.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ILCore.Minecraft.Versions
{
    public class Versions
    {
        private readonly JsonVersion _jsonVersion;
        private readonly IDownloadUrl _downloadUrl;
        public Versions(IDownloadUrl downloadUrl = null)
        {
            _downloadUrl = downloadUrl ?? new OfficialUrl();
            _jsonVersion = GetJsonVersion().Result;
        }

        private async Task<JsonVersion> GetJsonVersion()
        {
            var jsonText = await NetWorkClient.GetAsync(_downloadUrl.VersionList);
            var versions = JsonConvert.DeserializeObject<JsonVersion>(jsonText);
            return versions;
        }

        public List<VersionsItem> GetVersions() => _jsonVersion.Versions;
        

        public VersionsItem GetLatestVersion(LatestType latestType)
        => latestType == LatestType.Release ? _jsonVersion.Versions.FirstOrDefault(v => _jsonVersion.Latest.Release == v.Id) : _jsonVersion.Versions.FirstOrDefault(v => _jsonVersion.Latest.Snapshot == v.Id);
        

        public List<VersionsItem> GetVersions(VersionType versionType) => _jsonVersion.Versions.Where(v => v.Type == versionType.ToString()).ToList();

        public async Task<string> GetVersionJson(string uri) => await NetWorkClient.GetAsync(uri);
        
        public JObject GetVersionJObject(string json) => JObject.Parse(json);
    }
}
