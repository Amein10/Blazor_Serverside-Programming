using System.Security.Cryptography;
using System.Text;

namespace Blazor_Serverside_Programming.Services;

public class HashingService : IHashingService
{
    public string Sha256Hash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    public string HmacSha256Hash(string input, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(input);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(inputBytes);

        return Convert.ToBase64String(hash);
    }

    public string Pbkdf2Hash(string input, byte[] salt, string pepper, int iterations)
    {
        var valueToHash = input + pepper;

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password: valueToHash,
            salt: salt,
            iterations: iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32);

        return Convert.ToBase64String(hash);
    }

    public string BCryptHash(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input);
    }

    public string HmacSha256Hash(byte[] input, byte[] key)
    {
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(input);

        return Convert.ToBase64String(hash);
    }
}