using System.Security.Cryptography;

namespace Blazor_Serverside_Programming.Services;

public class RsaHandler
{
    private readonly RSA _rsa;

    public RsaHandler()
    {
        _rsa = RSA.Create(2048);
    }

    public string GetPublicKey()
    {
        return Convert.ToBase64String(
            _rsa.ExportSubjectPublicKeyInfo());
    }

    public byte[] Decrypt(byte[] data)
    {
        return _rsa.Decrypt(
            data,
            RSAEncryptionPadding.OaepSHA256);
    }
}