using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Auth.DTOs;

namespace MonavixxHub.Api.Features.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController (AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async ValueTask<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response =
            await authService.RegisterAsync(registerDto.Username, registerDto.Password, registerDto.Email);
        return Ok(response);
    }

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
}