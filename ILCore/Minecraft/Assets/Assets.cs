using ILCore.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ILCore.Minecraft.Assets;

public class Assets
{
    public Assets()
    {
        
    }


    private async Task<string> GetAssetsJson(JObject jObject)
    {
        var url = jObject?["assetIndex"]?["url"]?.ToString();
        return await NetWorkClient.GetAsync(url);
    }

    public async Task<JsonAsset> ToJsonAsset(JObject jObject)
    {
        var json = await GetAssetsJson(jObject);
        return JsonConvert.DeserializeObject<JsonAsset>(json);
    }
    
    
    
    
}