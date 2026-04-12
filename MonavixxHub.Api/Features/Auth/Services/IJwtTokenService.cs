using System.Security.Claims;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides functionality to generate and validate authentication tokens for users.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token will be generated.</param>
    /// <returns>A tuple containing the JWT string and its expiration time.</returns>
    (string, DateTimeOffset) GenerateToken(User user);

    /// <summary>
    /// Validates the specified JWT token.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The claims principal if valid; otherwise, null.</returns>
    ClaimsPrincipal? Validate(string token);
}

