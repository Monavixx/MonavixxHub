using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MonavixxHub.Api.Common;
using MonavixxHub.Api.Features.Auth.DTOs;
using MonavixxHub.Api.Features.Auth.Exceptions;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Auth.Services;

namespace MonavixxHub.Api.Features.Auth.Controllers;

/// <summary>
/// Provides endpoints to register and login.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController (IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Registers a new user and returns a new jwt token.
    /// </summary>
    /// <param name="registerDto">User registration data.</param>
    /// <returns>Response with jwt token and username.</returns>
    [EnableRateLimiting(Policies.RegisterRateLimiting)]
    [HttpPost("register")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response =
            await authService.RegisterAsync(registerDto.Username, registerDto.Password, registerDto.Email);
        return Ok(response);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        try
        {
            await authService.ConfirmEmailAsync(token);
            return Content(
                """
                <html>
                <body>
                    <h1>Email confirmed!</h1>
                    <p>You can now close this page.</p>
                </body>
                </html>
                """, "text/html");
        }
        catch (InvalidEmailConfirmationTokenException)
        {
            return Content(
                """
                <html>
                <body>
                    <h1>Invalid or expired token</h1>
                    <p>Please request a new confirmation email.</p>
                </body>
                </html>
                """, "text/html");
        }
        catch (Exception e)
        {
            return Content(
                $"""
                <html>
                <body>
                    <h1>Internal server error {e.Message}</h1>
                </body>
                </html>
                """, "text/html");
        }
    }

    [Authorize] //todo: policy isemailconfirmation false
    [HttpPost("new-email-confirmation-token")]
    public async Task<IActionResult> NewEmailConfirmationToken()
    {
        await authService.NewEmailConfirmationTokenAsync(User);
        return NoContent();
    }

    /// <summary>
    /// First, Determines whether provided <see cref="LoginDto.UsernameOrEmail"/> is username or email.
    /// Then, checks if a user with provided credentials exists.
    /// If so, returns a new jwt token.
    /// </summary>
    /// <param name="loginDto">Credentials of the user.</param>
    /// <returns>Response with jwt token and username.</returns>
    [EnableRateLimiting(Policies.LoginRateLimiting)]
    [HttpPost("login")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response =
            await authService.LoginAsync(loginDto.UsernameOrEmail, loginDto.Password);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var user = await authService.Refresh();
        return Ok(new AuthResponseDto(user.Id, user.Username, user.Email));
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await authService.Logout();
        return NoContent();
    }
}
// TODO: cookie handling to controller