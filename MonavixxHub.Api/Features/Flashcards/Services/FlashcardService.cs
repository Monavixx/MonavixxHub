using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.DTOs.Request;
using MonavixxHub.Api.Features.Flashcards.Exceptions;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.Images.Models;
using MonavixxHub.Api.Features.Images.Services;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Flashcards.Services;

/// <summary>
/// Handles flashcard creation, retrieval, and modification.
/// Each flashcard belongs to a specific user and can optionally contain an image.
/// </summary>
public class FlashcardService (IImageService imageService, AppDbContext dbContext, ILogger<FlashcardService> logger,
    IFlashcardSetEntryService flashcardSetEntryService) : IFlashcardService
{
    /// <summary>
    /// Creates a new flashcard for the specified user.
    /// </summary>
    /// <param name="dto">Flashcard data including front, back, and optional image.</param>
    /// <param name="user">User creating the flashcard.</param>
    /// <returns>The created flashcard.</returns>
    public async Task<Flashcard> CreateAsync(CreateFlashcardDto dto, ClaimsPrincipal user)
    {
        var flashcard = await CreateCoreAsync(dto, user);
        await dbContext.SaveChangesAsync();
        logger.LogInformation($"Flashcard [{flashcard.Id}] created successfully");

        return flashcard;
    }

    public async ValueTask<Flashcard> CreateCoreAsync(CreateFlashcardDto dto, ClaimsPrincipal user)
    {
        logger.LogDebug($"Creating Flashcard ({dto.Front} - {dto.Back})");
        Image? image = null;
        if(dto.Image is not null)
        {
            image = await imageService.SaveImageAsync(dto.Image.OpenReadStream(), dto.Image.ContentType);
        }

        var now = DateTimeOffset.UtcNow;
        var flashcard = new Flashcard
        {
            OwnerId = user.GetUserId(),
            Front = dto.Front,
            Back = dto.Back,
            Transcription = dto.Transcription,
            CreatedAt = now,
            UpdatedAt = now,
            ImageId = image?.Id
        };
        dbContext.Flashcards.Add(flashcard);
        if (dto.FlashcardSetId is {} fsi)
        {
            await flashcardSetEntryService.AddFlashcardToTheEndCoreAsync(fsi, flashcard.Id);
        }
        
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
        logger.LogDebug("Patching Flashcard [{FlashcardId}]...", flashcard.Id);
        List<string> patchedFields = new(4);
        if (dto.Image is not null)
        {
            var image = await imageService.SaveImageAsync(dto.Image.OpenReadStream(), dto.Image.ContentType);
            if (flashcard.ImageId is not null)
                await imageService.DecrementRcAndDeleteIfUnusedAsync(flashcard.ImageId.Value);
            flashcard.ImageId = image.Id;
            patchedFields.Add(nameof(Flashcard.ImageId));
        }

        PatchIfNotNull(dto.Back, s => flashcard.Back = s, nameof(Flashcard.Back));
        PatchIfNotNull(dto.Front, s => flashcard.Front = s, nameof(Flashcard.Front));
        PatchIfNotNull(dto.Transcription, s => flashcard.Transcription = s, nameof(Flashcard.Transcription));

        flashcard.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Flashcard [{FlashcardId}] patched successfully with the following fields: {PatchedFields}",
            flashcard.Id, string.Join(", ", patchedFields));
        void PatchIfNotNull<T>(T? value, Action<T> setter, string propertyName)
        {
            if (value is null) return;
            setter(value);
            patchedFields.Add(propertyName);
        }
    }

    

    /// <summary>
    /// Updates the specified flashcard. The changes even include provided null fields.
    /// </summary>
    /// <param name="flashcard">The flashcard entity to update.</param>
    /// <param name="dto">Fields to update.</param>
    public async ValueTask UpdateAsync(Flashcard flashcard, UpdateFlashcardDto dto)
    {
        logger.LogDebug("Updating Flashcard [{FlashcardId}]...", flashcard.Id);
        flashcard.Front = dto.Front;
        flashcard.Back = dto.Back;
        flashcard.Transcription = dto.Transcription;
        
        if (flashcard.ImageId is not null)
            await imageService.DecrementRcAndDeleteIfUnusedAsync(flashcard.ImageId.Value);
        
        if (dto.Image is null) flashcard.ImageId = null;
        else
        {
            var image = await imageService.SaveImageAsync(dto.Image.OpenReadStream(), dto.Image.ContentType);
            flashcard.ImageId = image.Id;
        }
        flashcard.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Flashcard [{FlashcardId}] updated successfully", flashcard.Id);
    }

    /// <summary>
    /// Returns a queryable collection of flashcards owned by the specified user.
    /// </summary>
    /// <param name="user">User whose flashcards to retrieve.</param>
    /// <returns>An IQueryable collection that can be further filtered or projected.</returns>
    public IQueryable<Flashcard> GetAll(UserIdType userId)
    {
        logger.LogDebug("Getting all flashcards owned by user [{SpecifiedUserId}]...", userId);
        return dbContext.Flashcards.Where(x => x.OwnerId == userId);
    }

    public IQueryable<Flashcard> GetPage(UserIdType userId, int page, int limit)
        => GetAll(userId).Skip(page * limit).Take(limit);

    /// <summary>
    /// Deletes the specified flashcard entirely from the database.
    /// If the flashcard has an associated image, its reference count is decremented
    /// and the image is deleted if no longer used by any other flashcard.
    /// </summary>
    /// <param name="flashcard">The flashcard to delete.</param>
    public async ValueTask DeleteAsync(Flashcard flashcard)
    {
        logger.LogDebug("Deleting Flashcard [{FlashcardId}]...", flashcard.Id);
        if(flashcard.ImageId is not null)
            await imageService.DecrementRcAndDeleteIfUnusedAsync(flashcard.ImageId.Value);
        dbContext.Remove(flashcard);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Flashcard [{FlashcardId}] deleted successfully", flashcard.Id);
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
        logger.LogDebug("Getting Flashcard [{FlashcardId}]...", id);
        return await dbContext.Flashcards.FindAsync(id) ?? throw new FlashcardNotFoundException();
    }
}