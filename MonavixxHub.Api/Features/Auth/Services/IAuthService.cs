using System.Security.Claims;
using MonavixxHub.Api.Features.Auth.DTOs;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides authentication-related operations such as login, registration, and token management.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with the given username or email and password.
    /// </summary>
    /// <param name="usernameOrEmail">The username or email of the user attempting to log in.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>The authenticated user.</returns>
    /// <exception cref="WrongUserCredentialsException">Thrown if the credentials are invalid or the user does not exist.</exception>
    Task<User> LoginAsync(string usernameOrEmail, string password);

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <returns>A tuple containing the JWT token string and its expiration time.</returns>
    (string Token, DateTimeOffset Expires) GenerateJwt(User user);

    /// <summary>
    /// Registers a new user with the specified username, password, and email.
    /// </summary>
    /// <param name="username">The desired username.</param>
    /// <param name="password">The desired password.</param>
    /// <param name="email">The user's email address.</param>
    /// <returns>The newly created user.</returns>
    /// <exception cref="DbUpdateException">Thrown if the username or email is already in use.</exception>
    Task<User> RegisterAsync(string username, string password, string email);

    /// <summary>
    /// Confirms a user's email address using a confirmation token.
    /// </summary>
    /// <param name="token">The email confirmation token.</param>
    /// <exception cref="InvalidEmailConfirmationTokenException">Thrown if the token is invalid or expired.</exception>
    Task ConfirmEmailAsync(string token);

    /// <summary>
    /// Generates and sends a new email confirmation token to the user.
    /// </summary>
    /// <param name="user">The user's claims principal.</param>
    Task NewEmailConfirmationTokenAsync(ClaimsPrincipal user);

    /// <summary>
    /// Refreshes the user's authentication token using the refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token from the request cookie.</param>
    /// <returns>A tuple containing the user entity and the new refresh token.</returns>
    /// <exception cref="SessionNotFoundException">Thrown if no refresh token is found.</exception>
    /// <exception cref="UserDoesNotExistException">Thrown if the user does not exist.</exception>
    Task<(User User, byte[] NewRefreshToken)> RefreshAsync(string refreshToken);

    /// <summary>
    /// Logs out the user by deleting the session.
    /// </summary>
    /// <param name="refreshToken">The refresh token from the request cookie.</param>
    /// <exception cref="SessionNotFoundException">Thrown if no refresh token is found.</exception>
    /// <exception cref="SessionNotFoundException">Thrown if the session is not found.</exception>
    Task LogoutAsync(string refreshToken);
}

