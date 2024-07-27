using System.Diagnostics;
using System.Net;
using ILCore.OAuth.MicrosoftOAuth;
using ILCore.OAuth.MinecraftOAuth;
using ILCore.OAuth.RedirectUri;
using ILCore.OAuth.XboxOAuth;
using ILCore.Util;
using Newtonsoft.Json;

namespace ILCore.OAuth;

public class MicrosoftOAuth2
{
    private readonly string _clientId;
    private readonly string _redirectUri;
    private readonly string _scope;
    
    public MicrosoftOAuth2(string clientId, string redirectUri, string scope = "XboxLive.signin offline_access")
    {
        _clientId = clientId;
        _redirectUri = redirectUri;
        _scope = scope;
    }
    
    public async Task<UserProfile> AuthorizeAsync()
    {
        var userProfile = new UserProfile();
        
        if (!HttpListener.IsSupported)
        {
            Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            return null;
        }

        if (_redirectUri == null)
            throw new ArgumentException("Prefixes cannot be null or empty.", nameof(_redirectUri));

        var redirectUri = _redirectUri + "/";
        
        var authUri = NetClient.BuildUrl("https://login.live.com/oauth20_authorize.srf",
            new SortedDictionary<string, string>
            {
                { "client_id", _clientId },
                { "response_type", "code" },
                { "redirect_uri", _redirectUri },
                { "scope", _scope }
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
                
                var microsoftAuthToken = await microsoftAuth.MicrosoftAuthAsync(code,_clientId,_redirectUri,_scope);

                var xboxAuthToken = await xboxAuth.XboxAuthAsync(microsoftAuthToken.AccessToken);
                
                var xtstAuthToken = await xboxAuth.ObtainXstsAsync(xboxAuthToken);
                    

                var minecraftToken = await minecraftAuth.MinecraftAuthAsync(xtstAuthToken);

                var profile = await minecraftAuth.GetProfileAsync(minecraftToken);
                userProfile = JsonConvert.DeserializeObject<UserProfile>(profile.ToString());
                
                buffer = new RedirectPage(new RedirectMessage(),AuthStatus.Success).ToPageBuffers();
            }
            else
            {
                throw  new Exception("Invalid request");
            }
            
            response.ContentLength64 = buffer.Length;
            await using var output = response.OutputStream;
            await output.WriteAsync(buffer);
        }
        catch (Exception e)
        {
            buffer = new RedirectPage(new RedirectMessage(contentError: e.ToString()),AuthStatus.Error).ToPageBuffers();
            if (context != null)
            {
                var response = context.Response;
                response.ContentLength64 = buffer.Length;
                await using var output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
            }
        }
        finally
        {
            listener.Stop();
        }
        
        return  userProfile;
    }
}