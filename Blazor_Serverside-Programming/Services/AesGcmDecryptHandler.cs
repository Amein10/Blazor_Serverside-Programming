using System.Security.Cryptography;

namespace Blazor_Serverside_Programming.Services;

public class AesGcmDecryptHandler
{
    public byte[] Decrypt(
        byte[] encryptedBytes,
        byte[] key,
        byte[] nonce,
        byte[] tag)
    {
        var decryptedBytes = new byte[encryptedBytes.Length];

        using var aesGcm = new AesGcm(key, 16);

        aesGcm.Decrypt(
            nonce,
            encryptedBytes,
            tag,
            decryptedBytes);

        return decryptedBytes;
    }
}