using Microsoft.AspNetCore.Authorization;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Admin.Authorization;

public class BanUserAuthorizationHandler : AuthorizationHandler<BanRequirement, User>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BanRequirement requirement, User resource)
    {
        if (resource.Role is UserRole.Admin) context.Fail();
        return Task.CompletedTask;
    }
}