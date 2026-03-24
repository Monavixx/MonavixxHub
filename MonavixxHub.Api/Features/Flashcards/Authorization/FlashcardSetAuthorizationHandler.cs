using System.Reflection.Metadata;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;

public class FlashcardSetAuthorizationHandler : AuthorizationHandler<FlashcardSetAccessRequirement, FlashcardSet>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        FlashcardSetAccessRequirement requirement,
        FlashcardSet resource)
    {
        if (resource.IsPublic && requirement.AccessType is FlashcardSetAccessType.Read) context.Succeed(requirement);
        else
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (resource.OwnerId == userId)
                context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}