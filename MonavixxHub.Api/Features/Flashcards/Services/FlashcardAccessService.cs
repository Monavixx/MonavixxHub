using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Flashcards.Services;

/// <summary>
/// Provides methods to check whether a specific user has access to a flashcard in the database.
/// Uses <see cref="FlashcardAccessExpressions"/>.
/// </summary>
public class FlashcardAccessService (AppDbContext dbContext, ILogger<FlashcardAccessService> logger)
{
    /// <summary>
    /// Determines whether the specified user has read access to the given flashcard.
    /// </summary>
    /// <param name="flashcardId">ID of the flashcard to check access for.</param>
    /// <param name="user">User whose access is being checked.</param>
    /// <returns><c>true</c> if the user can read the flashcard; otherwise, <c>false</c>.</returns>
    public async ValueTask<bool> CanReadAsync(Guid flashcardId, ClaimsPrincipal user)
    {
        logger.LogDebug("Checking user's read access to flashcard ({FlashcardId})...", flashcardId);
        var userId = user.GetUserId();
        var hasAccess = await dbContext.Flashcards
            .Where(x => x.Id == flashcardId)
            .AnyAsync(FlashcardAccessExpressions.CanRead(userId));
        logger.LogDebug("Read access to flashcard ({FlashcardId}) is {Access}",
            flashcardId, hasAccess ? "allowed" : "denied");
        return hasAccess;
    }
    /// <summary>
    /// Determines whether the specified user has edit access to the given flashcard.
    /// </summary>
    /// <param name="flashcardId">ID of the flashcard to check access for.</param>
    /// <param name="user">User trying to get access.</param>
    /// <returns><c>true</c> if the user can edit the flashcard; otherwise, <c>false</c>.</returns>
    public async ValueTask<bool> CanEditAsync(Guid flashcardId, ClaimsPrincipal user)
    {
        logger.LogDebug("Checking user's edit access to flashcard ({FlashcardId})...", flashcardId);
        var userId = user.GetUserId();
        var hasAccess = await dbContext.Flashcards
            .Where(x => x.Id == flashcardId)
            .AnyAsync(FlashcardAccessExpressions.CanEdit(userId));
        logger.LogDebug("Edit access to flashcard ({FlashcardId}) is {Access}",
            flashcardId, hasAccess ? "allowed" : "denied");
        return hasAccess;
    }
}