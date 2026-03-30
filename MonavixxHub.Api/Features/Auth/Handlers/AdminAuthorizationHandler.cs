using Microsoft.AspNetCore.Authorization;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.Handlers;

public class AdminAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        try
        {
            if (context.User.IsInRole(nameof(UserRole.Admin)))
            {
                foreach(var requirement in context.PendingRequirements)
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }
}