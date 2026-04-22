using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Flashcards.DTOs.Request;
using MonavixxHub.Api.Features.Flashcards.Exceptions;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.Services;

/// <summary>
/// Handles FlashcardSet creation, retrieval, modification and deletion.
/// </summary>
public interface IFlashcardSetService
{
    /// <summary>
    /// Creates a new flashcard set owned by the specified user.
    /// </summary>
    /// <param name="dto">Data used to create the flashcard set.</param>
    /// <param name="owner">User who will own the flashcard set.</param>
    /// <returns>The newly created flashcard set.</returns>
    /// <exception cref="DbUpdateException">Thrown if the specified parent flashcard set doesn't exist.</exception>
    ValueTask<FlashcardSet> CreateAsync(CreateFlashcardSetDto dto, ClaimsPrincipal owner);

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
    Task<FlashcardSet> UpdateAsync(FlashcardSet flashcardSet, UpdateFlashcardSetDto dto);

    /// <summary>
    /// Deletes the specified flashcard set.
    /// </summary>
    /// <param name="flashcardSet">The flashcard set to delete.</param>
    Task DeleteAsync(FlashcardSet flashcardSet);

    /// <summary>
    /// Finds a flashcard set by the specified ID.
    /// </summary>
    /// <param name="setId">Flashcard set ID to find by.</param>
    /// <returns>The flashcard set with the specified ID.</returns>
    /// <exception cref="FlashcardSetNotFoundException">Thrown if a flashcard set with the specified ID doesn't exist.</exception>
    Task<FlashcardSet> GetAsync(Guid setId, bool includeEntries = false, bool thenIncludeFlashcard = false,
        bool includeSubsets = false);

    Task<(FlashcardSet, IList<TFlashcardDto>)> GetWithEntriesPageAsync<TFlashcardDto>(Guid setId, int page, int limit,
        Expression<Func<Flashcard, TFlashcardDto>> flashcardProjection,
        bool includeSubsets = false);

    /// <summary>
    /// Gets all flashcard sets owned by the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <returns>Queryable collection of owned flashcard sets.</returns>
    IQueryable<FlashcardSet> GetUsersSets(UserIdType userId);

    /// <summary>
    /// Gets all flashcard sets that the user is learning from.
    /// </summary>
    /// <param name="userId">The learner's user ID.</param>
    /// <returns>Queryable collection of flashcard sets the user is learning.</returns>
    IQueryable<FlashcardSet> GetLearningSets(UserIdType userId);

    /// <summary>
    /// Ensures that the entries collection is loaded, sorted, and each entry's flashcard is loaded.
    /// </summary>
    /// <param name="flashcardSet">The flashcard set to process.</param>
    Task EnsureEntriesLoadedAndOrderedWithFlashcardAsync(FlashcardSet flashcardSet);

    /// <summary>
    /// Ensures that the entries collection and their flashcards are loaded.
    /// </summary>
    /// <param name="flashcardSet">The flashcard set to process.</param>
    Task EnsureEntriesLoadedWithFlashcardAsync(FlashcardSet flashcardSet);

    /// <summary>
    /// Ensures that the entries collection is loaded.
    /// </summary>
    /// <param name="flashcardSet">The flashcard set to process.</param>
    ValueTask EnsureEntriesLoadedAsync(FlashcardSet flashcardSet);

    /// <summary>
    /// Ensures that the subsets collection is loaded.
    /// </summary>
    /// <param name="flashcardSet">The flashcard set to process.</param>
    ValueTask EnsureSubsetsLoadedAsync(FlashcardSet flashcardSet);
}

