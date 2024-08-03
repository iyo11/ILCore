using ILCore.Minecraft.Arguments;
using ILCore.Minecraft.Libraries;
using Newtonsoft.Json.Linq;
using System.Text;


namespace ILCore.Launch
{
    public class LaunchArgs
    {
        public string PrepareArguments(LaunchInfo launchInfo)
        {
            var versionPath = $@"{launchInfo.MinecraftPath}\versions\{launchInfo.VersionName}";
            var jvmMemoryArgs =
                $"-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xmn921m -Xmx{launchInfo.MaxMemory}m";
            var json = File.ReadAllText($@"{versionPath}\{launchInfo.VersionName}.json");

            var versionObj = JObject.Parse(json);
            var librariesObj = versionObj["libraries"];

            var argumentsBuilder = new StringBuilder();
            var nativesPath = $@"-Djava.library.path={launchInfo.MinecraftPath}\versions\{launchInfo.VersionName}\natives";
            var assetsDir = $@"{launchInfo.MinecraftPath}\assets";
            var assetsIndex = versionObj["assetIndex"]?["id"]?.ToString();
            if (assetsIndex is not null && assetsIndex.Equals("legacy"))
            {
                assetsDir = $@"{launchInfo.MinecraftPath}\assets\virtual\legacy";
            }

            var minecraftArgument = new MinecraftArguments(
                userName: launchInfo.UserProfile.Name,
                versionName: versionObj["id"]?.ToString(),
                gameDir: $@"{launchInfo.MinecraftPath}\versions\{launchInfo.VersionName}",
                assetsDir: assetsDir,
                assetsIndex: assetsIndex,
                uuid: launchInfo.UserProfile.Id,
                accessToken: launchInfo.UserProfile.AccessToken,
                userType: launchInfo.UserProfile.UserType,
                userProperties: "{}",
                versionType: $"{versionObj["type"]?.ToString().ToUpper()}/{launchInfo.LauncherName}"
            );
            var minecraftArguments = minecraftArgument.ToMinecraftArguments(versionObj);

            var libraryArgumentsBuilder = new StringBuilder();
            if (librariesObj is not null)
            {
                var libraries = new Libraries();
                var enumerableLibs = libraries.ToLibraries(librariesObj);
                
                //取高版本Libs
                enumerableLibs = enumerableLibs
                    .Where(lib => lib.Type != LibraryType.Native) // 排除Native类型库
                    .GroupBy(lib =>
                    {
                        var nameArgs = lib.Name.Split(":");
                        var osPart = nameArgs.Length > 3 ? nameArgs[3] : "";
                        return $"{nameArgs[1]}:{osPart}";
                    }) // 假设路径中包含版本信息
                    .Select(group => group.MaxBy(lib => lib.Name.Split(":")[2]));


                foreach (var library in enumerableLibs)
                {
                    if (library.Type is not LibraryType.Native)
                    {
                        libraryArgumentsBuilder.Append($@"{launchInfo.MinecraftPath}\libraries\{library.Path.Replace("/", @"\")};");
                    }
                }
            }

            libraryArgumentsBuilder.Append($@"{launchInfo.MinecraftPath}\versions\{launchInfo.VersionName}\{launchInfo.VersionName}.jar");

            if (versionObj["arguments"] is not null)
            {
                var launchArgument = new LaunchArguments();

                var launchArguments = launchArgument.ToLaunchArguments(versionObj);
                launchArguments = launchArguments?
                    .Replace("${natives_directory}", $@"{launchInfo.MinecraftPath}\versions\{launchInfo.VersionName}\natives")
                    .Replace("${launcher_name}", $"{launchInfo.LauncherName}")
                    .Replace("${launcher_version}", $"{launchInfo.LauncherVersion}")
                    .Replace("${classpath}", libraryArgumentsBuilder.ToString())
                    .Replace("${version_name}", launchInfo.VersionName)
                    .Replace("${library_directory}", $@"{launchInfo.MinecraftPath}\libraries")
                    .Replace("${classpath_separator}", ";");

                argumentsBuilder.Append($"{launchInfo.JvmArgs} {jvmMemoryArgs} {launchArguments} ");
                argumentsBuilder.Append($" {versionObj["mainClass"]} ");
                argumentsBuilder.Append($" {minecraftArguments} ");
            }
            else
            {
                //jvmArgs = $"-XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True -Dlog4j2.formatMsgNoLookups=true";
                argumentsBuilder.Append($"{launchInfo.JvmArgs} {jvmMemoryArgs} {nativesPath} ");
                argumentsBuilder.Append("-cp ");
                argumentsBuilder.Append(libraryArgumentsBuilder);
                argumentsBuilder.Append($" {versionObj["mainClass"]} ");
                argumentsBuilder.Append($" {minecraftArguments} ");
            }


            argumentsBuilder.Append(" --width 854 --height 480");
            var arguments = argumentsBuilder.ToString();

            return arguments;
        }

    }
}
