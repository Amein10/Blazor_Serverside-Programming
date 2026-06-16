using System.Security.Cryptography;

namespace Blazor_FileTransferApp.Services;

public class AesHandler
{
    // Gammel AES-metode beholdes
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

    // Ny AES-GCM-metode
    public EncryptedGcmFileResult EncryptGcm(byte[] fileBytes)
    {
        var key = RandomNumberGenerator.GetBytes(32);   // 256-bit AES key
        var nonce = RandomNumberGenerator.GetBytes(12); // anbefalet størrelse til GCM
        var encryptedBytes = new byte[fileBytes.Length];
        var tag = new byte[16];                         // authentication tag

        using var aesGcm = new AesGcm(key, 16);

        aesGcm.Encrypt(
            nonce,
            fileBytes,
            encryptedBytes,
            tag);

        return new EncryptedGcmFileResult
        {
            EncryptedBytes = encryptedBytes,
            Key = key,
            Nonce = nonce,
            Tag = tag
        };
    }
}

public class EncryptedFileResult
{
    public byte[] EncryptedBytes { get; set; } = [];
    public byte[] Key { get; set; } = [];
    public byte[] IV { get; set; } = [];
}

public class EncryptedGcmFileResult
{
    public byte[] EncryptedBytes { get; set; } = [];
    public byte[] Key { get; set; } = [];
    public byte[] Nonce { get; set; } = [];
    public byte[] Tag { get; set; } = [];
}