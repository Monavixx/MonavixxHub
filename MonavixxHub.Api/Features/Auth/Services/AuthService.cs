using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.DTOs;
using MonavixxHub.Api.Features.Auth.Exceptions;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides authentication-related operations such as login and registration.
/// </summary>
public class AuthService(
    TokenService tokenService,
    AppDbContext dbContext,
    PasswordHashService passwordHashService,
    EmailCheckService emailCheckService,
    ILogger<AuthService> logger)
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
        if (user is null)
            throw new WrongUserCredentialsException();
        if (passwordHashService.Verify(password, user.PasswordHash))
            return new AuthResponseDto(tokenService.GenerateToken(user), user.Username);
        throw new WrongUserCredentialsException();
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
    /// <exception cref="UniqueConstraintException">
    /// Thrown if the username or email is already in use.
    /// </exception>
    public async ValueTask<AuthResponseDto> RegisterAsync(string username, string password, string email)
    {
        logger.LogInformation("Registration attempt for user '{Username}' with email '{Email}'.", username, email);

        User user = new User()
        {
            Email = email,
            Username = username,
            PasswordHash = passwordHashService.Hash(password),
            CreatedAt = DateTimeOffset.UtcNow
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Successfully registered user {Username} with email {Email}.", username, email);
        return new AuthResponseDto(tokenService.GenerateToken(user), user.Username);
    }
}