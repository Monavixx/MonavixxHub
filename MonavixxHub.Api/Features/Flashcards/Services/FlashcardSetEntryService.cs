using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Flashcards.Exceptions;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Flashcards.Services;

/// <summary>
/// Handles flashcard sets' entries' creation, retrieval, updating, deletion
/// </summary>
public class FlashcardSetEntryService (AppDbContext dbContext, ILogger<FlashcardSetEntryService> logger)
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
    public async Task AddFlashcardAsync(Guid flashcardSetId, Guid flashcardId, int order)
    {
        await AddFlashcardCoreAsync(flashcardSetId, flashcardId, order);
        
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Flashcard [{FlashcardId}] added to FlashcardSet [{FlashcardSetId}] with order [{Order}]",
            flashcardId, flashcardSetId, order);
    }

    /// <summary>
    /// Adds the specified flashcard to the end of the specified flashcard set.
    /// </summary>
    /// <param name="flashcardSetId">ID of the flashcard set to add the specified flashcard to</param>
    /// <param name="flashcardId">ID of the flashcard to add.</param>
    /// <exception cref="FlashcardAlreadyInSetException">
    /// Thrown if the flashcard is already a member of the specified set.
    /// </exception>
    public async Task AddFlashcardToTheEndAsync(Guid flashcardSetId, Guid flashcardId)
    {
        await AddFlashcardToTheEndCoreAsync(flashcardSetId, flashcardId);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Flashcard [{FlashcardId}] added to FlashcardSet [{FlashcardSetId}]",
            flashcardId, flashcardSetId);
    }
    //todo: admin functionality
    public async ValueTask AddFlashcardCoreAsync(Guid flashcardSetId, Guid flashcardId, int order)
    {
        logger.LogDebug("Adding Flashcard [{FlashcardId}] to FlashcardSet [{FlashcardSetId}]", 
            flashcardId, flashcardSetId);
        
        dbContext.FlashcardSetEntries.Add(new FlashcardSetEntry()
        {
            FlashcardId = flashcardId,
            Order = order,
            FlashcardSetId = flashcardSetId
        });
    }

    public async Task AddFlashcardToTheEndCoreAsync(Guid flashcardSetId, Guid flashcardId)
    {
        int maxOrder = await dbContext.FlashcardSetEntries.Where(x => x.FlashcardSetId == flashcardSetId)
            .MaxAsync(x => (int?)x.Order) ?? 0;
        await AddFlashcardCoreAsync(flashcardSetId, flashcardId, maxOrder + 1);
    }

    public IQueryable<Flashcard> GetFlashcardsInSet(Guid flashcardSetId, int page, int limit)
    {
        return dbContext.FlashcardSetEntries
            .OrderBy(fse => fse.Order)
            .Skip(page*limit)
            .Take(limit)
            .Where(fse => fse.FlashcardSetId == flashcardSetId)
            .Select(fse => fse.Flashcard);
    }
}