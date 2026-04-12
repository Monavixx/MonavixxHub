using System.Security.Claims;

namespace MonavixxHub.Api.Features.Flashcards.Services;

/// <summary>
/// Provides methods to check whether a specific user has access to a flashcard in the database.
/// </summary>
public interface IFlashcardAccessService
{
    /// <summary>
    /// Determines whether the specified user has read access to the given flashcard.
    /// </summary>
    /// <param name="flashcardId">ID of the flashcard to check access for.</param>
    /// <param name="user">User whose access is being checked.</param>
    /// <returns><c>true</c> if the user can read the flashcard; otherwise, <c>false</c>.</returns>
    ValueTask<bool> CanReadAsync(Guid flashcardId, ClaimsPrincipal user);

    /// <summary>
    /// Determines whether the specified user has edit access to the given flashcard.
    /// </summary>
    /// <param name="flashcardId">ID of the flashcard to check access for.</param>
    /// <param name="user">User trying to get access.</param>
    /// <returns><c>true</c> if the user can edit the flashcard; otherwise, <c>false</c>.</returns>
    ValueTask<bool> CanEditAsync(Guid flashcardId, ClaimsPrincipal user);
}

