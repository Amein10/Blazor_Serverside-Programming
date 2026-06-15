using System.Security.Cryptography;

namespace Blazor_Serverside_Programming.Services;

public class AesDecryptHandler
{
    public byte[] Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();

        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();

        return decryptor.TransformFinalBlock(
            encryptedBytes,
            0,
            encryptedBytes.Length);
    }
}