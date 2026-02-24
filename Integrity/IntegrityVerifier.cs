using System.Text.Json;
using IAmLegit.TransparencyAudit.Crypto;

namespace IAmLegit.TransparencyAudit.Integrity;

public sealed class IntegrityVerifier
{
    private static readonly JsonSerializerOptions ManifestJsonOptions = new()
    {
        PropertyNameCaseInsensitive = false
    };

    public AuditResult VerifyInstallation(
        string appDirectory,
        string manifestPath,
        string manifestSignaturePath,
        string releasePublicKeyBase64,
        string expectedAppName)
    {
        if (!File.Exists(manifestPath))
        {
            return Failed("manifest.json not found");
        }

        if (!File.Exists(manifestSignaturePath))
        {
            return Failed("manifest.sig not found");
        }

        var manifestBytes = File.ReadAllBytes(manifestPath);
        var manifestHashBytes = Sha256Helpers.HashBytes(manifestBytes);
        var manifestHashBase64 = Convert.ToBase64String(manifestHashBytes);

        ReleaseManifest? manifest;
        try
        {
            manifest = JsonSerializer.Deserialize<ReleaseManifest>(RemoveUtf8Bom(manifestBytes), ManifestJsonOptions);
        }
        catch (Exception ex)
        {
            return Failed($"manifest parse failed: {ex.Message}", manifestHashBase64);
        }

        if (manifest is null)
        {
            return Failed("manifest parse failed: empty", manifestHashBase64);
        }

        if (!string.Equals(manifest.AppName, expectedAppName, StringComparison.Ordinal))
        {
            return Failed("manifest app_name mismatch", manifestHashBase64, manifest.AppVersion);
        }

        bool signatureValid;
        try
        {
            var signatureRaw = File.ReadAllBytes(manifestSignaturePath);
            signatureValid = Ed25519Verifier.VerifyHash(
                Convert.FromBase64String(releasePublicKeyBase64),
                manifestHashBytes,
                signatureRaw);
        }
        catch (Exception ex)
        {
            return Failed($"manifest signature check failed: {ex.Message}", manifestHashBase64, manifest.AppVersion);
        }

        if (!signatureValid)
        {
            return Failed("manifest signature invalid", manifestHashBase64, manifest.AppVersion);
        }

        foreach (var fileEntry in manifest.Files)
        {
            var fullPath = Path.Combine(appDirectory, fileEntry.Path);
            if (!File.Exists(fullPath))
            {
                return Failed($"file missing: {fileEntry.Path}", manifestHashBase64, manifest.AppVersion, signatureValid);
            }

            var actualHash = Sha256Helpers.HashFileToHexLower(fullPath);
            if (!string.Equals(actualHash, fileEntry.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                return Failed($"sha256 mismatch: {fileEntry.Path}", manifestHashBase64, manifest.AppVersion, signatureValid);
            }
        }

        return new AuditResult
        {
            SignatureValid = true,
            FilesValid = true,
            ManifestHashBase64 = manifestHashBase64,
            AppVersion = manifest.AppVersion,
            FailureReason = string.Empty
        };
    }

    private static byte[] RemoveUtf8Bom(byte[] bytes)
    {
        if (bytes.Length >= 3 &&
            bytes[0] == 0xEF &&
            bytes[1] == 0xBB &&
            bytes[2] == 0xBF)
        {
            var copy = new byte[bytes.Length - 3];
            Buffer.BlockCopy(bytes, 3, copy, 0, copy.Length);
            return copy;
        }

        return bytes;
    }

    private static AuditResult Failed(
        string reason,
        string manifestHashBase64 = "",
        string appVersion = "unknown",
        bool signatureValid = false)
    {
        return new AuditResult
        {
            SignatureValid = signatureValid,
            FilesValid = false,
            ManifestHashBase64 = manifestHashBase64,
            AppVersion = appVersion,
            FailureReason = reason
        };
    }
}
