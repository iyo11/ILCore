using ILCore.Launch;

namespace ILCore.OAuth.MinecraftOAuth;

public interface IUserProfile
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string AccessToken { get; set; }

    public UserType UserType { get; set; }

    public string RefreshToken { get; set; }
}

public class LegacyUserProfile(string name) : IUserProfile
{
    public string Id { get; set; } = "ffffffffffffffffffffffffffffffff";

    public string Name { get; set; } = name;

    public string AccessToken { get; set; } = "{}";

    public UserType UserType { get; set; } = UserType.legacy;

    public string RefreshToken { get; set; } = "{}";
}

public class MsaUserProfile : IUserProfile
{
    public Skin[] Skins { get; set; }

    public object[] Capes { get; set; }

    public ProfileAction ProfileActions { get; set; }

    public string Id { get; set; }

    public string Name { get; set; }

    public string AccessToken { get; set; }

    public UserType UserType { get; set; } = UserType.msa;

    public string RefreshToken { get; set; }

    public class ProfileAction;

    public class Skin
    {
        public Guid Id { get; set; }
        public string State { get; set; }
        public Uri Url { get; set; }
        public string TextureKey { get; set; }
        public string Variant { get; set; }
    }
}