using NSec.Cryptography;

namespace IAmLegit.TransparencyAudit.Crypto;

public static class Ed25519Verifier
{
    private static readonly SignatureAlgorithm Algorithm = SignatureAlgorithm.Ed25519;

    public static bool VerifyHash(byte[] publicKeyRaw, byte[] hashBytes, byte[] signatureBytes)
    {
        var publicKey = PublicKey.Import(Algorithm, publicKeyRaw, KeyBlobFormat.RawPublicKey);
        return Algorithm.Verify(publicKey, hashBytes, signatureBytes);
    }
}
