using System.Security.Cryptography;

namespace Blazor_FileTransferApp.Services;

public class AesHandler
{
    public EncryptedFileResult Encrypt(byte[] fileBytes)
    {
        using var aes = Aes.Create();

        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();

        var encryptedBytes = encryptor.TransformFinalBlock(
            fileBytes,
            0,
            fileBytes.Length);

        return new EncryptedFileResult
        {
            EncryptedBytes = encryptedBytes,
            Key = aes.Key,
            IV = aes.IV
        };
    }
}

public class EncryptedFileResult
{
    public byte[] EncryptedBytes { get; set; } = [];

    public byte[] Key { get; set; } = [];

    public byte[] IV { get; set; } = [];
}