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
        /*await dbContext.FlashcardSets.Where(x=>x.Id==id)
            .ExecuteUpdateAsync(u =>
        {
            u.SetProperty(x=>x.Name, dto.Name);
            u.SetProperty(x=>x.ParentSetId, dto.ParentSetId);
            u.SetProperty(x=>x.IsPublic, dto.IsPublic);
        });*/
        flashcardSet.Name = dto.Name;
        flashcardSet.ParentSetId = dto.ParentSetId;
        flashcardSet.IsPublic = dto.IsPublic;
        await dbContext.SaveChangesAsync();
        return flashcardSet;
    }

    public async ValueTask AddFlashcardAsync(Guid flashcardId, Guid setId, int userId)
    {
        
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