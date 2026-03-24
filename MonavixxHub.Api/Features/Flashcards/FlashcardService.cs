using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using MonavixxHub.Api.Common;
using MonavixxHub.Api.Common.Models;
using MonavixxHub.Api.Features.Auth.Exceptions;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Exceptions;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Flashcards;

public class FlashcardService (IImageService imageService, AppDbContext dbContext)
{
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
            Transcription = dto.Transcription ?? "",
            CreatedAt = now,
            UpdatedAt = now,
            ImageId = image?.Id
        };
        dbContext.Flashcards.Add(flashcard);
        await dbContext.SaveChangesAsync();
        return flashcard;
    }

    public async ValueTask<Flashcard> PatchAsync(Guid id, PatchFlashcardDto dto, int userId)
    {
        Flashcard flashcard = await FindFlashcard(id, userId);

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
        return flashcard;
    }

    public async ValueTask<Flashcard> UpdateAsync(Guid id, UpdateFlashcardDto dto, int userId)
    {
        Flashcard flashcard = await FindFlashcard(id, userId);
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
        return flashcard;
    }

    public IQueryable<Flashcard> GetAll(int userId)
    {
        return dbContext.Flashcards.Where(x => x.OwnerId == userId);
    }

    public async ValueTask DeleteAsync(Guid id, int userId)
    {
        Guid? imageId = null;
        try
        {
            imageId = await dbContext.Flashcards
                .Where(x => x.OwnerId == userId && x.Id == id)
                .Select(x => x.ImageId)
                .SingleAsync();
        }
        catch (InvalidOperationException ex)
        {
            throw new UserDoesNotHaveFlashcardWithSuchId(ex);
        }
        if(imageId is not null)
            await imageService.DecrementRcAndDeleteIfUnusedAsync(imageId.Value);
        await dbContext.Flashcards.Where(x => x.OwnerId == userId && x.Id == id)
            .ExecuteDeleteAsync();
    }

    private async ValueTask<Flashcard> FindFlashcard(Guid id, int userId)
    {
        try
        {
            return await dbContext.Flashcards
                .Where(x => x.OwnerId == userId && x.Id == id)
                .SingleAsync();
        }
        catch (InvalidOperationException ex)
        {
            throw new UserDoesNotHaveFlashcardWithSuchId(ex);
        }
    }
}