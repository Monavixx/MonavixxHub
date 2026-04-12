using System.Net;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Common;
using MonavixxHub.Api.Features.Auth.Exceptions;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth.Services;

public class SessionService(
    IRefreshTokenService refreshTokenService,
    AppDbContext dbContext,
    IHttpContextAccessor httpContextAccessor) : ISessionService
{
    public async Task<Session> StartSessionAsync(UserIdType userId)
    {
        var token = refreshTokenService.NewRefreshToken;
        var session = new Session
        {
            UserId = userId,
            Ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress,
            ExpiresAt = refreshTokenService.Expiry,
            RefreshTokenHash = refreshTokenService.Hash(token)
        };
        dbContext.Sessions.Add(session);
        await dbContext.SaveChangesAsync();
        httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken",
            refreshTokenService.RefreshTokenToString(token), new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = session.ExpiresAt,
                Secure = true,
                Path = "/"
            });

        return session;
    }

    public async Task<Session> EnsureSessionIsValidAsync(string refreshToken)
    {
        byte[] hash = refreshTokenService.Hash(refreshToken);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var session = await dbContext.Sessions.SingleOrDefaultAsync(s => s.RefreshTokenHash == hash);
        if (session is null) throw new RefreshTokenNotFoundException();
        if (session.ExpiresAt <= now) throw new SessionExpiredException();
        dbContext.Sessions.Remove(session);
        return await StartSessionAsync(session.UserId);
    }
}