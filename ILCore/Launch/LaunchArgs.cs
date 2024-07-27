using ILCore.Minecraft.Arguments;
using ILCore.Minecraft.Libraries;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ILCore.Launch
{
    public class LaunchArgs
    {
        public string PrepareArguments(string versionName, string minecraftPath, string maxMemory, string jvmArgs, string userName)
        {
            var versionPath = $@"{minecraftPath}\versions\{versionName}";
            var jvmMemoryArgs =
                $"-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xmn921m -Xmx{maxMemory}m";
            var json = File.ReadAllText($@"{versionPath}\{versionName}.json");

            var versionObj = JObject.Parse(json);
            var librariesObj = versionObj["libraries"];

            var argumentsBuilder = new StringBuilder();
            var nativesPath = $@"-Djava.library.path={minecraftPath}\versions\{versionName}\natives";
            var assetsDir = $@"{minecraftPath}\assets";
            var assetsIndex = versionObj["assetIndex"]?["id"]?.ToString();
            if (assetsIndex is not null && assetsIndex.Equals("legacy"))
            {
                assetsDir = $@"{minecraftPath}\assets\virtual\legacy";
            }

            var minecraftArgument = new MinecraftArguments(
                userName: userName,
                versionName: versionObj["id"]?.ToString(),
                gameDir: $@"{minecraftPath}\versions\{versionName}",
                assetsDir: assetsDir,
                assetsIndex: assetsIndex,
                uuid: "{}",
                accessToken: "{}",
                userType: UserType.legacy,
                userProperties: "{}",
                versionType: versionObj["type"]?.ToString()
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
                        libraryArgumentsBuilder.Append($@"{minecraftPath}\libraries\{library.Path.Replace("/", @"\")};");
                    }
                }
            }

            libraryArgumentsBuilder.Append($@"{minecraftPath}\versions\{versionName}\{versionName}.jar");

            if (versionObj["arguments"] is not null)
            {
                var launchArgument = new LaunchArguments();

                var launchArguments = launchArgument.ToLaunchArguments(versionObj);
                launchArguments = launchArguments?
                    .Replace("${natives_directory}", $@"{minecraftPath}\versions\{versionName}\natives")
                    .Replace("${launcher_name}", "Minecraft")
                    .Replace("${launcher_version}", "0.1")
                    .Replace("${classpath}", libraryArgumentsBuilder.ToString())
                    .Replace("${version_name}", versionName)
                    .Replace("${library_directory}", $@"{minecraftPath}\libraries")
                    .Replace("${classpath_separator}", ";");

                argumentsBuilder.Append($"{jvmArgs} {jvmMemoryArgs} {launchArguments} ");
                argumentsBuilder.Append($" {versionObj["mainClass"]} ");
                argumentsBuilder.Append($" {minecraftArguments} ");
            }
            else
            {
                //jvmArgs = $"-XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True -Dlog4j2.formatMsgNoLookups=true";
                argumentsBuilder.Append($"{jvmArgs} {jvmMemoryArgs} {nativesPath} ");
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
