using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Blazor_Serverside_Programming.Services;

public class CertificateHandler
{
    private readonly X509Certificate2 _certificate;

    public CertificateHandler(IConfiguration configuration)
    {
        var path = configuration["CertificateSettings:Path"];
        var password = configuration["CertificateSettings:Password"];

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException("Certificate path is not configured.");
        }

        _certificate = X509CertificateLoader.LoadPkcs12FromFile(
            path,
            password,
            X509KeyStorageFlags.MachineKeySet |
            X509KeyStorageFlags.PersistKeySet |
            X509KeyStorageFlags.Exportable);
    }

    public string GetCertificate()
    {
        return Convert.ToBase64String(_certificate.Export(X509ContentType.Cert));
    }

    public byte[] Decrypt(byte[] encryptedData)
    {
        using var rsa = _certificate.GetRSAPrivateKey();

        if (rsa is null)
        {
            throw new InvalidOperationException("Certificate does not contain an RSA private key.");
        }

        return rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
    }
}