using MonavixxHub.Api.Features.Auth.Exceptions;
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
    /// <returns>A tuple containing the created session entity and the raw refresh token.</returns>
    Task<(Session Session, byte[] RefreshToken)> CreateSessionAsync(UserIdType userId);

    /// <summary>
    /// Validates and refreshes an existing session using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token for the session.</param>
    /// <returns>The new session created after validation.</returns>
    /// <exception cref="SessionNotFoundException">Thrown if the session is not found.</exception>
    /// <exception cref="SessionExpiredException">Thrown if the session has expired.</exception>
    Task<(Session Session, byte[] RefreshToken)> RotateSessionAsync(string refreshToken);

    Task CleanExpiredSessionsAsync();
}

