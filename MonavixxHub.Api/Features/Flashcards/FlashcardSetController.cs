using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Exceptions;

namespace MonavixxHub.Api.Features.Flashcards;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FlashcardSetController(IAuthorizationService authorizationService) : ControllerBase
{
    protected int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType<GetFlashcardSetDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Get(Guid id, [FromServices] FlashcardSetService flashcardSetService)
    {
        var flashcardSet = await flashcardSetService.GetAsync(id);
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, flashcardSet, Requirements.FlashcardSet.ReadAccess);
        if (authorizationResult.Succeeded)
            return Ok(GetFlashcardSetDto.From(flashcardSet));
        return Forbid();
    }

    [HttpPost]
    [ProducesResponseType<GetFlashcardSetDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> Create([FromBody] CreateFlashcardSetDto dto,
        [FromServices] FlashcardSetService flashcardSetService)
    {
        var flashcardSet = await flashcardSetService.CreateAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(Get), 
            new { id = flashcardSet.Id }, GetFlashcardSetDto.From(flashcardSet));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<GetFlashcardSetDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Put(Guid id, [FromBody] UpdateFlashcardSetDto dto,
        [FromServices] FlashcardSetService flashcardSetService)
    {
        var flashcardSet = await flashcardSetService.GetAsync(id);
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, flashcardSet, Requirements.FlashcardSet.EditAccess);
        if (authorizationResult.Succeeded)
            return Ok(
                GetFlashcardSetDto.From(
                    await flashcardSetService.UpdateAsync(flashcardSet, dto)));
        return Forbid();
    }

    [HttpPost("{flashcardSetId:guid}/add-flashcard/")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async ValueTask<IActionResult> AddFlashcard(Guid flashcardSetId,
        [FromServices] FlashcardSetService flashcardSetService,
        [FromBody] AddFlashcardToSetDto dto,
        [FromServices] FlashcardService flashcardService)
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
            await flashcardSetService.AddFlashcardToTheEndAsync(flashcardSetId, dto.FlashcardId);
        else
            await flashcardSetService.AddFlashcardAsync(flashcardSetId, dto.FlashcardId, dto.Order.Value);
        return NoContent();
    }
}