using System.Text.Json.Serialization;

namespace IAmLegit.TransparencyAudit.Integrity;

public sealed class ReleaseManifest
{
    [JsonPropertyName("schema_version")]
    public required string SchemaVersion { get; init; }

    [JsonPropertyName("app_name")]
    public required string AppName { get; init; }

    [JsonPropertyName("app_version")]
    public required string AppVersion { get; init; }

    [JsonPropertyName("build_utc")]
    public required DateTimeOffset BuildUtc { get; init; }

    [JsonPropertyName("files")]
    public required List<ReleaseManifestFile> Files { get; init; }
}

public sealed class ReleaseManifestFile
{
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    [JsonPropertyName("sha256")]
    public required string Sha256 { get; init; }
}
