using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Services;

namespace MonavixxHub.Api.Features.Flashcards.Controllers;

[ApiController]
[Route("api/flashcard-sets/{flashcardSetId}/entries")]
public class FlashcardSetEntryController
(IAuthorizationService authorizationService) :  ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async ValueTask<IActionResult> AddFlashcard(Guid flashcardSetId,
        [FromServices] FlashcardSetService flashcardSetService,
        [FromBody] AddFlashcardToSetDto dto,
        [FromServices] FlashcardService flashcardService,
        [FromServices] FlashcardSetEntryService flashcardSetEntryService)
    {
        var flashcardSet = await flashcardSetService.GetAsync(flashcardSetId);
        var authorizationResult = 
            await authorizationService.AuthorizeAsync(User, flashcardSet, Requirements.FlashcardSet.EditAccess);
        if (!authorizationResult.Succeeded) return Forbid();
        var flashcard = await flashcardService.GetAsync(dto.FlashcardId);
        authorizationResult = 
            await authorizationService.AuthorizeAsync(User, flashcard, Requirements.Flashcard.EditAccess);
        if (!authorizationResult.Succeeded) return Forbid();

        if (dto.Order is null)
            await flashcardSetEntryService.AddFlashcardToTheEndAsync(flashcardSetId, dto.FlashcardId);
        else
            await flashcardSetEntryService.AddFlashcardAsync(flashcardSetId, dto.FlashcardId, dto.Order.Value);
        return NoContent();
    }
}