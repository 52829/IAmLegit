# Transparency Kit (Open Source)

This folder contains standalone open-source code for independent verification of IAmLegit desktop release integrity.

Goal:
- allow third-party auditors to verify that an installed app matches a signed release manifest;
- provide transparency without publishing anti-bypass implementation details.

## What This Kit Verifies
- `manifest.json` exists and is valid JSON;
- `manifest.sig` is a valid Ed25519 signature of `manifest.json`;
- every file listed in `manifest.json` exists in the target app directory;
- every listed file SHA-256 hash matches the manifest value.

## What Is Intentionally NOT Included
- client scanner internals;
- rule evaluation internals;
- runtime anti-cheat heuristics and thresholds.

These parts are excluded to avoid publishing details that can be used to bypass detection.

## Build
```bash
dotnet build open-source/transparency-kit/IAmLegit.TransparencyAudit.csproj -c Release
```

## Run
```bash
dotnet run --project open-source/transparency-kit/IAmLegit.TransparencyAudit.csproj -- \
  --app-dir "<installed_app_dir>" \
  --manifest "<manifest_json_path>" \
  --signature "<manifest_sig_path>" \
  --public-key-base64 "<release_public_key_base64>" \
  --app-name "IAmLegit App"
```

Example output:
```json
{
  "SignatureValid": true,
  "FilesValid": true,
  "ManifestHashBase64": "...",
  "AppVersion": "1.1.4",
  "FailureReason": ""
}
```

Exit codes:
- `0` verification passed;
- `1` verification failed;
- `2` invalid CLI arguments.

## Security Notes
- a valid result confirms binary integrity against the signed manifest;
- this kit does not claim formal malware-proof, but provides a reproducible integrity check that auditors can run independently.
