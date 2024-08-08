using ILCore.Util;

namespace ILCore.Minecraft.Libraries;

public class Natives
{
    public async Task Extract(string minecraftPath,string versionName)
    {
        var libraries = await new Libraries().GetLibraries(minecraftPath,versionName);
        var nativesFolder = $@"{minecraftPath}\versions\{versionName}\natives";
        
        //取高版本Libs
        var nativeLibraries = libraries
            .Where(lib => lib.Type == LibraryType.Native)
            .GroupBy(lib =>
            {
                var nameArgs = lib.Path.Split('/');
                return nameArgs.Length switch
                {
                    6 => $"{nameArgs[3]}",
                    5 => $"{nameArgs[2]}",
                    _ => $"{nameArgs.Last()}"
                };
            })
            .Select(group => group.MaxBy(lib =>
            {
                var nameArgs = lib.Path.Split('/');
                return nameArgs[^2];
            }));
        
        foreach (var nativeLibrary in nativeLibraries)
        {
            try
            {
                await Compress.ExtractAsync($@"{minecraftPath}\libraries\{nativeLibrary.Path}", nativesFolder,
                    nativeLibrary.Exclude,[".dll"]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    
    
}