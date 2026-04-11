using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.DTOs.Request;
using MonavixxHub.Api.Features.Flashcards.Exceptions;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Flashcards.Services;

/// <summary>
/// Handles FlashcardSet creation, retrieval, modification and deletion.
/// </summary>
public class FlashcardSetService (AppDbContext dbContext, ILogger<FlashcardSetService> logger)
{
    /// <summary>
    /// Creates a new flashcard set owned by the specified user.
    /// </summary>
    /// <param name="dto">Data used to create the flashcard set.</param>
    /// <param name="owner">User who will own the flashcard set.</param>
    /// <returns>The newly created flashcard set.</returns>
    /// <exception cref="DbUpdateException">
    /// Thrown if the specified parent flashcard set doesn't exist.
    /// </exception>
    public async ValueTask<FlashcardSet> CreateAsync(CreateFlashcardSetDto dto, ClaimsPrincipal owner)
    {
        logger.LogDebug("Creating FlashcardSet '{FlashcardSetName}'", dto.Name);
        var flashcardSet = new FlashcardSet
        {
            Name = dto.Name,
            ParentSetId = dto.ParentSetId,
            OwnerId = owner.GetUserId(),
            IsPublic = dto.IsPublic,
            Learners = { new FlashcardSetUser() {UserId = owner.GetUserId()} },
        };
        
        dbContext.FlashcardSets.Add(flashcardSet);
        
        await dbContext.SaveChangesAsync();
        logger.LogInformation("FlashcardSet [{FlashcardSetId}] {FlashcardSetName} created successfully",
            flashcardSet.Id, flashcardSet.Name);
        return flashcardSet;
    }

    /// <summary>
    /// Updates the specified flashcard set with the provided values.
    /// </summary>
    /// <param name="flashcardSet">The flashcard set to update.</param>
    /// <param name="dto">New values to apply to the flashcard set.</param>
    /// <returns>The updated flashcard set instance.</returns>
    /// <remarks>
    /// All fields in <paramref name="dto"/> are applied unconditionally.
    /// Unlike patch-style updates, null values will overwrite existing data.
    /// </remarks>
    public async Task<FlashcardSet> UpdateAsync(FlashcardSet flashcardSet, UpdateFlashcardSetDto dto)
    {
        logger.LogDebug("Updating FlashcardSet [{FlashcardSetId}]", flashcardSet.Id);
        flashcardSet.Name = dto.Name;
        flashcardSet.ParentSetId = dto.ParentSetId;
        flashcardSet.IsPublic = dto.IsPublic;
        await dbContext.SaveChangesAsync();
        logger.LogInformation("FlashcardSet [{FlashcardSetId}] updated successfully", flashcardSet.Id);
        return flashcardSet;
    }

    public async Task DeleteAsync(FlashcardSet flashcardSet)
    {
        logger.LogDebug("Deleting FlashcardSet [{FlashcardSetId}]...", flashcardSet.Id);
        await dbContext.FlashcardSets
            .Where(f => f.ParentSetId == flashcardSet.Id)
            .ExecuteUpdateAsync(builder => builder
                .SetProperty(f => f.ParentSetId, flashcardSet.ParentSetId));
        dbContext.FlashcardSets.Remove(flashcardSet);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("FlashcardSet [{FlashcardSetId}] deleted successfully", flashcardSet.Id);
    }

    /// <summary>
    /// Finds a flashcard set by the specified ID.
    /// </summary>
    /// <param name="setId">Flashcard set ID to find by.</param>
    /// <returns>The flashcard set with the specified ID.</returns>
    /// <exception cref="FlashcardSetNotFoundException">
    /// Thrown if a flashcard set with the specified ID doesn't exist.
    /// </exception>
    public async Task<FlashcardSet> GetAsync(Guid setId)
    {
        logger.LogDebug("Getting FlashcardSet [{FlashcardSetId}]...", setId);
        return await dbContext.FlashcardSets.FindAsync(setId) ?? throw new FlashcardSetNotFoundException();
    }

    public IQueryable<FlashcardSet> GetUsersSets(UserIdType userId)
        => dbContext.FlashcardSets.Where(fs => fs.OwnerId == userId);

    public IQueryable<FlashcardSet> GetLearningSets(UserIdType userId)
        => dbContext.FlashcardSetUsers.Where(fsu => fsu.UserId == userId).Select(fsu => fsu.FlashcardSet);

    /// <summary>
    /// Ensures that the <see cref="FlashcardSet.Entries"/> collection is loaded,
    /// that each entry's <see cref="FlashcardSetEntry.Flashcard"/> is loaded,
    /// and that the entries are sorted by <see cref="FlashcardSetEntry.Order"/>.
    /// If everything is already loaded, the method only sorts the collection in memory.
    /// </summary>
    /// <param name="flashcardSet">The <see cref="FlashcardSet"/> to process. Must be attached to the current <see cref="DbContext"/>.</param>
    public async Task EnsureEntriesLoadedAndOrderedWithFlashcardAsync(FlashcardSet flashcardSet)
    {
        var entriesCollection = dbContext.Entry(flashcardSet)
            .Collection(f => f.Entries);
        if (entriesCollection.IsLoaded)
        {
            bool allLoaded = flashcardSet.Entries
                .All(e => dbContext
                    .Entry(e)
                    .Reference(x => x.Flashcard)
                    .IsLoaded);
            if (allLoaded)
            {
                flashcardSet.Entries.Sort((a,b)=>a.Order.CompareTo(b.Order));
                return;
            }
        }
        await entriesCollection
            .Query()
            .OrderBy(e=>e.Order)
            .Include(e => e.Flashcard)
            .LoadAsync();
    }
    public async Task EnsureEntriesLoadedWithFlashcardAsync(FlashcardSet flashcardSet)
    {
        var entriesCollection = dbContext.Entry(flashcardSet)
            .Collection(f => f.Entries);
        if (entriesCollection.IsLoaded)
        {
            bool allLoaded = flashcardSet.Entries
                .All(e => dbContext
                    .Entry(e)
                    .Reference(x => x.Flashcard)
                    .IsLoaded);
            if (allLoaded)
                return;
        }
        await entriesCollection
            .Query()
            .Include(e => e.Flashcard)
            .LoadAsync();
    }
    public async ValueTask EnsureEntriesLoadedAsync(FlashcardSet flashcardSet)
    {
        var entriesCollection = dbContext.Entry(flashcardSet)
            .Collection(f => f.Entries);
        if (entriesCollection.IsLoaded) return;
        await entriesCollection
            .Query()
            .Include(e => e.Flashcard)
            .LoadAsync();
    }

    public async ValueTask EnsureSubsetsLoadedAsync(FlashcardSet flashcardSet)
    {
        var subsetsCollection = dbContext.Entry(flashcardSet).Collection(f => f.Subsets);
        if (subsetsCollection.IsLoaded) return;
        await subsetsCollection.LoadAsync();
    }
}