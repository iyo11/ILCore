using ILCore.OAuth.MinecraftOAuth;

namespace ILCore.Launch;

public class LaunchInfo
{
    //string versionName, string minecraftPath, string maxMemory, string jvmArgs, string userName, UserType userType, UserProfile userProfile
    public string VersionName { get; set; }
    public string MinecraftPath { get; set; }
    public string MaxMemory { get; set; }
    public string JvmArgs { get; set; }
    public UserProfile UserProfile { get; set; }
    public string LauncherName { get; set; }
    public string LauncherVersion { get; set; }
    public bool AbsoluteVersion { get; set; }
}