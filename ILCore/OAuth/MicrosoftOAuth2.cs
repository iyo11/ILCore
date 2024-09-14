using System.Diagnostics;
using System.Net;
using ILCore.OAuth.MicrosoftOAuth;
using ILCore.OAuth.MinecraftOAuth;
using ILCore.OAuth.RedirectUri;
using ILCore.OAuth.XboxOAuth;
using ILCore.Util;
using Newtonsoft.Json;

namespace ILCore.OAuth;

public class MicrosoftOAuth2(
    string clientId,
    string uri,
    RedirectMessage redirectMessage,
    string scope = "XboxLive.signin offline_access")
{
    /*
    public async Task<UserProfile> RefreshAsync(string refreshToken)
    {
        var userProfile = new UserProfile();
    }
    */

    public async Task<IUserProfile> AuthorizeAsync()
    {
        var userProfile = new MsaUserProfile();

        if (!HttpListener.IsSupported)
        {
            Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            return null;
        }

        if (uri == null)
            throw new ArgumentException("Prefixes cannot be null or empty.", nameof(uri));

        var redirectUri = uri + "/";

        var authUri = NetWorkClient.BuildUrl("https://login.live.com/oauth20_authorize.srf",
            new SortedDictionary<string, string>
            {
                { "client_id", clientId },
                { "response_type", "code" },
                { "redirect_uri", uri },
                { "scope", scope }
            });

        new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = authUri,
                UseShellExecute = true
            }
        }.Start();


        using var listener = new HttpListener();
        listener.Prefixes.Add(redirectUri);
        listener.Start();

        HttpListenerContext context = null;
        byte[] buffer;
        try
        {
            context = await listener.GetContextAsync();
            var response = context.Response;
            var url = context.Request.Url?.ToString();
            if (url != null && url.Contains("code="))
            {
                var code = url[(url.LastIndexOf('=') + 1)..];

                var microsoftAuth = new MicrosoftAuth();
                var xboxAuth = new XboxAuth();
                var minecraftAuth = new MinecraftAuth();

                var microsoftAuthToken = await microsoftAuth.MicrosoftAuthAsync(code, clientId, uri, scope);
                Console.WriteLine(microsoftAuthToken.AccessToken);
                
                var xboxAuthToken = await xboxAuth.XboxAuthAsync(microsoftAuthToken.AccessToken);
                Console.WriteLine(xboxAuthToken);
                
                var xtstAuthToken = await xboxAuth.ObtainXstsAsync(xboxAuthToken);
                Console.WriteLine(xtstAuthToken.XtstToken);

                var minecraftToken = await minecraftAuth.MinecraftAuthAsync(xtstAuthToken);
                Console.WriteLine(minecraftToken);

                var profile = await minecraftAuth.GetProfileAsync(minecraftToken);
                userProfile = JsonConvert.DeserializeObject<MsaUserProfile>(profile.ToString());
                userProfile.RefreshToken = microsoftAuthToken.RefreshToken;
                userProfile.AccessToken = minecraftToken;
                buffer = new RedirectPage(redirectMessage, AuthStatus.Success).ToPageBuffers();
            }
            else
            {
                throw new Exception("Invalid request");
            }

            response.ContentLength64 = buffer.Length;
            await using var output = response.OutputStream;
            await output.WriteAsync(buffer);
        }
        catch (Exception e)
        {
            redirectMessage.ContentError = e.ToString();
            buffer = new RedirectPage(redirectMessage, AuthStatus.Error).ToPageBuffers();
            if (context != null)
            {
                var response = context.Response;
                response.ContentLength64 = buffer.Length;
                await using var output = response.OutputStream;
                await output.WriteAsync(buffer);
            }
        }
        finally
        {
            listener.Stop();
        }

        return userProfile;
    }
}