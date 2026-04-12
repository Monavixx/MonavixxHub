using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides session management for authenticated users.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Starts a new session for the specified user.
    /// </summary>
    /// <param name="userId">The user ID to start a session for.</param>
    /// <returns>The created session entity.</returns>
    Task<Session> StartSessionAsync(UserIdType userId);

    /// <summary>
    /// Validates and refreshes an existing session using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token for the session.</param>
    /// <returns>The new session created after validation.</returns>
    /// <exception cref="RefreshTokenNotFoundException">Thrown if the refresh token is not found.</exception>
    /// <exception cref="SessionExpiredException">Thrown if the session has expired.</exception>
    Task<Session> EnsureSessionIsValidAsync(string refreshToken);
}

