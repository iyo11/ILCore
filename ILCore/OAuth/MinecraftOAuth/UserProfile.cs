using ILCore.Launch;

namespace ILCore.OAuth.MinecraftOAuth;

public abstract class UserProfile
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string AccessToken { get; set; }
    public UserType UserType { get; set; }
    public string RefreshToken { get; set; }
}

public class LegacyUserProfile : UserProfile
{
    public LegacyUserProfile()
    {
        UserType = UserType.legacy;
        Id = "{}";
        RefreshToken = "{}";
        AccessToken = "{}";
    }
}

public class MsaUserProfile : UserProfile
{
    public UserType UserType => UserType.msa;
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