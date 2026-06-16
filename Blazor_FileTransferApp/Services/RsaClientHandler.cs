using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Blazor_FileTransferApp.Services;

public class RsaClientHandler
{
    public byte[] EncryptWithCertificate(byte[] data, string certificateBase64)
    {
        var certificateBytes = Convert.FromBase64String(certificateBase64);

        using var certificate = X509CertificateLoader.LoadCertificate(certificateBytes);
        using var rsa = certificate.GetRSAPublicKey();

        if (rsa is null)
        {
            throw new InvalidOperationException("Certificate does not contain an RSA public key.");
        }

        return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
    }

    public string GetCertificateThumbprint(string certificateBase64)
    {
        var certificateBytes = Convert.FromBase64String(certificateBase64);

        using var certificate =
        X509CertificateLoader.LoadCertificate(certificateBytes);

        return certificate.Thumbprint.Replace(" ", "").ToUpperInvariant();
    }
}