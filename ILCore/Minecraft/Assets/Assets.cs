using System.Collections.Immutable;
using ILCore.Download.DownloadData;
using ILCore.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ILCore.Minecraft.Assets;

public class Assets
{
    public async Task<string> DownloadAssetsJsonAsync(JObject jObject)
    {
        var responseMessage = await NetWorkClient.GetAsync(jObject?["assetIndex"]?["url"]?.ToString());
        return await responseMessage.Content.ReadAsStringAsync();
    }

    public async Task<string> DownloadAssetsJsonAsync(string url)
    {
        var responseMessage = await NetWorkClient.GetAsync(url);
        return await responseMessage.Content.ReadAsStringAsync();
    }

    public async Task<string> ReadAssetsJsonAsync(string path)
    {
        return await File.ReadAllTextAsync(path);
    }

    public async Task<string> GetAssetsJsonAsync(string name, IDownloadUrl downloadUrlApi, string minecraftPath = null)
    {
        var path = $"{minecraftPath ?? Environment.CurrentDirectory + "/.minecraft"}/assets/indexes/{name}.json";
        if (File.Exists(path)) return await ReadAssetsJsonAsync(path);
        var url = $"{downloadUrlApi.Asset}/{name}.json";
        var responseMessage = await NetWorkClient.GetAsync(path);
        return await responseMessage.Content.ReadAsStringAsync();
    }


    public async Task<JsonAsset> ToJsonAsset(JObject jObject, string minecraftPath = null)
    {
        /*var
        var assetsIndexPath = string.IsNullOrEmpty(minecraftPath)? $"{Environment.CurrentDirectory}/assets/index/}"
        if (expr)
        {

        }*/
        var json = await DownloadAssetsJsonAsync(jObject);
        minecraftPath ??= minecraftPath ?? Environment.CurrentDirectory + "/.minecraft";
        var directory = new DirectoryInfo($"{minecraftPath}/assets/indexes");
        if (!directory.Exists)
        {
            directory.Create();
        }
        await File.WriteAllTextAsync($"{minecraftPath}/assets/indexes/{jObject["assetIndex"]["id"]}.json", json);
        return JsonConvert.DeserializeObject<JsonAsset>(json);
    }
    
    public Task CopyToVirtualAsync(JsonAsset jsonAsset, string minecraftPath = null)
    {
        minecraftPath ??= minecraftPath ?? Environment.CurrentDirectory + "/.minecraft";
        return Task.Run(() =>
            jsonAsset.Objects?
                .AsParallel()
                .ForAll(pair =>
                {
                    var objectPath = $"{minecraftPath}/assets/objects/{pair.Value.Path}/{pair.Value.Hash}";
                    var virtualPath = $"{minecraftPath}/assets/virtual/legacy/{pair.Key}";
                    var virtualDir = Path.GetDirectoryName(virtualPath);
                    if (!File.Exists(objectPath) || File.Exists(virtualPath))
                    {
                        return;
                    }
                    Directory.CreateDirectory(virtualDir);
                    File.Copy(objectPath, virtualPath);
                })
        );
    }
    
    
    public async Task<IEnumerable<AssetObject>> CheckIntegrityAsync(JsonAsset jAsset, string minecraftPath = null)
    {
        if (jAsset.Objects is null) return [];
        minecraftPath ??= Path.Combine(Environment.CurrentDirectory, ".minecraft");
        var query = jAsset.Objects
            .Select(pair => pair.Value)
            .AsParallel()
            .Where(obj =>
            {
                var objPath = Path.Combine(minecraftPath, "assets", "objects", obj.Path, obj.Hash);
                return !File.Exists(objPath) || !Crypto.ValidateFileSHA1(objPath, obj.Hash);
            })
            .ToList(); // 确保查询立即执行
        return query;
    }


    public async Task<IEnumerable<DownloadItem>> ToDownloadItems(JsonAsset jAsset, IDownloadUrl downloadUrlApi,
        string minecraftPath = null)
    {
        var unIntegrityItems = await CheckIntegrityAsync(jAsset, minecraftPath);
        return unIntegrityItems
            .AsParallel()
            .Select(obj =>
            new DownloadItem
            {
                Name = obj.Hash,
                Path = $"{minecraftPath ?? $"{Environment.CurrentDirectory}/.minecraft"}/assets/objects/{obj.Path}",
                Url = downloadUrlApi.Asset + obj.Url,
                Size = obj.Size,
                IsCompleted = false,
                DownloadedBytes = 0
            });
    }
}