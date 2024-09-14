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


    public async Task<JsonAsset> ToJsonAsset(JObject jObject, IDownloadUrl downloadUrlApi, string minecraftPath = null)
    {
        /*var
        var assetsIndexPath = string.IsNullOrEmpty(minecraftPath)? $"{Environment.CurrentDirectory}/assets/index/}"
        if (expr)
        {

        }*/
        var json = await DownloadAssetsJsonAsync(jObject);
        return JsonConvert.DeserializeObject<JsonAsset>(json);
    }

    public IEnumerable<DownloadItem> ToDownloadItems(IEnumerable<AssetObject> assetObjects, IDownloadUrl downloadUrlApi,
        string minecraftPath = null)
    {
        return assetObjects.Select(obj =>
            new DownloadItem
            {
                Name = obj.Hash,
                Path = $"{minecraftPath ?? Environment.CurrentDirectory + "/.minecraft"}/objects/{obj.Path}",
                Url = downloadUrlApi.Asset + obj.Url,
                Size = obj.Size,
                IsCompleted = false,
                DownloadedBytes = 0
            });
    }
}