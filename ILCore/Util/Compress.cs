using System.IO.Compression;

namespace ILCore.Util;
public static class Compress
{
    public static async Task ExtractJarAsync(string sourceJarPath, string targetDirectory, string[] excludePatterns)
    {

        if (!File.Exists(sourceJarPath))
        {
            throw new FileNotFoundException("The specified JAR file does not exist.", sourceJarPath);
        }

        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory ?? throw new ArgumentNullException(nameof(targetDirectory)));
        }

        await Task.Run(async () =>
        {
            await using var fileStream = File.OpenRead(sourceJarPath);
            using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);
            foreach (var entry in archive.Entries)
            {
                if (excludePatterns.Any(p => entry.FullName.Contains(p)))
                    continue;
                var targetPath = Path.Combine(targetDirectory, entry.FullName);
                
                entry.ExtractToFile(targetPath, true);
            }
        });
    }
}