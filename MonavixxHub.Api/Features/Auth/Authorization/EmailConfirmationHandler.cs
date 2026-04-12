using Microsoft.AspNetCore.Authorization;
using MonavixxHub.Api.Features.Auth.Services;

namespace MonavixxHub.Api.Features.Auth.Authorization;

public class EmailConfirmationHandler (ICurrentUserService currentUserService)
    : AuthorizationHandler<EmailConfirmationRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        EmailConfirmationRequirement requirement)
    {
        var user = await currentUserService.GetUserAsync();
        if (user.IsEmailConfirmed)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, "Email confirmation required"));
        }
    }
}