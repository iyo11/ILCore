using System.IO.Compression;

namespace ILCore.Util;

public static class Compress
{
    public static async Task ExtractAsync(string sourceJarPath, string targetDirectory, string[] excludePatterns,
        string[] suffixesPatterns)
    {
        if (!File.Exists(sourceJarPath)) throw new FileNotFoundException("The file does not exist.", sourceJarPath);

        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory ?? throw new ArgumentNullException(nameof(targetDirectory)));

        await Task.Run(async () =>
        {
            await using var fileStream = File.OpenRead(sourceJarPath);
            using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);

            excludePatterns ??= ["META-INF"];

            var files =
                archive.Entries
                    .Where(file => suffixesPatterns.Where(s => s != null).Any(suffix =>
                        Path.GetExtension(file.FullName).Equals(suffix, StringComparison.OrdinalIgnoreCase)))
                    .Where(file => !excludePatterns.Any(pattern => file.FullName.Contains(pattern)));
            foreach (var file in files)
            {
                var targetPath = Path.Combine(targetDirectory, file.FullName);
                file.ExtractToFile(targetPath, true);
            }
        });
    }
}