using System.Linq.Expressions;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;

/// <summary>
/// Provides reusable LINQ expressions to determine a user's access
/// to flashcards. These expressions can be used in queries
/// without immediately executing them, e.g., with Entity Framework Core.
/// </summary>
public static class FlashcardAccessExpressions
{
    /// <summary>
    /// Returns an expression that determines whether the specified user
    /// can read a given flashcard.
    /// </summary>
    /// <param name="userId">The ID of the user to check access for.</param>
    /// <returns>
    /// An expression that evaluates to
    /// <c>true</c> if the user has read access to the flashcard; otherwise <c>false</c>.
    /// </returns>
    public static Expression<Func<Flashcard, bool>> CanRead(UserIdType userId)
        => f => f.OwnerId == userId || f.Entries.Any(e => e.FlashcardSet.IsPublic);

    /// <summary>
    /// Returns an expression that determines whether the specified user
    /// can edit a given flashcard.
    /// </summary>
    /// <param name="userId">The ID of the user to check access for.</param>
    /// <returns>
    /// An expression that evaluates to
    /// <c>true</c> if the user has edit access to the flashcard; otherwise <c>false</c>.
    /// </returns>
    public static Expression<Func<Flashcard, bool>> CanEdit(UserIdType userId)
        => f => f.OwnerId == userId;
}