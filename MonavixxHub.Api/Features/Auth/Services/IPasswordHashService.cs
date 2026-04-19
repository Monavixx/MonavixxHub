using MonavixxHub.Api.Features.Auth.Exceptions;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides hashing and verification of user passwords.
/// </summary>
public interface IPasswordHashService
{
    /// <summary>
    /// Generates a salted hash for the specified password.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>A byte array containing the hashed password and salt.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="password"/> is null.</exception>
    byte[] Hash(string password);

    /// <summary>
    /// Verifies that the specified password matches the given hashed password.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="hash">The stored password hash to compare against.</param>
    /// <returns>True if the password is correct; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="password"/> or <paramref name="hash"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="hash"/> is too short.</exception>
    bool Verify(string password, byte[] hash);
}

