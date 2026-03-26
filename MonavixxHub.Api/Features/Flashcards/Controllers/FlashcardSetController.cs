using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Services;

namespace MonavixxHub.Api.Features.Flashcards.Controllers;

[Authorize]
[ApiController]
[Route("api/flashcard-sets")]
public class FlashcardSetController(IAuthorizationService authorizationService) : ControllerBase
{
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
        var flashcardSet = await flashcardSetService.CreateAsync(dto, User.GetUserId());
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
}