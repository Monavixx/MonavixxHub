using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Admin.Authorization;
using MonavixxHub.Api.Features.Admin.DTOs;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Features.Auth.Services;

namespace MonavixxHub.Api.Features.Admin.Controllers;

[Authorize(Roles = nameof(UserRole.Admin))]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    [HttpGet("users")]
    public IActionResult GetUsers(
        [FromServices] UserService userService,
        [FromQuery(Name = "page")] int page = 0,
        [FromQuery(Name = "limit")] int limit = 50)
    {
        return Ok(userService.GetUsers(page, limit).Select(AdminUserDto.Projection));
    }

    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser([FromServices] UserService userService, UserIdType userId)
    {
        await userService.DeleteUser(userId);
        return NoContent();
    }

    [HttpPost("users/batch-delete")]
    public async Task<IActionResult> DeleteUsers([FromServices] UserService userService,
        [FromBody] [MinLength(1)] ISet<UserIdType> userIds)
    {
        await userService.DeleteUsers(userIds);
        return NoContent();
    }

    [HttpPost("users/{userId}/ban")]
    public async Task<IActionResult> Ban([FromServices] UserService userService, UserIdType userId,
        [FromServices] IAuthorizationService authorizationService)
    {
        var user = await userService.GetUserAsync(userId);
        if(!(await authorizationService.AuthorizeAsync(User, user, Requirements.Ban)).Succeeded)
            return Forbid();
        await userService.BanAsync(user);
        return NoContent();
    }
}