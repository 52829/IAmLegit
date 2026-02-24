using System.Text.Json;
using IAmLegit.TransparencyAudit.Integrity;

var argsMap = ParseArgs(args);
if (!TryGetRequired(argsMap, "--app-dir", out var appDir) ||
    !TryGetRequired(argsMap, "--manifest", out var manifestPath) ||
    !TryGetRequired(argsMap, "--signature", out var signaturePath) ||
    !TryGetRequired(argsMap, "--public-key-base64", out var publicKeyBase64))
{
    PrintUsage();
    return 2;
}

var expectedAppName = argsMap.TryGetValue("--app-name", out var appName)
    ? appName
    : "IAmLegit App";

var verifier = new IntegrityVerifier();
var result = verifier.VerifyInstallation(
    appDir,
    manifestPath,
    signaturePath,
    publicKeyBase64,
    expectedAppName);

var output = JsonSerializer.Serialize(result, new JsonSerializerOptions
{
    WriteIndented = true
});

Console.WriteLine(output);
return result.SignatureValid && result.FilesValid ? 0 : 1;

static Dictionary<string, string> ParseArgs(string[] args)
{
    var map = new Dictionary<string, string>(StringComparer.Ordinal);
    for (var i = 0; i < args.Length; i++)
    {
        var key = args[i];
        if (!key.StartsWith("--", StringComparison.Ordinal))
        {
            continue;
        }

        if (i + 1 >= args.Length)
        {
            map[key] = string.Empty;
            continue;
        }

        var value = args[i + 1];
        if (value.StartsWith("--", StringComparison.Ordinal))
        {
            map[key] = string.Empty;
            continue;
        }

        map[key] = value;
        i++;
    }

    return map;
}

static bool TryGetRequired(Dictionary<string, string> map, string key, out string value)
{
    if (map.TryGetValue(key, out value!) && !string.IsNullOrWhiteSpace(value))
    {
        return true;
    }

    value = string.Empty;
    return false;
}

static void PrintUsage()
{
    Console.Error.WriteLine("Usage:");
    Console.Error.WriteLine("  dotnet run --project open-source/transparency-kit/IAmLegit.TransparencyAudit.csproj -- \\");
    Console.Error.WriteLine("    --app-dir <path_to_installed_app> \\");
    Console.Error.WriteLine("    --manifest <path_to_manifest.json> \\");
    Console.Error.WriteLine("    --signature <path_to_manifest.sig> \\");
    Console.Error.WriteLine("    --public-key-base64 <ed25519_public_key_base64> \\");
    Console.Error.WriteLine("    [--app-name \"IAmLegit App\"]");
}
