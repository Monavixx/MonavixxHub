using System.Security.Claims;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.DTOs.Request;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.Services;

/// <summary>
/// Handles flashcard creation, retrieval, and modification.
/// Each flashcard belongs to a specific user and can optionally contain an image.
/// </summary>
public interface IFlashcardService
{
    /// <summary>
    /// Creates a new flashcard for the specified user.
    /// </summary>
    /// <param name="dto">Flashcard data including front, back, and optional image.</param>
    /// <param name="user">User creating the flashcard.</param>
    /// <returns>The created flashcard.</returns>
    Task<Flashcard> CreateAsync(CreateFlashcardDto dto, ClaimsPrincipal user);

    /// <summary>
    /// Internal method to create a flashcard without saving to database.
    /// </summary>
    /// <param name="dto">Flashcard data.</param>
    /// <param name="user">User creating the flashcard.</param>
    /// <returns>The flashcard entity (not yet saved).</returns>
    ValueTask<Flashcard> CreateCoreAsync(CreateFlashcardDto dto, ClaimsPrincipal user);

    /// <summary>
    /// Patches the specified flashcard.
    /// Only provided fields are updated - null fields are ignored.
    /// </summary>
    /// <param name="flashcard">The flashcard entity to patch.</param>
    /// <param name="dto">Fields to update (null values are skipped).</param>
    ValueTask PatchAsync(Flashcard flashcard, PatchFlashcardDto dto);

    /// <summary>
    /// Updates the specified flashcard. The changes include provided null fields.
    /// </summary>
    /// <param name="flashcard">The flashcard entity to update.</param>
    /// <param name="dto">Fields to update.</param>
    ValueTask UpdateAsync(Flashcard flashcard, UpdateFlashcardDto dto);

    /// <summary>
    /// Returns a queryable collection of flashcards owned by the specified user.
    /// </summary>
    /// <param name="userId">User whose flashcards to retrieve.</param>
    /// <returns>An IQueryable collection that can be further filtered or projected.</returns>
    IQueryable<Flashcard> GetAll(UserIdType userId);

    /// <summary>
    /// Gets a page of flashcards owned by the specified user.
    /// </summary>
    /// <param name="userId">User whose flashcards to retrieve.</param>
    /// <param name="page">Zero-based page number.</param>
    /// <param name="limit">Number of flashcards per page.</param>
    /// <returns>An IQueryable collection for the specified page.</returns>
    IQueryable<Flashcard> GetPage(UserIdType userId, int page, int limit);

    /// <summary>
    /// Deletes the specified flashcard entirely from the database.
    /// If the flashcard has an associated image, its reference count is decremented
    /// and the image is deleted if no longer used by any other flashcard.
    /// </summary>
    /// <param name="flashcard">The flashcard to delete.</param>
    ValueTask DeleteAsync(Flashcard flashcard);

    /// <summary>
    /// Checks if the flashcard with the specified ID belongs to any public flashcard set.
    /// </summary>
    /// <remarks>If a flashcard with the specified ID doesn't exist, the method returns false
    /// instead of throwing an exception.</remarks>
    /// <param name="id">Flashcard ID to check.</param>
    /// <returns>Returns true if there is at least one public flashcard set that contains the specified flashcard.</returns>
    ValueTask<bool> IsPublicAsync(Guid id);

    /// <summary>
    /// Gets a flashcard entity with the specified id.
    /// </summary>
    /// <param name="id">Flashcard ID to get.</param>
    /// <returns>Not null flashcard with the specified ID.</returns>
    /// <exception cref="FlashcardNotFoundException">Thrown if there is no flashcard with the specified ID.</exception>
    ValueTask<Flashcard> GetAsync(Guid id);
}

