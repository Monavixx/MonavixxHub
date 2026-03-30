using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.DTOs.Response;
using MonavixxHub.Api.Features.Flashcards.Services;
using MonavixxHub.Api.Features.FlashcardsStudy.Algorithms;
using MonavixxHub.Api.Features.FlashcardsStudy.Services;

namespace MonavixxHub.Api.Features.FlashcardsStudy.Controllers;

[Authorize]
[ApiController]
[Route("api/flashcard-sets/{flashcardSetId:guid}/study")]
public class FlashcardsStudyController(
    IAuthorizationService authorizationService,
    FlashcardSetService flashcardSetService,
    FlashcardsStudyService flashcardsStudyService) : ControllerBase
{
    [HttpGet("next")]
    public async Task<IActionResult> Next(Guid flashcardSetId,
        [FromQuery(Name = "alg")] FlashcardStudyAlgorithm algorithm = FlashcardStudyAlgorithm.Random)
    {
        var flashcardSet = await flashcardSetService.GetAsync(flashcardSetId);
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, flashcardSet, Requirements.FlashcardSet.ReadAccess);
        if (!authorizationResult.Succeeded) return Forbid();
        var flashcard = await flashcardsStudyService.NextAsync(User, flashcardSet, algorithm);
        return Ok(GetFlashcardDto.FromFlashcard(flashcard));
    }
}