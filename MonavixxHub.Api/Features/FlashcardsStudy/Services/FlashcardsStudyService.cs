using System.Security.Claims;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.Flashcards.Services;
using MonavixxHub.Api.Features.FlashcardsStudy.Algorithms;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.FlashcardsStudy.Services;

public class FlashcardsStudyService(
    IServiceProvider serviceProvider)
{
    public async Task<Flashcard> NextAsync(ClaimsPrincipal User, FlashcardSet flashcardSet,
        FlashcardStudyAlgorithm flashcardStudyAlgorithm)
    {
        return await serviceProvider.GetKeyedService<IFlashcardStudyAlgorithm>(flashcardStudyAlgorithm)!.NextAsync(User,
            flashcardSet);
    }
}