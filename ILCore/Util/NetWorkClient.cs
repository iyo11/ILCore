using System.Net;
using System.Text;

namespace ILCore.Util;

public static class NetWorkClient
{
    private static HttpClient _httpClient = CreateHttpClient();
    private static CookieContainer _cookieContainer = new();

    private static HttpClient CreateHttpClient()
    {
        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectTimeout = TimeSpan.FromSeconds(60),
            CookieContainer = _cookieContainer
        };
        return new HttpClient(socketsHttpHandler);
    }

    public static void InitHttpClient()
    {
        _cookieContainer = new CookieContainer();
        _httpClient = CreateHttpClient();
    }

    public static async Task<IEnumerable<Cookie>> GetCookies(string url)
    {
        var cookieContainer = new CookieContainer();
        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectTimeout = TimeSpan.FromSeconds(60),
            CookieContainer = cookieContainer
        };
        var httpClient = new HttpClient(socketsHttpHandler);
        var uri = new Uri(url);
        try
        {
            var response = await httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                Log.Warn($"Request failed with status code {response.StatusCode}.");
                return [];
            }

            var cookies = cookieContainer.GetCookies(uri).ToList();
            return cookies;
        }
        catch (Exception e)
        {
            Log.Error($"[GetCookiesException] {e}");
        }

        return [];
    }

    public static async Task<List<string>> GetCookiesAsStringAsync(string url)
    {
        return (await GetCookies(url)).Select(cookie => cookie.ToString()).ToList();
    }

    public static async Task<string> GetContentAsStringAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        return null;
    }


    public static async ValueTask<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (headers is not null)
            foreach (var keyValuePair in headers)
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) Log.Warn($"Request failed with status code {response.StatusCode}.");
            return response;
        }
        catch (Exception e)
        {
            Log.Error($"[GetException] {e}");
            return null;
        }
    }

    public static async ValueTask<HttpResponseMessage> PostAsync(
        string url,
        HttpContent content,
        Dictionary<string, string> headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        if (headers is not null)
            foreach (var keyValuePair in headers)
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
        if (content is not null) request.Content = content;

        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) Log.Warn($"Request failed with status code {response.StatusCode}.");
            return response;
        }
        catch (Exception e)
        {
            Log.Error($"{e}");
            return null;
        }
    }

    public static string BuildUrl(string url, SortedDictionary<string, string> dic)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(url);
        stringBuilder.Append('?');
        stringBuilder.Append(BuildParam(dic));
        return stringBuilder.ToString();
    }

    private static string BuildParam(SortedDictionary<string, string> dic)
    {
        var stringBuilder = new StringBuilder();
        foreach (var item in dic)
            stringBuilder
                .Append(item.Key)
                .Append('=')
                .Append(WebUtility.UrlEncode(item.Value))
                .Append('&');

        return stringBuilder.ToString();
    }
}