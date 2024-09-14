using System.Text;
using ILCore.Exceptions;
using ILCore.Minecraft.Arguments;
using ILCore.Minecraft.Libraries;
using ILCore.Util;
using Newtonsoft.Json.Linq;

namespace ILCore.Launch;

public class LaunchArgs
{
    public async Task<string> PrepareArguments(LaunchInfo launchInfo)
    {
        var versionPath = $@"{launchInfo.MinecraftPath}\versions\{launchInfo.VersionName}";
        var jvmMemoryArgs =
            $"-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xmn921m -Xmx{launchInfo.MaxMemory}m";
        var jsonPath = $@"{versionPath}\{launchInfo.VersionName}.json";
        if (!File.Exists(jsonPath)) throw new IlCoreException(Language.GetValue("JsonNotFound"));
        var json = await File.ReadAllTextAsync(jsonPath);

        var versionObj = JObject.Parse(json);
        var librariesObj = versionObj["libraries"];

        var argumentsBuilder = new StringBuilder();
        var nativesPath = $@"-Djava.library.path={launchInfo.MinecraftPath}\versions\{launchInfo.VersionName}\natives";
        var assetsDir = $@"{launchInfo.MinecraftPath}\assets";
        var assetsIndex = versionObj["assetIndex"]?["id"]?.ToString();
        if (assetsIndex is not null && assetsIndex.Equals("legacy"))
            assetsDir = $@"{launchInfo.MinecraftPath}\assets\virtual\legacy";

        var minecraftArgument = new MinecraftArguments(
            launchInfo.UserProfile.Name,
            versionObj["id"]?.ToString(),
            launchInfo.AbsoluteVersion
                ? $@"{launchInfo.MinecraftPath}\versions\{launchInfo.VersionName}"
                : launchInfo.MinecraftPath,
            assetsDir,
            assetsIndex,
            launchInfo.UserProfile.Id,
            launchInfo.UserProfile.AccessToken,
            launchInfo.UserProfile.UserType,
            "{}",
            //versionType: $"{versionObj["type"]?.ToString().ToUpper()}/{launchInfo.LauncherName}/{launchInfo.CustomArgs}"
            launchInfo.CustomArgs
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
                if (library.Type is not LibraryType.Native)
                    libraryArgumentsBuilder.Append(
                        $@"{launchInfo.MinecraftPath}\libraries\{library.Path.Replace("/", @"\")};");
        }

        libraryArgumentsBuilder.Append(
            $@"{launchInfo.MinecraftPath}\versions\{launchInfo.VersionName}\{launchInfo.VersionName}.jar");

        if (versionObj["arguments"] is not null)
        {
            var launchArgument = new JvmArguments();

            var launchArguments = launchArgument.ToLaunchArguments(versionObj);
            launchArguments = launchArguments?
                .Replace("${natives_directory}",
                    $@"{launchInfo.MinecraftPath}\versions\{launchInfo.VersionName}\natives")
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

        if (!string.IsNullOrEmpty(launchInfo.ServerAddress))
        {
            argumentsBuilder.Append($" --server {launchInfo.ServerAddress}");
            if (!string.IsNullOrEmpty(launchInfo.Port))
                argumentsBuilder.Append($" --port {launchInfo.Port}");
            else
                argumentsBuilder.Append(" --port 25565");
        }


        argumentsBuilder.Append($" --width {launchInfo.WindowWidth} --height {launchInfo.WindowHeight}");
        if (launchInfo.Fullscreen) argumentsBuilder.Append(" --fullscreen");

        var arguments = argumentsBuilder.ToString();

        await File.WriteAllTextAsync("1.txt", arguments);

        return arguments;
    }
}