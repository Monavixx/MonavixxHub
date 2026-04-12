using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides access to the currently authenticated user from the HTTP context.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user from the HTTP context.
    /// </summary>
    /// <returns>The current user entity.</returns>
    /// <exception cref="UserDoesNotExistException">Thrown if the user is not found in the database.</exception>
    ValueTask<User> GetUserAsync();
}

