using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth.Middlewares;

public class ValidateUserMiddleware
{
    private readonly RequestDelegate _next;

    public ValidateUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var hasAuthorize = context.GetEndpoint()?.Metadata
                .OfType<AuthorizeAttribute>()
                .Any() ?? false;
            if (!hasAuthorize)
            {
                await _next(context);
                return;
            }

            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (!await dbContext.Users.AnyAsync(x => x.Id == userId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        await _next(context);
    }
}