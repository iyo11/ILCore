using System.Text;
using ILCore.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ILCore.OAuth.XboxOAuth;

public class XboxAuth
{
    public async Task<string> XboxAuthAsync(string microsoftToken)
    {
        var xboxAuthPostJson = JsonConvert.SerializeObject(new XboxAuthContent
        {
            Properties = new XboxProperties
            {
                AuthMethod = "RPS",
                SiteName = "user.auth.xboxlive.com",
                RpsTicket = "d=" + microsoftToken
            },
            RelyingParty = "http://auth.xboxlive.com",
            TokenType = "JWT"
        });
        var xboxResponseMessage = await NetWorkClient.PostAsync(
            "https://user.auth.xboxlive.com/user/authenticate",
            new StringContent(xboxAuthPostJson, Encoding.UTF8, "application/json")
        );

        var xboxAuthObject = JObject.Parse(await xboxResponseMessage.Content.ReadAsStringAsync());
        return xboxAuthObject["Token"]?.ToString();
    }

    public async Task<XtstAuthToken> ObtainXstsAsync(string xboxToken)
    {
        var xtstAuthPostJson = JsonConvert.SerializeObject(new XtstContent
        {
            Properties = new XtstProperties
            {
                SandboxId = "RETAIL",
                UserTokens =
                [
                    xboxToken
                ]
            },
            RelyingParty = "rp://api.minecraftservices.com/",
            TokenType = "JWT"
        });
        var xtstResponseMessage = await NetWorkClient.PostAsync(
            "https://xsts.auth.xboxlive.com/xsts/authorize",
            new StringContent(xtstAuthPostJson, Encoding.UTF8, "application/json")
        );

        var xtstAuthObject = JObject.Parse(await xtstResponseMessage.Content.ReadAsStringAsync());
        return new XtstAuthToken
        {
            Uhs = xtstAuthObject["DisplayClaims"]?["xui"]?[0]?["uhs"]?.ToString(),
            XtstToken = xtstAuthObject["Token"]?.ToString()
        };
    }
}