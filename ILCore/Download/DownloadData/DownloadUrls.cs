namespace ILCore.Download.DownloadData;

public class OfficialUrl : IDownloadUrl
{
    public string VersionList => "https://launchermeta.mojang.com/mc/game/version_manifest.json";
    public string VersionListV2 => "http://launchermeta.mojang.com/mc/game/version_manifest_v2.json";

    public string Version => "https://launcher.mojang.com/";

    public string Library => "https://libraries.minecraft.net/";

    public string Json => "https://launchermeta.mojang.com/";

    public string Asset => "https://resources.download.minecraft.net/";

    public string ForgeList => "https://bmclapi2.bangbang93.com/forge/minecraft/";

    public string Forge => "https://files.minecraftforge.net/maven/net/minecraftforge/forge/";

    public string ForgeMaven => "https://files.minecraftforge.net/maven/";

    public string Fabric => "https://meta.fabricmc.net/v2/versions/loader/";

    public string FabricMaven => "https://maven.fabricmc.net/";

    public string AuthlibInjector => "https://authlib-injector.yushi.moe/artifact/latest.json";
}

public class BmclApiUrl : IDownloadUrl
{
    public string Maven => "https://bmclapi2.bangbang93.com/maven/";
    public string VersionList => "https://bmclapi2.bangbang93.com/mc/game/version_manifest.json";
    public string VersionListV2 => "https://bmclapi2.bangbang93.com/mc/game/version_manifest_v2.json";

    public string Version => "https://bmclapi2.bangbang93.com/";

    public string Library => "https://bmclapi2.bangbang93.com/libraries/";

    public string Json => "https://bmclapi2.bangbang93.com/";

    public string Asset => "https://bmclapi2.bangbang93.com/assets/";

    public string ForgeList => "https://bmclapi2.bangbang93.com/forge/minecraft/";

    public string Forge => "https://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/";

    public string ForgeMaven => Maven;

    public string Fabric => "http://bmclapi.bangbang93.com/fabric-meta/v2/versions/loader/";

    public string FabricMaven => Maven;

    public string AuthlibInjector => "https://bmclapi2.bangbang93.com/mirrors/authlib-injector/artifact/latest.json";
}