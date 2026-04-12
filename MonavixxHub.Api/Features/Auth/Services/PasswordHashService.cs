using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides hashing and verification of user passwords.
/// </summary>
public class PasswordHashService : IPasswordHashService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int HashIterations = 600000;
    
    /// <summary>
    /// Generates a salted hash for the specified password.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>A byte array containing the hashed password and salt.</returns>
    public byte[] Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        return HashWithSalt(password, salt).Concat(salt).ToArray();
    }

    /// <summary>
    /// Verifies that the specified password matches the given hashed password.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="hash">The stored password hash to compare against.</param>
    /// <returns>True if the password is correct; otherwise, false.</returns>
    public bool Verify(string password, byte[] hash)
    {
        return HashWithSalt(password, GetSalt(hash).ToArray())
            .SequenceEqual(GetHash(hash));
    }

    private static byte[] HashWithSalt(string password, byte[] salt)
    {
        return KeyDerivation.Pbkdf2(password, salt,
            KeyDerivationPrf.HMACSHA256, HashIterations, HashSize);
    }

    private static Span<byte> GetSalt(byte[] hash) => hash.AsSpan(HashSize);
    private static Span<byte> GetHash(byte[] hash) => hash.AsSpan(0, HashSize);
    
}