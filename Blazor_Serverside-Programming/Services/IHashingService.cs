namespace Blazor_Serverside_Programming.Services;

public interface IHashingService
{
    string Sha256Hash(string input);
    string HmacSha256Hash(string input, string key);
    string Pbkdf2Hash(string input, byte[] salt, string pepper, int iterations);
    string BCryptHash(string input);

    string HmacSha256Hash(byte[] input, byte[] key);
}