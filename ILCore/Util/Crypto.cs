using System.Text;

namespace ILCore.Util;

public class Crypto
{
    
    public static string DecodeBase64Url(string base64Url)
    {
        var padding = 4 - base64Url.Length % 4;
        if (padding < 4)
        {
            base64Url += new string('=', padding);
        }
        base64Url = base64Url.Replace('-', '+').Replace('_', '/');

        var bytes = Convert.FromBase64String(base64Url);
        return Encoding.UTF8.GetString(bytes);
    }
}