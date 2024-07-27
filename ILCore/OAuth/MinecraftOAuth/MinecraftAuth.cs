using System.Net.Http.Headers;
using ILCore.OAuth.XboxOAuth;
using ILCore.Util;
using Newtonsoft.Json.Linq;

namespace ILCore.OAuth.MinecraftOAuth;

public class MinecraftAuth
{
    public async Task<string> MinecraftAuthAsync(XtstAuthToken xtstAuthToken)
    {
        var xtstParam = $"{xtstAuthToken.Uhs};{xtstAuthToken.XtstToken}";
        var minecraftAuthPostJson = "{" + $"\"identityToken\": \"XBL3.0 x={xtstParam}\"" + "}";
        var minecraftAuthorizationJson = await NetClient.PostJsonAsync(
            "https://api.minecraftservices.com/authentication/login_with_xbox",
            minecraftAuthPostJson,
            [new MediaTypeWithQualityHeaderValue("application/json")]);
        var minecraftAuthObject = JObject.Parse(minecraftAuthorizationJson);
        return minecraftAuthObject["access_token"]?.ToString();
    }

    public async Task<JObject> GetProfileAsync(string minecraftToken)
    {
        var profileJson = await NetClient.GetAsync(
            "https://api.minecraftservices.com/minecraft/profile",
            new AuthenticationHeaderValue("Bearer",$"{minecraftToken}")
        );
        var profileObject = JObject.Parse(profileJson);
        return profileObject;
    }
}