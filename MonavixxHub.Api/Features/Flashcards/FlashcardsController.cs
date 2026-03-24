using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Exceptions;

namespace MonavixxHub.Api.Features.Flashcards;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FlashcardsController : ControllerBase
{
    protected int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    [HttpGet("all")]
    public IActionResult Get([FromServices] FlashcardService flashcardService)
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
            return Ok(new GetFlashcardDto
            {
                CreatedAt = flashcard.CreatedAt,
                UpdatedAt = flashcard.UpdatedAt,
                Transcription = flashcard.Transcription,
                Id = flashcard.Id,
                ImageId = flashcard.ImageId,
                Back = flashcard.Back,
                Front = flashcard.Front
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id:guid}")]
    public async ValueTask<IActionResult> Patch(Guid id, [FromForm] PatchFlashcardDto dto,
        [FromServices] FlashcardService flashcardService)
    {
        try
        {
            var flashcard = await flashcardService.PatchAsync(id, dto, CurrentUserId);
            return Ok(new GetFlashcardDto
            {
                CreatedAt = flashcard.CreatedAt,
                UpdatedAt = flashcard.UpdatedAt,
                Transcription = flashcard.Transcription,
                Id = flashcard.Id,
                ImageId = flashcard.ImageId,
                Back = flashcard.Back,
                Front = flashcard.Front
            });
        }
        catch (UserDoesNotHaveFlashcardWithSuchId ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("{id:guid}")]
    public async ValueTask<IActionResult> Update(Guid id, [FromForm] UpdateFlashcardDto dto,
        [FromServices] FlashcardService flashcardService)
    {
        try
        {
            var flashcard = await flashcardService.UpdateAsync(id, dto, CurrentUserId);
            return Ok(new GetFlashcardDto
            {
                CreatedAt = flashcard.CreatedAt,
                UpdatedAt = flashcard.UpdatedAt,
                Transcription = flashcard.Transcription,
                Id = flashcard.Id,
                ImageId = flashcard.ImageId,
                Back = flashcard.Back,
                Front = flashcard.Front
            });
        }
        catch (UserDoesNotHaveFlashcardWithSuchId ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async ValueTask<IActionResult> Delete(Guid id, [FromServices] FlashcardService flashcardService)
    {
        try
        {
            await flashcardService.DeleteAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (UserDoesNotHaveFlashcardWithSuchId ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}