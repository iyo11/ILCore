using System.Net.Http.Headers;
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
        var xboxAuthorizationJson = await NetWorkClient.PostJsonAsync(
            "https://user.auth.xboxlive.com/user/authenticate",
            xboxAuthPostJson,
            [new MediaTypeWithQualityHeaderValue("application/json")]
        );

        var xboxAuthObject = JObject.Parse(xboxAuthorizationJson);
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
        var xtstAuthorizationJson = await NetWorkClient.PostJsonAsync(
            "https://xsts.auth.xboxlive.com/xsts/authorize",
            xtstAuthPostJson,
            [
                new MediaTypeWithQualityHeaderValue("application/json")
            ]
        );

        var xtstAuthObject = JObject.Parse(xtstAuthorizationJson);
        return new XtstAuthToken
        {
            Uhs = xtstAuthObject["DisplayClaims"]?["xui"]?[0]?["uhs"]?.ToString(),
            XtstToken = xtstAuthObject["Token"]?.ToString()
        };
    }
}