using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Auth.DTOs;

namespace MonavixxHub.Api.Features.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController (AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async ValueTask<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var response =
                await authService.RegisterAsync(registerDto.Username, registerDto.Password, registerDto.Email);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async ValueTask<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response =
                await authService.LoginAsync(loginDto.UsernameOrEmail, loginDto.Password);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}