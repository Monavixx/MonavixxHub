using Microsoft.AspNetCore.Authorization;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.Flashcards.Services;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;

public class FlashcardAuthorizationHandler (FlashcardAccessService flashcardAccessService)
    : AuthorizationHandler<FlashcardAccessRequirement, Flashcard>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, FlashcardAccessRequirement requirement,
        Flashcard resource)
    {
        if (requirement.AccessType is FlashcardAccessType.Read)
        {
            if (await flashcardAccessService.CanReadAsync(resource.Id, context.User.GetUserId()))
                context.Succeed(requirement);
        }
        else if(await flashcardAccessService.CanEditAsync(resource.Id, context.User.GetUserId()))
            context.Succeed(requirement);
    }
}