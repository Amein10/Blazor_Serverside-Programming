using System.Security.Cryptography;

namespace Blazor_Serverside_Programming.Services;

public class RsaHandler
{
    private readonly RSA _rsa;

    public RsaHandler()
    {
        _rsa = RSA.Create(2048);

        var keyFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Keys");

        Directory.CreateDirectory(keyFolder);

        var privateKeyPath = Path.Combine(
            keyFolder,
            "rsa_private_key.txt");

        if (File.Exists(privateKeyPath))
        {
            var privateKeyBase64 = File.ReadAllText(privateKeyPath);
            var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);

            _rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
        }
        else
        {
            var privateKeyBytes = _rsa.ExportPkcs8PrivateKey();
            var privateKeyBase64 = Convert.ToBase64String(privateKeyBytes);

            File.WriteAllText(privateKeyPath, privateKeyBase64);
        }
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