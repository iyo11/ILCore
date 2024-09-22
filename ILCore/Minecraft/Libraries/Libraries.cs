using ILCore.Download.DownloadData;
using ILCore.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ILCore.Minecraft.Libraries;

public class Libraries
{
    public async Task<IEnumerable<Library>> GetLibraries(string minecraftPath, string versionName)
    {
        var versionPath = $@"{minecraftPath}\versions\{versionName}";
        var jsonPath = $@"{versionPath}\{versionName}.json";
        if (!File.Exists(jsonPath)) return null;
        var json = await File.ReadAllTextAsync(jsonPath);
        var jLibrariesToken = JToken.Parse(json).SelectToken("libraries");
        return ToLibraries(jLibrariesToken);
    }
    
    public async Task<IEnumerable<Library>> CheckIntegrityAsync(IEnumerable<Library> libraries, string minecraftPath = null)
    {
        if (libraries is null) return [];
        minecraftPath ??= Path.Combine(Environment.CurrentDirectory, ".minecraft");
        var query = libraries
            .AsParallel()
            .Where(lib =>
            {
                var libPath = $"{minecraftPath ?? Environment.CurrentDirectory + "/.minecraft"}/libraries/{lib.Path}";
                return !File.Exists(libPath) || (lib.Sha1 != null && !Crypto.ValidateFileSHA1(libPath, lib.Sha1));
            });

        return query;
    }

    public async Task<IEnumerable<DownloadItem>> ToDownloadItems(JToken jLibrariesToken, IDownloadUrl downloadUrlApi,
        string minecraftPath = null)
    {
        var libraries = ToLibraries(jLibrariesToken);
        libraries = await CheckIntegrityAsync(libraries, minecraftPath);
        var queue = libraries
            .AsParallel()
            .Select(obj =>
            new DownloadItem
            {
                Name = null,
                Path = $"{minecraftPath ?? Environment.CurrentDirectory + "/.minecraft"}/libraries/{obj.Path}",
                Url = downloadUrlApi.Library + obj.Url,
                Size = obj.Size,
                IsCompleted = false,
                DownloadedBytes = 0
            });
        return queue;
    }

    public IEnumerable<Library> ToLibraries(JToken jLibrariesToken)
    {
        var libraries = JsonConvert.DeserializeObject<IEnumerable<JsonLibrary>>(jLibrariesToken.ToString());

        foreach (var library in libraries)
            if (library.Natives is null)
            {
                var allowOsName = library.Rules?[0].Os?.Name;
                if (allowOsName != null)
                {
                    if (allowOsName.Equals(EnvironmentRuntime.Os.ToString().ToLower()))
                        yield return FormatArtifact(library);
                }
                else
                {
                    yield return FormatArtifact(library);
                }
            }
            else
            {
                if (library.Downloads?.Classifiers is not null)
                    switch (EnvironmentRuntime.Os.ToString())
                    {
                        case "WINDOWS":
                            if (library.Natives.TryGetValue("windows", out var windowsValue) &&
                                windowsValue is not null)
                                if (FormatClassifiers(windowsValue, library) is { } lib)
                                    yield return lib;
                            break;
                        case "LINUX":
                            if (library.Natives.TryGetValue("linux", out var linuxValue) && linuxValue is not null)
                                if (FormatClassifiers(linuxValue, library) is { } lib)
                                    yield return lib;
                            break;
                        case "OSX":
                            if (library.Natives.TryGetValue("osx", out var osxValue) && osxValue is not null)
                                if (FormatClassifiers(osxValue, library) is { } lib)
                                    yield return lib;
                            break;
                    }
            }
    }

    private Library FormatArtifact(JsonLibrary library)
    {
        var nameArgs = library.Name.Split(':');
        var fileInfo = library.Downloads?.Artifact;

        string namePath;
        if (nameArgs.Length > 3)
            namePath = string.Format("{0}/{1}/{2}/{1}-{2}-{3}.jar", nameArgs[0].Replace('.', '/'), nameArgs[1],
                nameArgs[2], nameArgs[3]);
        else
            namePath = string.Format("{0}/{1}/{2}/{1}-{2}.jar", nameArgs[0].Replace('.', '/'), nameArgs[1],
                nameArgs[2]);

        var lib = new Library
        {
            Name = library.Name,
            Path = fileInfo?.Path ?? namePath,
            Size = fileInfo?.Size ?? 0,
            Sha1 = fileInfo?.Sha1 ?? "",
            Rules = library.Rules
        };

        if (nameArgs[0] == "net.minecraftforge" && nameArgs[1] == "forge")
        {
            lib.Type = LibraryType.ForgeMain;
            lib.Url = $"{nameArgs[2]}/forge-{nameArgs[2]}-universal.jar";
        }
        else if (library.Downloads?.Artifact?.Url.StartsWith("https://files.minecraftforge.net/maven/") ??
                 library.Url is "http://files.minecraftforge.net/maven/")
        {
            lib.Type = LibraryType.Forge;
            lib.Url = library.Downloads?.Artifact?.Url[39..];
        }
        else
        {
            switch (library.Url)
            {
                case "https://maven.minecraftforge.net/" or "http://maven.minecraftforge.net/":
                    lib.Type = LibraryType.Forge;
                    lib.Url = lib.Path;
                    break;
                case "https://maven.fabricmc.net/":
                    lib.Type = LibraryType.Fabric;
                    lib.Url = library.Url + lib.Path;
                    break;
                default:
                    lib.Type = LibraryType.Minecraft;
                    lib.Url = lib.Path;
                    break;
            }
        }


        return lib;
    }

    private Library FormatClassifiers(string os, JsonLibrary library)
    {
        var nameArgs = library.Name.Split(':');
        if (os.EndsWith("${arch}")) os = os.Replace("${arch}", EnvironmentRuntime.Is64Bit ? "64" : "32");

        var fileInfo = new JsonDownloadFile();
        library.Downloads?.Classifiers.TryGetValue(os, out fileInfo);

        if (fileInfo is null)
            return null;

        var lib = new Library
        {
            Name = $"{nameArgs[1]}-{nameArgs[2]}-{os}",
            Type = LibraryType.Native,
            Path = fileInfo.Path ??
                   string.Format("{0}/{1}/{2}/{1}-{2}-{3}.jar",
                       nameArgs[0].Replace('.', '/'), nameArgs[1], nameArgs[2], os),
            Size = fileInfo.Size,
            Sha1 = fileInfo.Sha1,
            Url = fileInfo.Url[32..],
            Rules = library.Rules,
            Exclude = library.Extract?.Exclude
        };
        return lib;
    }
}