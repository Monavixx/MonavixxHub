using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;

public class FlashcardAuthorizationHandler (IServiceScopeFactory scopeFactory)
    : AuthorizationHandler<FlashcardAccessRequirement, Flashcard>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, FlashcardAccessRequirement requirement,
        Flashcard resource)
    {
        if (resource.OwnerId == int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value))
        {
            context.Succeed(requirement);
            return;
        }

        if (requirement.AccessType is FlashcardAccessType.Read)
        {
            using var scope = scopeFactory.CreateScope();
            if(await scope.ServiceProvider.GetService<FlashcardService>()!.IsPublicAsync(resource.Id))
                context.Succeed(requirement);
        }
    }
}