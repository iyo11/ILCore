using System.Security.Cryptography;
using System.Text;

namespace ILCore.Util;

public static class Crypto
{
    public static string DecodeBase64Url(string base64Url)
    {
        var padding = 4 - base64Url.Length % 4;
        if (padding < 4) base64Url += new string('=', padding);
        base64Url = base64Url.Replace('-', '+').Replace('_', '/');

        var bytes = Convert.FromBase64String(base64Url);
        return Encoding.UTF8.GetString(bytes);
    }
    
    public static bool ValidateFileSHA1(string path, ReadOnlySpan<char> sha1)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file {path} does not exist.");
        }
        using var sha1Hash = SHA1.Create();
        using var fileStream = File.OpenRead(path);
        var hashBytes = sha1Hash.ComputeHash(fileStream);
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString().AsSpan().SequenceEqual(sha1);
    }

    public static bool ValidateFileSHA256(string path, ReadOnlySpan<char> sha256)
    {
        using var sha256Alg = SHA256.Create();
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        // Compute the hash for the file.
        var hashBytes = sha256Alg.ComputeHash(stream);
        // Convert the hash bytes to a hexadecimal string.
        var computedSha256 = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        // Convert the provided hash into a comparable format.
        var expectedSha256 = new string(sha256.ToArray()).ToLowerInvariant();
        // Compare the computed and expected hashes.
        return computedSha256 == expectedSha256;
    }
}