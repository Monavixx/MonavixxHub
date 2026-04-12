using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides functionality to generate, hash, and verify refresh tokens.
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Gets a new random refresh token.
    /// </summary>
    byte[] NewRefreshToken { get; }

    /// <summary>
    /// Gets the expiration time for a refresh token.
    /// </summary>
    DateTimeOffset Expiry { get; }

    /// <summary>
    /// Hashes a refresh token (string format).
    /// </summary>
    /// <param name="refreshToken">The Base64-encoded refresh token string.</param>
    /// <returns>The hashed token.</returns>
    byte[] Hash(string refreshToken);

    /// <summary>
    /// Hashes a refresh token (byte format).
    /// </summary>
    /// <param name="refreshToken">The refresh token bytes.</param>
    /// <returns>The hashed token.</returns>
    byte[] Hash(byte[] refreshToken);

    /// <summary>
    /// Converts a refresh token from byte array to Base64 string.
    /// </summary>
    /// <param name="refreshToken">The refresh token bytes.</param>
    /// <returns>Base64-encoded string representation.</returns>
    string RefreshTokenToString(byte[] refreshToken);

    /// <summary>
    /// Converts a refresh token from Base64 string to byte array.
    /// </summary>
    /// <param name="refreshToken">The Base64-encoded refresh token string.</param>
    /// <returns>The token bytes.</returns>
    byte[] RefreshTokenFromString(string refreshToken);

    /// <summary>
    /// Verifies that the specified refresh token matches the given hash.
    /// </summary>
    /// <param name="refreshToken">The Base64-encoded refresh token to verify.</param>
    /// <param name="hash">The stored hash to compare against.</param>
    /// <returns>True if the token matches the hash; otherwise, false.</returns>
    bool Verify(string refreshToken, byte[] hash);
}

