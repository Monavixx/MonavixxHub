using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Common;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.Authorization.Filters;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.DTOs.Request;
using MonavixxHub.Api.Features.Flashcards.DTOs.Response;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.Flashcards.Services;

namespace MonavixxHub.Api.Features.Flashcards.Controllers;

/// <summary>
/// Provides endpoints to manage flashcards within a flashcard set.
/// </summary>
/// <remarks>
/// All endpoints require authentication. Users must have edit access to the flashcard set 
/// and to the individual flashcards they are adding.
/// </remarks>
[Authorize]
[ApiController]
[Route("api/flashcard-sets/{flashcardSetId:guid}/entries")]
[Authorize(Policy = Policies.EmailConfirmed)]
public class FlashcardSetEntryController(
    IFlashcardSetEntryService flashcardSetEntryService) : ControllerBase
{
    /// <summary>
    /// Adds an existing flashcard to a specific flashcard set.
    /// </summary>
    /// <param name="flashcardSetId">The ID of the flashcard set to which the flashcard will be added.</param>
    /// <param name="dto">Data containing the ID of the flashcard and optionally the order position.</param>
    /// <param name="flashcardSetService">Service to access flashcard sets.</param>
    /// <param name="flashcardService">Service to access flashcards.</param>
    /// <returns>
    /// - 204 No Content if the flashcard was successfully added.
    /// - 403 Forbidden if the user is not authorized to modify the set or flashcard.
    /// - 404 Not Found if the flashcard set or flashcard does not exist.
    /// - 409 Conflict if there is a conflict in ordering or duplicate entries.
    /// </returns>
    [HttpPost("{flashcardId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [FlashcardSetAuthorizationFilter(FlashcardSetAccessType.Edit, 
        FlashcardSetIdArgName = nameof(flashcardId))]
    [FlashcardAuthorizationFilter(FlashcardAccessType.Read,
        FlashcardIdName = nameof(flashcardId))]
    public async Task<IActionResult> AddFlashcard([FromRoute] Guid flashcardSetId,
        [FromRoute] Guid flashcardId,
        [FromBody] AddFlashcardToSetDto dto,
        [FromServices] IFlashcardSetService flashcardSetService,
        [FromServices] IFlashcardService flashcardService)
    {
        if (dto.Order is null)
            await flashcardSetEntryService.AddFlashcardToTheEndAsync(flashcardSetId, flashcardId);
        else
            await flashcardSetEntryService.AddFlashcardAsync(flashcardSetId, flashcardId, dto.Order.Value);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetFlashcardsInSet(
        [FromRoute] Guid flashcardSetId,
        [FromServices] IFlashcardSetService flashcardSetService,
        [FromServices] IAuthorizationService authorizationService,
        [FromQuery] int page,
        [FromQuery] int limit = 15
    )
    {
        var (flashcardSet, flashcardDtos) =
            await flashcardSetService.GetWithEntriesPageAsync<GetFlashcardDto>(flashcardSetId, page, limit,
                GetFlashcardDto.Projection);
        var authRes =
            await authorizationService.AuthorizeAsync(User, flashcardSet, FlashcardSetAccessRequirement.ReadAccess);
        if (!authRes.Succeeded) return Forbid();
        return Ok(flashcardDtos);
    }

    [HttpDelete("{flashcardId:guid}")]
    public async Task<IActionResult> RemoveFlashcardFromSet(Guid flashcardSetId, Guid flashcardId,
        [FromServices] IFlashcardSetService flashcardSetService,
        [FromServices] IFlashcardService flashcardService,
        [FromServices] IAuthorizationService authorizationService)
    {
        var flashcardSet = await flashcardSetService.GetAsync(flashcardSetId);
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, flashcardSet, Requirements.FlashcardSet.EditAccess);
        if (!authorizationResult.Succeeded) return Forbid();
        var flashcard = await flashcardService.GetAsync(flashcardId);
        authorizationResult =
            await authorizationService.AuthorizeAsync(User, flashcard, Requirements.Flashcard.EditAccess);
        if (!authorizationResult.Succeeded) return Forbid();

        await flashcardSetEntryService.RemoveFlashcardFromSetAsync(flashcardSetId, flashcardId);
        return NoContent();
    }
}