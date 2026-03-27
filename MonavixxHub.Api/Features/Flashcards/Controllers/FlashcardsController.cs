using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.Flashcards.Services;

namespace MonavixxHub.Api.Features.Flashcards.Controllers;

/// <summary>
/// Contains endpoints to work with flashcards. This includes CRUD operations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FlashcardsController : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="flashcardService"></param>
    /// <returns></returns>
    
    [HttpGet("all")]
    [ProducesResponseType<IEnumerable<GetFlashcardDto>>(StatusCodes.Status200OK)]
    public IActionResult GetAll([FromServices] FlashcardService flashcardService)
    {
        return Ok(flashcardService.GetAll(User.GetUserId())
            .Select(flashcard => new GetFlashcardDto
            (
                Front: flashcard.Front,
                Back: flashcard.Back,
                Transcription: flashcard.Transcription,
                ImageId: flashcard.ImageId,
                CreatedAt: flashcard.CreatedAt,
                UpdatedAt: flashcard.UpdatedAt,
                Id: flashcard.Id
            )));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<GetFlashcardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Get(Guid id, [FromServices] FlashcardService flashcardService,
        [FromServices] IAuthorizationService authorizationService)
    {
        var flashcard = await flashcardService.GetAsync(id);
        var authRes =
            await authorizationService.AuthorizeAsync(User, flashcard, Requirements.Flashcard.ReadAccess);
        if (!authRes.Succeeded) return Forbid();
        return Ok(GetFlashcardDto.FromFlashcard(flashcard));
    }

    [HttpPost]
    [ProducesResponseType<GetFlashcardDto>(StatusCodes.Status201Created)]
    public async ValueTask<IActionResult> Create([FromForm] CreateFlashcardDto dto,
        [FromServices] FlashcardService flashcardService)
    {
        var flashcard = await flashcardService.CreateAsync(dto, User.GetUserId());
        return CreatedAtAction(nameof(Get), new { id = flashcard.Id }, GetFlashcardDto.FromFlashcard(flashcard));
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType<GetFlashcardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Patch(Guid id, [FromForm] PatchFlashcardDto dto,
        [FromServices] FlashcardService flashcardService,
        [FromServices] IAuthorizationService authorizationService)
    {
        return await EditAccess(id, flashcardService, authorizationService, async flashcard =>
        {
            await flashcardService.PatchAsync(flashcard, dto);
            return Ok(GetFlashcardDto.FromFlashcard(flashcard));
        });
    }
    [HttpPut("{id:guid}")]
    [ProducesResponseType<GetFlashcardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Update(Guid id, [FromForm] UpdateFlashcardDto dto,
        [FromServices] FlashcardService flashcardService,
        [FromServices] IAuthorizationService authorizationService)
    {
        return await EditAccess(id, flashcardService, authorizationService, async flashcard =>
        {
            await flashcardService.UpdateAsync(flashcard, dto);
            return Ok(GetFlashcardDto.FromFlashcard(flashcard));
        });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Delete(Guid id, [FromServices] FlashcardService flashcardService,
        [FromServices] IAuthorizationService authorizationService)
    {
        return await EditAccess(id, flashcardService, authorizationService, async flashcard =>
        {
            await flashcardService.DeleteAsync(flashcard);
            return NoContent();
        });
    }

    private async ValueTask<IActionResult> EditAccess(Guid id,
        FlashcardService flashcardService, IAuthorizationService authorizationService,
        Func<Flashcard, ValueTask<IActionResult>> func)
    {
        var flashcard = await flashcardService.GetAsync(id);
        var authRes = await authorizationService.AuthorizeAsync(User, flashcard, Requirements.Flashcard.EditAccess);
        if (!authRes.Succeeded) return Forbid();
        return await func(flashcard);
    }
}