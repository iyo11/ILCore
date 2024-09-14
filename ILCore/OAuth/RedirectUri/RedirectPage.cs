using System.Reflection;
using System.Text;

namespace ILCore.OAuth.RedirectUri;

public class RedirectPage(RedirectMessage redirectMessage, AuthStatus authStatus)
{
    private const string RedirectPageNamespace = "ILCore.OAuth.RedirectUri";

    public byte[] ToPageBuffers()
    {
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream($"{RedirectPageNamespace}.RedirectPage.html");
        if (stream == null) return null;
        using var reader = new StreamReader(stream);
        var htmlContent = reader.ReadToEnd();

        byte[] buffer;
        switch (authStatus)
        {
            case AuthStatus.Success:
                htmlContent = htmlContent
                    .Replace("${Header}", redirectMessage.TitleSuccess)
                    .Replace("${Body}", redirectMessage.ContentSuccess);
                buffer = Encoding.UTF8.GetBytes(htmlContent);
                break;
            case AuthStatus.Error:
                htmlContent = htmlContent
                    .Replace("${Header}", redirectMessage.TitleError)
                    .Replace("${Body}", redirectMessage.ContentError);
                buffer = Encoding.UTF8.GetBytes(htmlContent);
                break;
            case AuthStatus.Interrupt:
            default:
                htmlContent = htmlContent
                    .Replace("${Header}", redirectMessage.TitleInterrupt)
                    .Replace("${Body}", redirectMessage.ContentInterrupt);
                buffer = Encoding.UTF8.GetBytes(htmlContent);
                break;
        }

        return buffer;
    }
}