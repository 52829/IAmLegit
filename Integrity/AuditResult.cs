namespace IAmLegit.TransparencyAudit.Integrity;

public sealed class AuditResult
{
    public required bool SignatureValid { get; init; }
    public required bool FilesValid { get; init; }
    public required string ManifestHashBase64 { get; init; }
    public required string AppVersion { get; init; }
    public required string FailureReason { get; init; }
}
