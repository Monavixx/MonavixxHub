using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Exceptions;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Flashcards.Services;

/// <summary>
/// Handles FlashcardSet creation, retrieval, modification and deletion.
/// </summary>
public class FlashcardSetService (AppDbContext dbContext)
{
    /// <summary>
    /// Creates a new flashcard set owned by the specified user.
    /// </summary>
    /// <param name="dto">Data used to create the flashcard set.</param>
    /// <param name="ownerId">ID of the user who will own the flashcard set.</param>
    /// <returns>The newly created flashcard set.</returns>
    /// <exception cref="FlashcardSetNotFoundException">
    /// Thrown if the specified parent flashcard set doesn't exist.
    /// </exception>
    public async ValueTask<FlashcardSet> CreateAsync(CreateFlashcardSetDto dto, int ownerId)
    {
        if(dto.ParentSetId is not null) await VerifyFlashcardSetExists(dto.ParentSetId.Value);
        var flashcardSet = new FlashcardSet
        {
            Name = dto.Name,
            ParentSetId = dto.ParentSetId,
            OwnerId = ownerId,
            IsPublic =  dto.IsPublic
        };
        dbContext.FlashcardSets.Add(flashcardSet);
        await dbContext.SaveChangesAsync();
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
    public async ValueTask<FlashcardSet> UpdateAsync(FlashcardSet flashcardSet, UpdateFlashcardSetDto dto)
    {
        flashcardSet.Name = dto.Name;
        flashcardSet.ParentSetId = dto.ParentSetId;
        flashcardSet.IsPublic = dto.IsPublic;
        await dbContext.SaveChangesAsync();
        return flashcardSet;
    }

    /// <summary>
    /// Finds a flashcard set by the specified ID.
    /// </summary>
    /// <param name="setId">Flashcard set ID to find by.</param>
    /// <returns>The flashcard set with the specified ID.</returns>
    /// <exception cref="FlashcardSetNotFoundException">
    /// Thrown if a flashcard set with the specified ID doesn't exist.
    /// </exception>
    public async ValueTask<FlashcardSet> GetAsync(Guid setId)
        => await dbContext.FlashcardSets.FindAsync(setId) ?? throw new FlashcardSetNotFoundException();

    /// <summary>
    /// Verifies that a flashcard set with the specified ID exist by throwing an exception if it doesn't.
    /// </summary>
    /// <param name="setId">ID of the flashcard set to verify.</param>
    /// <exception cref="FlashcardSetNotFoundException">
    /// Thrown if a flashcard set with the specified ID doesn't exist.
    /// </exception>
    public async ValueTask VerifyFlashcardSetExists(Guid setId)
    {
        if (await dbContext.FlashcardSets.FindAsync(setId) is null) throw new FlashcardSetNotFoundException();
    }
}