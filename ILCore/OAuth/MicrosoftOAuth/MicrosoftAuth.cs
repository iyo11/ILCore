using ILCore.Util;
using Newtonsoft.Json.Linq;

namespace ILCore.OAuth.MicrosoftOAuth;

public class MicrosoftAuth
{
    public async Task<MicrosoftAuthToken> MicrosoftAuthAsync(string authorizationCode, string clientId,
        string redirectUri, string scope)
    {
        var microsoftAuthorizationResponseMessage = await NetWorkClient.PostAsync(
            "https://login.live.com/oauth20_token.srf",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "code", authorizationCode },
                { "redirect_uri", redirectUri },
                { "scope", scope },
                { "grant_type", "authorization_code" }
            })
        );


        var microsoftAuthObject =
            JObject.Parse(await microsoftAuthorizationResponseMessage.Content.ReadAsStringAsync());

        return new MicrosoftAuthToken
        {
            AccessToken = microsoftAuthObject["access_token"]?.ToString(),
            RefreshToken = microsoftAuthObject["refresh_token"]?.ToString()
        };
    }

    public async Task<MicrosoftAuthToken> MicrosoftAuthRefreshAsync(string refreshCode, string clientId,
        string redirectUri, string scope)
    {
        var microsoftAuthorizationResponseMessage = await NetWorkClient.PostAsync(
            "https://login.live.com/oauth20_token.srf",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "code", refreshCode },
                { "redirect_uri", redirectUri },
                { "scope", scope },
                { "grant_type", "refresh_token" }
            })
        );


        var microsoftAuthObject =
            JObject.Parse(await microsoftAuthorizationResponseMessage.Content.ReadAsStringAsync());

        return new MicrosoftAuthToken
        {
            AccessToken = microsoftAuthObject["access_token"]?.ToString(),
            RefreshToken = microsoftAuthObject["refresh_token"]?.ToString()
        };
    }
}