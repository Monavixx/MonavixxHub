using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Exceptions;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Service responsible for managing user sessions and refresh tokens.
/// Handles session creation, validation, and token rotation.
/// </summary>
public class SessionService(
    IRefreshTokenService refreshTokenService,
    AppDbContext dbContext,
    ILogger<SessionService> logger) : ISessionService
{
    /// <summary>
    /// Creates a new session for the specified user with a fresh refresh token.
    /// 
    /// This method generates a new JWT refresh token and creates an associated
    /// Session record in the database. The session is valid until the expiry time
    /// defined by the refresh token service.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to create a session for.</param>
    /// <returns>A tuple containing the newly created Session and the raw refresh token bytes.</returns>
    public async Task<(Session Session, byte[] RefreshToken)> CreateSessionAsync(UserIdType userId)
    {
        var token = refreshTokenService.NewRefreshToken;
        var session = new Session
        {
            UserId = userId,
            ExpiresAt = refreshTokenService.Expiry,
            RefreshTokenHash = refreshTokenService.Hash(token)
        };
        dbContext.Sessions.Add(session);
        await dbContext.SaveChangesAsync();

        return (session, token);
    }

    /// <summary>
    /// Validates an existing refresh token, rotates it if valid, and updates the session record.
    /// </summary>
    /// <param name="refreshToken">The refresh token string provided by the client.</param>
    /// <returns>The newly created Session object after successful validation and rotation.</returns>
    /// <exception cref="SessionNotFoundException">Thrown when the refresh token is not found in the database.</exception>
    /// <exception cref="SessionExpiredException">Thrown when the refresh token has expired.</exception>
    public async Task<(Session Session, byte[] RefreshToken)> RotateSessionAsync(string refreshToken)
    {
        byte[] hash = refreshTokenService.Hash(refreshToken);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var existingSession = await dbContext.Sessions.SingleOrDefaultAsync(s => s.RefreshTokenHash == hash);
        if (existingSession is null) throw new SessionNotFoundException();
        if (existingSession.ExpiresAt <= now) throw new SessionExpiredException();
        
        // 1. Remove the old session record
        dbContext.Sessions.Remove(existingSession);
        
        // 2. Create and save the new session
        return await CreateSessionAsync(existingSession.UserId);
    }

    public async Task CleanExpiredSessionsAsync()
    {
        var now = DateTimeOffset.UtcNow;
        var sessionsCleaned = await dbContext.Sessions.Where(s => s.ExpiresAt < now).ExecuteDeleteAsync();
        logger.LogInformation("Sessions cleaned: {SessionsCleaned}", sessionsCleaned);
    }
}
