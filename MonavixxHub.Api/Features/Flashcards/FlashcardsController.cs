using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Exceptions;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FlashcardsController : ControllerBase
{
    protected int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    [HttpGet("all")]
    public IActionResult GetAll([FromServices] FlashcardService flashcardService)
    {
        return Ok(flashcardService.GetAll(CurrentUserId).AsEnumerable()
            .Select(GetFlashcardDto.FromFlashcard));
    }

    [HttpPost]
    public async ValueTask<IActionResult> Create([FromForm] CreateFlashcardDto dto,
        [FromServices] FlashcardService flashcardService)
    {
        try
        {
            var flashcard = await flashcardService.CreateAsync(dto, CurrentUserId);
            return Ok(GetFlashcardDto.FromFlashcard(flashcard));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id:guid}")]
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
        try
        {
            var flashcard = await flashcardService.GetAsync(id);
            var authRes = await authorizationService.AuthorizeAsync(User, flashcard, Requirements.Flashcard.EditAccess);
            if (!authRes.Succeeded) return Forbid();
            return await func(flashcard);
        }
        catch (FlashcardNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}