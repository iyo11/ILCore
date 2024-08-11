using ILCore.OAuth.MinecraftOAuth;

namespace ILCore.Launch;

public class LaunchInfo
{
    public string VersionName { get; set; }
    public bool AbsoluteVersion { get; set; }
    public string MinecraftPath { get; set; }
    public string MaxMemory { get; set; }
    public string JvmArgs { get; set; }
    public IUserProfile UserProfile { get; set; }
    public string LauncherName { get; set; }
    public string LauncherVersion { get; set; }
    public string CustomArgs { get; set; }
    public string WindowTitle { get; set; }
    public string ServerAddress { get; set; }
    public string Port { get; set; }
    public bool Fullscreen { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
}