using System.Security.Claims;
using System.Security.Cryptography;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Common.Services;
using MonavixxHub.Api.Features.Auth.DTOs;
using MonavixxHub.Api.Features.Auth.Exceptions;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides authentication-related operations such as login and registration.
/// </summary>
public class AuthService(
    IJwtTokenService jwtTokenService,
    AppDbContext dbContext,
    IPasswordHashService passwordHashService,
    IEmailCheckService emailCheckService,
    ILogger<AuthService> logger,
    ISessionService sessionService,
    IHttpContextAccessor httpContextAccessor,
    IRefreshTokenService refreshTokenService) : IAuthService
{
    /// <summary>
    /// Authenticates a user with the given username or email and password.
    /// </summary>
    /// <param name="usernameOrEmail">The username or email of the user attempting to log in.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>
    /// An <see cref="AuthResponseDto"/> containing the authentication token and the additional data.
    /// </returns>
    /// <exception cref="WrongUserCredentialsException">
    /// Thrown if the credentials are invalid or the user does not exist.
    /// </exception>
    public async ValueTask<AuthResponseDto> LoginAsync(string usernameOrEmail, string password)
    {
        logger.LogInformation("Login attempt for user '{Username}'.", usernameOrEmail);
        User? user = (emailCheckService.IsValid(usernameOrEmail)
            ? await dbContext.Users.SingleOrDefaultAsync(u => u.Email == usernameOrEmail)
            : await dbContext.Users.SingleOrDefaultAsync(u => u.Username == usernameOrEmail));
        if (user is null || !passwordHashService.Verify(password, user.PasswordHash))
            throw new WrongUserCredentialsException();
        await sessionService.StartSessionAsync(user.Id);
        AddJwtToCookie(user);
        return new AuthResponseDto(user.Id, user.Username, user.Email);
    }

    public void AddJwtToCookie(User user)
    {
        var (token, expires) = jwtTokenService.GenerateToken(user);
        httpContextAccessor.HttpContext!.Response.Cookies.Append("JwtToken", token,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = expires,
                Secure = true,
                Path = "/"
            });
    }

    /// <summary>
    /// Registers a new user with the specified username, password, and email.
    /// </summary>
    /// <param name="username">The desired username.</param>
    /// <param name="password">The desired password.</param>
    /// <param name="email">The user's email address.</param>
    /// <returns>
    /// An <see cref="AuthResponseDto"/> containing the authentication token
    /// and the additional data of the newly registered user.
    /// </returns>
    /// <exception cref="DbUpdateException">
    /// Thrown if the username or email is already in use.
    /// </exception>
    public async ValueTask<AuthResponseDto> RegisterAsync(string username, string password, string email)
    {
        logger.LogInformation("Registration attempt for user '{Username}' with email '{Email}'.", username, email);

        var tokenBytes = GenerateConfirmationToken();
        var token = Convert.ToBase64String(tokenBytes);
        User user = new User()
        {
            Email = email,
            Username = username,
            PasswordHash = passwordHashService.Hash(password),
            CreatedAt = DateTimeOffset.UtcNow,
            IsEmailConfirmed = false,
            EmailConfirmationToken = tokenBytes,
            EmailConfirmationTokenExpiresAt = DateTimeOffset.UtcNow.AddMinutes(15)
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Successfully registered user {Username} with email {Email}.", username, email);

        BackgroundJob.Enqueue<EmailService>(e => e.SendConfirmationAsync(email, token));

        await sessionService.StartSessionAsync(user.Id);
        AddJwtToCookie(user);
        return new AuthResponseDto(user.Id, user.Username, user.Email);
    }
    private static byte[] GenerateConfirmationToken() 
        => RandomNumberGenerator.GetBytes(32);

    public async Task ConfirmEmailAsync(string token)
    {
        byte[] tokenBytes = Convert.FromBase64String(token);
        var now = DateTimeOffset.UtcNow;
        int updatedRows = await dbContext.Users
            .Where(u => u.EmailConfirmationToken == tokenBytes
                        && u.EmailConfirmationTokenExpiresAt > now)
            .ExecuteUpdateAsync(builder =>
            {
                builder.SetProperty(u => u.IsEmailConfirmed, true);
                builder.SetProperty(u => u.EmailConfirmationToken, (byte[]?)null);
                builder.SetProperty(u => u.EmailConfirmationTokenExpiresAt, (DateTimeOffset?)null);
            });
        if (updatedRows <= 0)
            throw new InvalidEmailConfirmationTokenException();
    }

    public async Task NewEmailConfirmationTokenAsync(ClaimsPrincipal user)
    {
        var email = user.GetEmail();
        var id = user.GetUserId();
        var expires = DateTimeOffset.UtcNow.AddMinutes(15);
        var tokenBytes = GenerateConfirmationToken();
        var token = Convert.ToBase64String(tokenBytes);
        await dbContext.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(builder =>
            {
                builder.SetProperty(u => u.IsEmailConfirmed, false);
                builder.SetProperty(u => u.EmailConfirmationToken, tokenBytes);
                builder.SetProperty(u => u.EmailConfirmationTokenExpiresAt, expires);
            });
        BackgroundJob.Enqueue<EmailService>(e => e.SendConfirmationAsync(email, token));
    }

    public async Task<User> Refresh()
    {
        if (!httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            throw new RefreshTokenNotFoundException();
        var session = await sessionService.EnsureSessionIsValidAsync(refreshToken);
        var user = await dbContext.Users.FindAsync(session.UserId);
        if (user is null) throw new UserDoesNotExistException();
        AddJwtToCookie(user);
        return user;
    }

    public async Task Logout()
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        };

        httpContextAccessor.HttpContext!.Response.Cookies.Delete("JwtToken", options);
        httpContextAccessor.HttpContext!.Response.Cookies.Delete("refreshToken", options);
        if (!httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            throw new RefreshTokenNotFoundException();
        var rt = refreshTokenService.Hash(refreshToken);
        if (await dbContext.Sessions
                .Where(s => s.RefreshTokenHash == rt)
                .ExecuteDeleteAsync() <= 0)
        {
            throw new SessionNotFoundException();
        }
    }
}