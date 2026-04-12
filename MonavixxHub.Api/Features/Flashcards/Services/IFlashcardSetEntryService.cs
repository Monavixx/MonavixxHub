using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.Services;

/// <summary>
/// Handles flashcard sets' entries' creation, retrieval, updating, deletion
/// </summary>
public interface IFlashcardSetEntryService
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
    Task AddFlashcardAsync(Guid flashcardSetId, Guid flashcardId, int order);

    /// <summary>
    /// Adds the specified flashcard to the end of the specified flashcard set.
    /// </summary>
    /// <param name="flashcardSetId">ID of the flashcard set to add the specified flashcard to</param>
    /// <param name="flashcardId">ID of the flashcard to add.</param>
    /// <exception cref="FlashcardAlreadyInSetException">
    /// Thrown if the flashcard is already a member of the specified set.
    /// </exception>
    Task AddFlashcardToTheEndAsync(Guid flashcardSetId, Guid flashcardId);

    /// <summary>
    /// Internal method to add flashcard to set at specific order without saving.
    /// </summary>
    /// <param name="flashcardSetId">ID of the flashcard set.</param>
    /// <param name="flashcardId">ID of the flashcard.</param>
    /// <param name="order">Position within the set.</param>
    void AddFlashcardCoreAsync(Guid flashcardSetId, Guid flashcardId, int order);

    /// <summary>
    /// Internal method to add flashcard to end of set without saving.
    /// </summary>
    /// <param name="flashcardSetId">ID of the flashcard set.</param>
    /// <param name="flashcardId">ID of the flashcard.</param>
    Task AddFlashcardToTheEndCoreAsync(Guid flashcardSetId, Guid flashcardId);

    /// <summary>
    /// Gets a page of flashcards in the specified set, ordered by their position.
    /// </summary>
    /// <param name="flashcardSetId">ID of the flashcard set.</param>
    /// <param name="page">Zero-based page number.</param>
    /// <param name="limit">Number of flashcards per page.</param>
    /// <returns>Queryable collection of flashcards in the set for the specified page.</returns>
    IQueryable<Flashcard> GetFlashcardsInSet(Guid flashcardSetId, int page, int limit);
}

