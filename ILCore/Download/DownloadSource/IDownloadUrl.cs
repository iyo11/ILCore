namespace ILCore.Download.DownloadSource;

public interface IDownloadUrl
{
    string VersionList { get; }

    string Version { get; }

    string Library { get; }

    string Json { get; }

    string Asset { get; }

    string ForgeList { get; }

    string Forge { get; }

    string ForgeMaven { get; }

    string Fabric { get; }

    string FabricMaven { get; }

    string AuthlibInjector { get; }
}