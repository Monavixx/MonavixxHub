using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MonavixxHub.Api.Common;
using MonavixxHub.Api.Features.Auth.DTOs;
using MonavixxHub.Api.Features.Auth.Services;

namespace MonavixxHub.Api.Features.Auth.Controllers;

/// <summary>
/// Provides endpoints to register and login.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController (AuthService authService) : ControllerBase
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
    public async ValueTask<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (User.Identity?.IsAuthenticated is true)
            return Forbid();
        var response =
            await authService.RegisterAsync(registerDto.Username, registerDto.Password, registerDto.Email);
        return Ok(response);
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
    public async ValueTask<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response =
            await authService.LoginAsync(loginDto.UsernameOrEmail, loginDto.Password);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async ValueTask<IActionResult> Refresh()
    {
        await authService.Refresh();
        return NoContent();
    }
}
// TODO: cookie handling to controller