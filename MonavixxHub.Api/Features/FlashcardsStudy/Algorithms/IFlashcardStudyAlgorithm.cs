using System.Security.Claims;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.FlashcardsStudy.Algorithms;

public interface IFlashcardStudyAlgorithm
{
    Task<Flashcard> NextAsync(ClaimsPrincipal User, FlashcardSet flashcardSet);
}