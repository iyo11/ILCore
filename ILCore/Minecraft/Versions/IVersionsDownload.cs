using ILCore.Download;
using ILCore.Download.DownloadData;
using Newtonsoft.Json.Linq;

namespace ILCore.Minecraft.Versions;

public interface IVersionsDownload
{
    Task<JsonVersion> GetJsonVersion(IDownloadUrl downloadUrl);
    List<VersionsItem> GetAllVersion(IDownloadUrl downloadUrl);
    VersionsItem GetLatestVersion(LatestType latestType);
    List<VersionsItem> GetVersions(VersionType versionType);
    Task<string> GetVersionJson(VersionsItem item);
    Task<bool> Install(VersionsItem versionsItem, string versionName,Downloader downloadManager,string minecraftPath = null);
}