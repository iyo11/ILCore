namespace ILCore.OAuth.XboxOAuth;

public class XboxAuthContent
{
    public XboxProperties Properties { get; set; }
    public string RelyingParty { get; set; }
    public string TokenType { get; set; }
}

public class XboxProperties
{
    public string AuthMethod { get; set; }
    public string SiteName { get; set; }
    public string RpsTicket { get; set; }
}

public class XtstContent
{
    public XtstProperties Properties { get; set; }
    public string RelyingParty { get; set; }
    public string TokenType { get; set; }
}

public class XtstProperties
{
    public string SandboxId { get; set; }
    public string[] UserTokens { get; set; }
}