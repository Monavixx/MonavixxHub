using System.Security.Claims;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.Flashcards.Services;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.FlashcardsStudy.Algorithms;

[ApiFlashcardStudyAlgorithm(FlashcardStudyAlgorithm.Random)]
public class RandomFlashcardStudyAlgorithm (AppDbContext dbContext,
    FlashcardSetService flashcardSetService,
    FlashcardService flashcardService) : IFlashcardStudyAlgorithm
{
    public async Task<Flashcard> NextAsync(ClaimsPrincipal User, FlashcardSet flashcardSet)
    {
        await flashcardSetService.EnsureEntriesLoadedAsync(flashcardSet);
        
        var fse = flashcardSet.Entries[Random.Shared.Next(flashcardSet.Entries.Count)];
        return await flashcardService.GetAsync(fse.FlashcardId);
    }
}