using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Images.Models;
using MonavixxHub.Api.Features.Images.Services;

namespace MonavixxHub.Api.Features.Images.Authorization;

public class ImageAuthorizationHandler (ImageAccessService imageAccessService)
    : AuthorizationHandler<ImageReadAccessRequirement, Image>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ImageReadAccessRequirement requirement,
        Image resource)
    {
        if (await imageAccessService.CanRead(resource.Id, context.User))
            context.Succeed(requirement);
    }
}