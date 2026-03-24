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
            Transcription = dto.Transcription,
            CreatedAt = now,
            UpdatedAt = now,
            ImageId = image?.Id
        };
        dbContext.Flashcards.Add(flashcard);
        await dbContext.SaveChangesAsync();
        return flashcard;
    }

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

    public IQueryable<Flashcard> GetAll(int userId)
    {
        return dbContext.Flashcards.Where(x => x.OwnerId == userId);
    }

    public async ValueTask DeleteAsync(Flashcard flashcard)
    {
        if(flashcard.ImageId is not null)
            await imageService.DecrementRcAndDeleteIfUnusedAsync(flashcard.ImageId.Value);
        dbContext.Remove(flashcard);
        await dbContext.SaveChangesAsync();
    }

    public async ValueTask<bool> IsPublicAsync(Guid id)
    {
        return await dbContext.Flashcards
            .AnyAsync(x => x.Id == id && x.Entries
                .Any(a => a.FlashcardSet.IsPublic));
    }

    public async ValueTask<Flashcard> GetAsync(Guid id)
    {
        return await dbContext.Flashcards.FindAsync(id) ?? throw new FlashcardNotFoundException();
    }
}