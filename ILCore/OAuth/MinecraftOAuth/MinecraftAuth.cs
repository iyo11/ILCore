using System.Text;
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
        var minecraftAuthorizationResponseMessage = await NetWorkClient.PostAsync(
            "https://api.minecraftservices.com/authentication/login_with_xbox",
            new StringContent(minecraftAuthPostJson, Encoding.UTF8, "application/json"));
        var minecraftAuthObject = JObject.Parse(await minecraftAuthorizationResponseMessage.Content.ReadAsStringAsync());
        return minecraftAuthObject["access_token"]?.ToString();
    }

    public async Task<JObject> GetProfileAsync(string minecraftToken)
    {
        var profileJResponseMessage = await NetWorkClient.GetAsync(
            "https://api.minecraftservices.com/minecraft/profile",
            new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {minecraftToken}"}
            }
        );
        var profileObject = JObject.Parse(await profileJResponseMessage.Content.ReadAsStringAsync());
        return profileObject;
    }
}