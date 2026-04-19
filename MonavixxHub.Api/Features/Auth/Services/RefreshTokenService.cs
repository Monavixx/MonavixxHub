using System.Security.Cryptography;
using System.Text;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Implementation of <see cref="IRefreshTokenService"/> that generates, hashes, and verifies refresh tokens.
/// Uses SHA256 with a configuration-based key for hashing.
/// </summary>
public class RefreshTokenService(IConfiguration configuration) : IRefreshTokenService
{
    /// <inheritdoc />
    public byte[] NewRefreshToken => RandomNumberGenerator.GetBytes(64);

    /// <inheritdoc />
    public DateTimeOffset Expiry => DateTimeOffset.UtcNow + Session.Expiration;

    /// <inheritdoc />
    public byte[] Hash(string refreshToken) => Hash(Convert.FromBase64String(refreshToken));

    /// <inheritdoc />
    public byte[] Hash(byte[] refreshToken)
        => SHA256.HashData(refreshToken.Concat(Encoding.UTF8.GetBytes(configuration["RefreshToken:Key"]!))
            .ToArray());

    /// <inheritdoc />
    public string RefreshTokenToString(byte[] refreshToken) => Convert.ToBase64String(refreshToken);

    /// <inheritdoc />
    public byte[] RefreshTokenFromString(string refreshToken) => Convert.FromBase64String(refreshToken);

    /// <inheritdoc />
    public bool Verify(string refreshToken, byte[] hash)
        => Hash(Convert.FromBase64String(refreshToken)).SequenceEqual(hash);
}