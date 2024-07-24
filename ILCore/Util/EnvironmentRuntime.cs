using System.Runtime.InteropServices;

namespace ILCore.Util
{
    public static class EnvironmentRuntime
    {
        public static readonly OSPlatform Os = RuntimeInformation.OSDescription switch
        {
            _ when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => OSPlatform.Linux,
            _ when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => OSPlatform.Windows,
            _ when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => OSPlatform.OSX,
            _ => OSPlatform.Create("UNKNOWN")
        };

        public static readonly string Architecture = RuntimeInformation.OSArchitecture.ToString();

        public static readonly bool Is64Bit = Environment.Is64BitOperatingSystem;
    }
}
