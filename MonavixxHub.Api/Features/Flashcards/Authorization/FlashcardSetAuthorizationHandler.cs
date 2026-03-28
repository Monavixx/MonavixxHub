using System.Reflection.Metadata;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;

/// <summary>
/// Handles authorization for <see cref="FlashcardSet"/> resources based on
/// <see cref="FlashcardSetAccessRequirement"/>.
/// </summary>
public class FlashcardSetAuthorizationHandler : AuthorizationHandler<FlashcardSetAccessRequirement, FlashcardSet>
{
    /// <summary>
    /// Evaluates the access requirement for a specific flashcard set.
    /// </summary>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        FlashcardSetAccessRequirement requirement,
        FlashcardSet resource)
    {
        if (resource.IsPublic && requirement.AccessType is FlashcardSetAccessType.Read) context.Succeed(requirement);
        else
        {
            var userId = context.User.GetUserId();
            if (resource.OwnerId == userId)
                context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}