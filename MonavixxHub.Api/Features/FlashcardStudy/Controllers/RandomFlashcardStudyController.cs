using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.DTOs.Response;
using MonavixxHub.Api.Features.Flashcards.Services;
using MonavixxHub.Api.Features.FlashcardStudy.Services;

namespace MonavixxHub.Api.Features.FlashcardStudy.Controllers;

[Authorize]
[ApiController]
[Route("api/flashcard-sets/{flashcardSetId:guid}/study")]
public class RandomFlashcardStudyController(
    IAuthorizationService authorizationService,
    IFlashcardSetService flashcardSetService,
    RandomFlashcardStudyAlgorithmService randomFlashcardStudyAlgorithmService) : ControllerBase
{
    [HttpGet("random/next")]
    public async Task<IActionResult> RandomNext(Guid flashcardSetId)
    {
        var flashcardSet = await flashcardSetService.GetAsync(flashcardSetId);
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, flashcardSet, Requirements.FlashcardSet.ReadAccess);
        if (!authorizationResult.Succeeded) return Forbid();
        var flashcard = await randomFlashcardStudyAlgorithmService.NextAsync(User, flashcardSet);
        return Ok(flashcard is null ? null : GetFlashcardDto.FromFlashcard(flashcard));
    }

    [HttpGet("random/session")]
    public async Task<IActionResult> GetSession(Guid flashcardSetId)
    {
        var flashcardSet = await flashcardSetService.GetAsync(flashcardSetId);
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, flashcardSet, Requirements.FlashcardSet.ReadAccess);
        if (!authorizationResult.Succeeded) return Forbid();
        var session = await randomFlashcardStudyAlgorithmService.GetSessionDto(User.GetUserId(), flashcardSet);
        return Ok(session);
    }
}