using ILCore.Download.DownloadData;
using ILCore.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ILCore.Minecraft.Assets;

public class Assets
{
    public async Task<string> DownloadAssetsJsonAsync(JObject jObject) => await NetWorkClient.GetAsync(jObject?["assetIndex"]?["url"]?.ToString());
    public async Task<string> DownloadAssetsJsonAsync(string url) => await NetWorkClient.GetAsync(url);
    
    public async Task<string> ReadAssetsJsonAsync(string path) => await File.ReadAllTextAsync(path);

    public async Task<string> GetAssetsJsonAsync(string name,IDownloadUrl downloadUrlApi,string minecraftPath = null)
    {
        var path = $"{minecraftPath??Environment.CurrentDirectory + $"/.minecraft"}/assets/indexes/{name}.json";
        if (File.Exists(path))
        {
            return await ReadAssetsJsonAsync(path);
        }
        var url = $"{downloadUrlApi.Asset}/{name}.json";
        return await NetWorkClient.GetAsync(path);
    }
    

    public async Task<JsonAsset> ToJsonAsset(JObject jObject,IDownloadUrl downloadUrlApi,string minecraftPath = null)
    {
        /*var 
        var assetsIndexPath = string.IsNullOrEmpty(minecraftPath)? $"{Environment.CurrentDirectory}/assets/index/}"
        if (expr)
        {
            
        }*/
        var json = await DownloadAssetsJsonAsync(jObject);
        return JsonConvert.DeserializeObject<JsonAsset>(json);
    }

    public IEnumerable<DownloadItem> ToDownloadItems(IEnumerable<AssetObject> assetObjects,IDownloadUrl downloadUrlApi,string minecraftPath = null)
    {
        return assetObjects.Select(obj =>
            new DownloadItem
            {
                Name = obj.Hash,
                Path = $"{minecraftPath??Environment.CurrentDirectory + "/.minecraft"}/objects/{obj.Path}",
                Url = downloadUrlApi.Asset + obj.Url,
                Size = obj.Size,
                IsCompleted = false,
                DownloadedBytes = 0,
            });
    }
    
    
}