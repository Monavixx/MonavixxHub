using System.Security.Claims;

namespace MonavixxHub.Api.Features.Auth.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Retrieves the user ID from the <see cref="ClaimTypes.NameIdentifier"/> claim.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <returns>The user identifier parsed as an <see cref="int"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the claim is missing or cannot be parsed.
    /// </exception>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (value is null || !int.TryParse(value, out var userId))
            throw new InvalidOperationException("User ID claim is missing or invalid.");

        return userId;
    }

    public static string GetUsername(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Name)?.Value ??
           throw new InvalidOperationException("Username claim is missing or invalid.");
    public static string GetEmail(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Email)?.Value ??
           throw new InvalidOperationException("Email claim is missing or invalid.");
}