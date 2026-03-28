using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth.Middlewares;

/// <summary>
/// Middleware that validates the existence of an authenticated user in the database.
/// </summary>
/// <remarks>
/// This middleware checks if the current request has an authenticated user and if the endpoint
/// requires authorization (<see cref="AuthorizeAttribute"/>). 
/// If the endpoint requires authorization:
/// - The middleware retrieves the user's ID from <see cref="ClaimsPrincipal"/>.
/// - It verifies that a user with this ID exists in the database (<see cref="AppDbContext.Users"/>).
/// - If the user does not exist, the request is rejected with HTTP 401 Unauthorized.
/// - Otherwise, the request proceeds to the next middleware.
/// </remarks>
public class ValidateUserMiddleware(RequestDelegate next, ILogger<ValidateUserMiddleware> logger)
{
    ///
    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.GetUserId();
            string username = context.User.GetUsername();
            string email = context.User.GetEmail();
            using var _ = logger.BeginScope(new Dictionary<string, object>
            {
                ["UserId"] = userId,
                ["Username"] = username,
                ["Email"] = email,
            });
            
            var hasAuthorize = context.GetEndpoint()?.Metadata
                .OfType<AuthorizeAttribute>()
                .Any() ?? false;
            if (!hasAuthorize)
            {
                logger.LogDebug("Skipping user validation. Endpoint does not require authorization.");
                await next(context);
                return;
            }

            logger.LogInformation("Checking user existence...");
            if (!await dbContext.Users.AnyAsync(x => x.Id == userId && x.Username == username && x.Email == email))
            {
                logger.LogWarning(
                    "Authenticated user not found in database. Rejecting request.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            logger.LogInformation("User validated successfully");
            await next(context);
            return;
        }
        
        logger.LogDebug("Request is not authenticated.");
        await next(context);
    }
}