namespace ILCore.Download.DownloadData;

public interface IDownloadUrl
{
    string VersionList { get; }
    string VersionListV2 { get; }

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