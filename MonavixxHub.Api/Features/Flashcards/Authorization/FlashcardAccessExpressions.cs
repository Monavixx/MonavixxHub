using System.Linq.Expressions;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;

public static class FlashcardAccessExpressions
{
    public static Expression<Func<Flashcard, bool>> CanRead(int userId)
        => f => f.OwnerId == userId || f.Entries.Any(e => e.FlashcardSet.IsPublic);

    public static Expression<Func<Flashcard, bool>> CanEdit(int userId)
        => f => f.OwnerId == userId;
}