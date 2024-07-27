using System.Text;

namespace ILCore.OAuth.RedirectUri;

public class RedirectPage
{
    private RedirectMessage _redirectMessage;
    private AuthStatus _authStatus;

    public RedirectPage(RedirectMessage redirectMessage,AuthStatus authStatus)
    {
        _redirectMessage = redirectMessage;
        _authStatus = authStatus;
    }

    public byte[] ToPageBuffers()
    {
        var html = "<html><head><style>body{font-family:Arial,sans-serif;line-height:1.6;color:#333;background-color:#f7f7f7;margin:50px auto;padding:0}.message-container{width:100%;max-width:600px;margin:0 auto;background-color:#fff;border-radius:5px;box-shadow:0 2px 5px rgba(0,0,0,0.1)}h1,h2{font-weight:normal;color:#fff;text-align:center}p{margin:0 0 15px}.highlight{color:#007bff;font-weight:bold}.header{border-radius:5px;background-color:${Color}}.content{padding:15px}</style><title>OAuth2</title></head><body><div class=\"message-container\"><div class=\"header\"><h2>${Header}</h2></div><div class=\"content\"><p>${Body}</p></div></div></body></html>";
        byte[] buffer;
        switch (_authStatus)
        {
            case AuthStatus.Success:
                html = html
                    .Replace("${Header}", _redirectMessage.TitleSuccess)
                    .Replace("${Body}", _redirectMessage.ContentSuccess)
                    .Replace("${Color}", "#28a745");
                buffer = Encoding.UTF8.GetBytes(html);
                break;
            case AuthStatus.Error:
                html = html
                    .Replace("${Header}", _redirectMessage.TitleError)
                    .Replace("${Body}", _redirectMessage.ContentError)
                    .Replace("${Color}", "#dc3545");
                buffer = Encoding.UTF8.GetBytes(html);
                break;
            case AuthStatus.Interrupt:
                default:
                html = html
                    .Replace("${Header}", _redirectMessage.TitleInterrupt)
                    .Replace("${Body}", _redirectMessage.ContentInterrupt)
                    .Replace("${Color}", "#ffc107");
                buffer = Encoding.UTF8.GetBytes(html);
                break;
        }

        return  buffer;
    }
}