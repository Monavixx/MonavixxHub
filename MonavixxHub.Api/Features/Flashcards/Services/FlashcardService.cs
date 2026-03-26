using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Exceptions;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.Images;
using MonavixxHub.Api.Features.Images.Models;
using MonavixxHub.Api.Features.Images.Services;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Flashcards.Services;

/// <summary>
/// Handles flashcard creation, retrieval, and modification.
/// Each flashcard belongs to a specific user and can optionally contain an image.
/// </summary>
public class FlashcardService (IImageService imageService, AppDbContext dbContext)
{
    /// <summary>
    /// Creates a new flashcard for the specified user.
    /// </summary>
    /// <param name="dto">Flashcard data including front, back, and optional image.</param>
    /// <param name="userId">ID of the user creating the flashcard.</param>
    /// <returns>The created flashcard.</returns>
    public async ValueTask<Flashcard> CreateAsync(CreateFlashcardDto dto, int userId)
    {
        Image? image = null;
        if(dto.Image is not null)
        {
            image = await imageService.SaveImageAsync(dto.Image);
        }

        var now = DateTimeOffset.UtcNow;
        var flashcard = new Flashcard
        {
            OwnerId = userId,
            Front = dto.Front,
            Back = dto.Back,
            Transcription = dto.Transcription,
            CreatedAt = now,
            UpdatedAt = now,
            ImageId = image?.Id
        };
        dbContext.Flashcards.Add(flashcard);
        await dbContext.SaveChangesAsync();
        return flashcard;
    }

    /// <summary>
    /// Patches the specified flashcard.
    /// Only provided fields are updated - null fields are ignored.
    /// </summary>
    /// <param name="flashcard">The flashcard entity to patch.</param>
    /// <param name="dto">Fields to update (null values are skipped).</param>
    public async ValueTask PatchAsync(Flashcard flashcard, PatchFlashcardDto dto)
    {
        if (dto.Image is not null)
        {
            if (flashcard.ImageId is not null)
                await imageService.DecrementRcAndDeleteIfUnusedAsync(flashcard.ImageId.Value);
            var image = await imageService.SaveImageAsync(dto.Image);
            flashcard.ImageId = image.Id;
        }
        if(dto.Back is not null)
            flashcard.Back = dto.Back;
        if(dto.Transcription is not null)
            flashcard.Transcription = dto.Transcription;
        if(dto.Front is not null)
            flashcard.Front = dto.Front;
        flashcard.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates the specified flashcard. The changes even include provided null fields.
    /// </summary>
    /// <param name="flashcard">The flashcard entity to update.</param>
    /// <param name="dto">Fields to update.</param>
    public async ValueTask UpdateAsync(Flashcard flashcard, UpdateFlashcardDto dto)
    {
        flashcard.Front = dto.Front;
        flashcard.Back = dto.Back;
        flashcard.Transcription = dto.Transcription;
        if (dto.Image is null) flashcard.ImageId = null;
        else
        {
            var image = await imageService.SaveImageAsync(dto.Image);
            flashcard.ImageId = image.Id;
        }
        flashcard.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Returns a queryable collection of flashcards owned by the specified user.
    /// </summary>
    /// <param name="userId">ID of the user whose flashcards to retrieve.</param>
    /// <returns>An IQueryable collection that can be further filtered or projected.</returns>
    public IQueryable<Flashcard> GetAll(int userId)
    {
        return dbContext.Flashcards.Where(x => x.OwnerId == userId);
    }

    /// <summary>
    /// Deletes the specified flashcard entirely from the database.
    /// If the flashcard has an associated image, its reference count is decremented
    /// and the image is deleted if no longer used by any other flashcard.
    /// </summary>
    /// <param name="flashcard">The flashcard to delete.</param>
    public async ValueTask DeleteAsync(Flashcard flashcard)
    {
        if(flashcard.ImageId is not null)
            await imageService.DecrementRcAndDeleteIfUnusedAsync(flashcard.ImageId.Value);
        dbContext.Remove(flashcard);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if the flashcard with the specified ID belongs to any public flashcardSet.
    /// </summary>
    /// <remarks>If a flashcard with the specified ID doesn't exist, the method returns false
    /// instead of throwing an exception.</remarks>
    /// <param name="id">Flashcard ID to check.</param>
    /// <returns>Returns true if there is at least one public flashcardSet that contains the specified flashcard.</returns>
    public async ValueTask<bool> IsPublicAsync(Guid id)
    {
        return await dbContext.Flashcards
            .AnyAsync(x => x.Id == id && x.Entries
                .Any(a => a.FlashcardSet.IsPublic));
    }

    /// <summary>
    /// Gets a flashcard entity with the specified id.
    /// </summary>
    /// <param name="id">Flashcard ID to get.</param>
    /// <returns>Not null flashcard with the specified ID.</returns>
    /// <exception cref="FlashcardNotFoundException">Thrown if there is no flashcard with the specified ID.</exception>
    public async ValueTask<Flashcard> GetAsync(Guid id)
    {
        return await dbContext.Flashcards.FindAsync(id) ?? throw new FlashcardNotFoundException();
    }
}