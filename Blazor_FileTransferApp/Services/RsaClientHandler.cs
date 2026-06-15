using System.Security.Cryptography;

namespace Blazor_FileTransferApp.Services;

public class RsaClientHandler
{
    public byte[] EncryptWithPublicKey(byte[] data, string publicKeyBase64)
    {
        var publicKeyBytes = Convert.FromBase64String(publicKeyBase64);

        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
    }
}