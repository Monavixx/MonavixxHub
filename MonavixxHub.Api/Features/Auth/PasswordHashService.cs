using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MonavixxHub.Api.Features.Auth;

public class PasswordHashService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int HashIterations = 600000;
    
    public byte[] Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        return HashWithSalt(password, salt).Concat(salt).ToArray();
    }

    public bool Verify(string password, byte[] hash)
    {
        return HashWithSalt(password, GetSalt(hash).ToArray())
            .SequenceEqual(GetHash(hash));
    }

    private byte[] HashWithSalt(string password, byte[] salt)
    {
        return KeyDerivation.Pbkdf2(password, salt,
            KeyDerivationPrf.HMACSHA256, HashIterations, HashSize);
    }

    private Span<byte> GetSalt(byte[] hash) => hash.AsSpan(HashSize);
    private Span<byte> GetHash(byte[] hash) => hash.AsSpan(0, HashSize);
    
}