using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Exceptions;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Flashcards;

public class FlashcardSetService (AppDbContext dbContext)
{
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

    public async ValueTask<FlashcardSet> UpdateAsync(FlashcardSet flashcardSet, UpdateFlashcardSetDto dto)
    {
        flashcardSet.Name = dto.Name;
        flashcardSet.ParentSetId = dto.ParentSetId;
        flashcardSet.IsPublic = dto.IsPublic;
        await dbContext.SaveChangesAsync();
        return flashcardSet;
    }

    public async ValueTask AddFlashcardAsync(Guid flashcardSetId, Guid flashcardId, int order)
    {
        await ThrowIfIncludesFlashcardAsync(flashcardSetId, flashcardId);
        await AddFlashcardWithoutCheckAsync(flashcardSetId, flashcardId, order);
    }

    public async ValueTask AddFlashcardToTheEndAsync(Guid flashcardSetId, Guid flashcardId)
    {
        await ThrowIfIncludesFlashcardAsync(flashcardSetId, flashcardId);
        int maxOrder = await dbContext.FlashcardSetEntries.Where(x => x.FlashcardSetId == flashcardSetId)
            .MaxAsync(x => (int?)x.Order) ?? 0;
        await AddFlashcardWithoutCheckAsync(flashcardSetId, flashcardId, maxOrder + 1);
    }

    private async ValueTask AddFlashcardWithoutCheckAsync(Guid flashcardSetId, Guid flashcardId, int order)
    {
        dbContext.FlashcardSetEntries.Add(new FlashcardSetEntry()
        {
            FlashcardId = flashcardId,
            Order = order,
            FlashcardSetId = flashcardSetId
        });
        await dbContext.SaveChangesAsync();
    }

    private async ValueTask ThrowIfIncludesFlashcardAsync(Guid flashcardSetId, Guid flashcardId)
    {
        if (await HasFlashcardAsync(flashcardSetId, flashcardId)) throw new FlashcardAlreadyInSetException();
    }

    public async ValueTask<bool> HasFlashcardAsync(Guid flashcardSetId, Guid flashcardId)
    {
        return await dbContext.FlashcardSetEntries.AnyAsync(x =>
            x.FlashcardId == flashcardId && x.FlashcardSetId == flashcardSetId);
    }

    public async ValueTask<FlashcardSet> GetAsync(Guid setId)
    {
        var flashcardSet = await dbContext.FlashcardSets.FindAsync(setId);
        if (flashcardSet is null) throw new FlashcardSetNotFoundException();
        return flashcardSet;
    }

    private async ValueTask VerifyFlashcardSetExists(Guid parentSetId)
    {
        if (await dbContext.FlashcardSets.FindAsync(parentSetId) is null) throw new FlashcardSetNotFoundException();
    }
}