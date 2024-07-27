namespace ILCore.OAuth.MinecraftOAuth;

public class UserProfile
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Skin[] Skins { get; set; }
    public object[] Capes { get; set; }
    public ProfileActions ProfileActions { get; set; }
}

public class ProfileActions;

public class Skin
{
    public Guid Id { get; set; }
    public string State { get; set; }
    public Uri Url { get; set; }
    public string TextureKey { get; set; }
    public string Variant { get; set; }
}