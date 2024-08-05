using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ILCore.Util;

public static class NetWorkClient
{
    private static readonly HttpClient HttpClient = new();
    
    
    public static async ValueTask<string> PostPairAsync(string url, Dictionary<string, string> dictionary, List<MediaTypeWithQualityHeaderValue> headerValues)
    {
        // 将键值对转换为 FormUrlEncodedContent
        var content = new FormUrlEncodedContent(dictionary);
        return await PostAsync(url, content, headerValues);
    }

    public static async ValueTask<string> PostJsonAsync(string url, string json, List<MediaTypeWithQualityHeaderValue> headerValues)
    {
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        // 将键值对转换为 FormUrlEncodedContent
        return await PostAsync(url, httpContent, headerValues);
    }

    private static async ValueTask<string> PostAsync(string url, HttpContent content, List<MediaTypeWithQualityHeaderValue> values)
    {
        // 设置请求方法和Content-Type头
        HttpClient.DefaultRequestHeaders.Accept.Clear();

        if (values != null)
        {
            foreach (var headerValue in values)
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(headerValue);
            }
        }

        // 发起POST请求
        var response = await HttpClient.PostAsync(url, content);

        // 检查响应状态码
        string responseString = null;
        if (response.IsSuccessStatusCode)
        {
            // 获取响应内容（这里假设是JSON格式并解析）
            responseString = await response.Content.ReadAsStringAsync();
        }
        return responseString;

    }

    public static async ValueTask<string> GetAsync(string url, AuthenticationHeaderValue value)
    {
        // 设置请求方法和Content-Type头
        HttpClient.DefaultRequestHeaders.Accept.Clear();

        if (value != null)
        {
            HttpClient.DefaultRequestHeaders.Authorization = value;
        }

        // 发起Get请求
        var response = await HttpClient.GetAsync(url);

        // 检查响应状态码
        string responseString = null;
        if (response.IsSuccessStatusCode)
        {
            // 获取响应内容（这里假设是JSON格式并解析）
            responseString = await response.Content.ReadAsStringAsync();
        }
        return responseString;

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
        {
            stringBuilder
                .Append(item.Key)
                .Append('=')
                .Append(WebUtility.UrlEncode(item.Value))
                .Append('&');
        }
        
        return stringBuilder.ToString();

    }
}