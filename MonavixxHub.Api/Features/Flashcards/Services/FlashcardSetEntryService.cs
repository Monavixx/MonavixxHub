using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Flashcards.Exceptions;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Flashcards.Services;

/// <summary>
/// Handles flashcard sets' entries' creation, retrieval, updating, deletion
/// </summary>
public class FlashcardSetEntryService (AppDbContext dbContext)
{
    /// <summary>
    /// Adds the specified flashcard to the specified flashcard set at the given position.
    /// </summary>
    /// <param name="flashcardSetId">ID of the flashcard set to add the specified flashcard to</param>
    /// <param name="flashcardId">ID of the flashcard to add.</param>
    /// <param name="order">Position of the flashcard within the set.</param>
    /// <exception cref="FlashcardAlreadyInSetException">
    /// Thrown if the flashcard is already a member of the specified set.
    /// </exception>
    public async ValueTask AddFlashcardAsync(Guid flashcardSetId, Guid flashcardId, int order)
    {
        await ThrowIfIncludesFlashcardAsync(flashcardSetId, flashcardId);
        await AddFlashcardWithoutCheckAsync(flashcardSetId, flashcardId, order);
    }

    /// <summary>
    /// Adds the specified flashcard to the end of the specified flashcard set.
    /// </summary>
    /// <param name="flashcardSetId">ID of the flashcard set to add the specified flashcard to</param>
    /// <param name="flashcardId">ID of the flashcard to add.</param>
    /// <exception cref="FlashcardAlreadyInSetException">
    /// Thrown if the flashcard is already a member of the specified set.
    /// </exception>
    public async ValueTask AddFlashcardToTheEndAsync(Guid flashcardSetId, Guid flashcardId)
    {
        await ThrowIfIncludesFlashcardAsync(flashcardSetId, flashcardId);
        int maxOrder = await dbContext.FlashcardSetEntries.Where(x => x.FlashcardSetId == flashcardSetId)
            .MaxAsync(x => (int?)x.Order) ?? 0;
        await AddFlashcardWithoutCheckAsync(flashcardSetId, flashcardId, maxOrder + 1);
    }

    private async ValueTask AddFlashcardWithoutCheckAsync(Guid flashcardSetId, Guid flashcardId, int order)
    {
        dbContext.FlashcardSetEntries.Add(new FlashcardSetEntry()
        {
            FlashcardId = flashcardId,
            Order = order,
            FlashcardSetId = flashcardSetId
        });
        await dbContext.SaveChangesAsync();
    }

    private async ValueTask ThrowIfIncludesFlashcardAsync(Guid flashcardSetId, Guid flashcardId)
    {
        if (await HasFlashcardAsync(flashcardSetId, flashcardId)) throw new FlashcardAlreadyInSetException();
    }

    /// <summary>
    /// Checks if the specified flashcard is a member of the specified set.
    /// </summary>
    /// <param name="flashcardSetId">ID of the set to check in.</param>
    /// <param name="flashcardId">ID of the flashcard to check.</param>
    /// <returns>true if the specified flashcard is a member of the specified set, false otherwise.</returns>
    public async ValueTask<bool> HasFlashcardAsync(Guid flashcardSetId, Guid flashcardId)
    {
        return await dbContext.FlashcardSetEntries.AnyAsync(x =>
            x.FlashcardId == flashcardId && x.FlashcardSetId == flashcardSetId);
    }
}