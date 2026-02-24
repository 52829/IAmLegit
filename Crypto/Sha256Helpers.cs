using System.Security.Cryptography;

namespace IAmLegit.TransparencyAudit.Crypto;

public static class Sha256Helpers
{
    public static byte[] HashBytes(byte[] bytes)
    {
        return SHA256.HashData(bytes);
    }

    public static string HashFileToHexLower(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
