using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Flashcards.Services;

public class FlashcardAccessService (AppDbContext dbContext)
{
    public async ValueTask<bool> CanReadAsync(Guid flashcardId, int userId)
    {
        return await dbContext.Flashcards
            .Where(x => x.Id == flashcardId)
            .AnyAsync(FlashcardAccessExpressions.CanRead(userId));
    }

    public async ValueTask<bool> CanEditAsync(Guid flashcardId, int userId)
    {
        return await dbContext.Flashcards
            .Where(x => x.Id == flashcardId)
            .AnyAsync(FlashcardAccessExpressions.CanEdit(userId));
    }
}