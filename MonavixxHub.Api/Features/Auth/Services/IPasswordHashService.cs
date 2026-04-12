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
    byte[] Hash(string password);

    /// <summary>
    /// Verifies that the specified password matches the given hashed password.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="hash">The stored password hash to compare against.</param>
    /// <returns>True if the password is correct; otherwise, false.</returns>
    bool Verify(string password, byte[] hash);
}

