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
    public async ValueTask<IActionResult> Get(Guid id, [FromServices] FlashcardSetService flashcardSetService)
    {
        try
        {
            var flashcardSet = await flashcardSetService.GetAsync(id);
            var authorizationResult =
                await authorizationService.AuthorizeAsync(User, flashcardSet, Requirements.FlashcardSet.ReadAccess);
            if (authorizationResult.Succeeded)
            {
                return Ok(GetFlashcardSetDto.From(flashcardSet));
            }
            return Forbid();
        }
        catch (FlashcardSetNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] CreateFlashcardSetDto dto,
        [FromServices] FlashcardSetService flashcardSetService)
    {
        try
        {
            var flashcardSet = await flashcardSetService.CreateAsync(dto, CurrentUserId);
            return Ok(GetFlashcardSetDto.From(flashcardSet));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async ValueTask<IActionResult> Put(Guid id, [FromBody] UpdateFlashcardSetDto dto,
        [FromServices] FlashcardSetService flashcardSetService)
    {
        try
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
        catch (FlashcardSetNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}