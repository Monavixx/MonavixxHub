using Microsoft.AspNetCore.Authorization;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.Flashcards.Services;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;


/// <summary>
/// Handles authorization for <see cref="Flashcard"/> resources based on
/// <see cref="FlashcardAccessRequirement"/>.
/// </summary>
/// <remarks>
/// Uses <see cref="FlashcardAccessService"/> to determine whether the
/// current user has read or edit access to a specific flashcard.
/// </remarks>
public class FlashcardAuthorizationHandler (FlashcardAccessService flashcardAccessService)
    : AuthorizationHandler<FlashcardAccessRequirement, Flashcard>
{
    /// <summary>
    /// Handles the authorization requirement for a specific flashcard.
    /// </summary>
    /// <param name="context">The authorization context containing the user and claims.</param>
    /// <param name="requirement">The access requirement to evaluate (Read or Edit).</param>
    /// <param name="resource">The flashcard resource to check access against.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// If the user meets the requirement (read or edit access), 
    /// <see cref="AuthorizationHandlerContext.Succeed(IAuthorizationRequirement)"/>
    /// is called to indicate successful authorization.
    /// </remarks>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, FlashcardAccessRequirement requirement,
        Flashcard resource)
    {
        if (requirement.AccessType is FlashcardAccessType.Read)
        {
            if (await flashcardAccessService.CanReadAsync(resource.Id, context.User))
                context.Succeed(requirement);
        }
        else if(await flashcardAccessService.CanEditAsync(resource.Id, context.User))
            context.Succeed(requirement);
    }
}