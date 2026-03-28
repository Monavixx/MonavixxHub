using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.Flashcards.Services;

namespace MonavixxHub.Api.Features.Flashcards.Controllers;

/// <summary>
/// Provides CRUD endpoints for managing flashcards.
/// All endpoints require authorization.
/// </summary>
/// <remarks>
/// Each method ensures proper access control using <see cref="FlashcardAccessRequirement"/> 
/// and the <see cref="FlashcardAuthorizationHandler"/> via ASP.NET Core authorization policies.
/// </remarks>
[Authorize]
[ApiController]
[Route("api/flashcards")]
public class FlashcardsController(FlashcardService flashcardService) : ControllerBase
{
    /// <summary>
    /// Retrieves all flashcards that the current user owns.
    /// </summary>
    /// <returns>
    /// Returns <see cref="IActionResult"/> with a 200 OK containing a list of <see cref="GetFlashcardDto"/>.
    /// </returns>
    [HttpGet("my")]
    [ProducesResponseType<IEnumerable<GetFlashcardDto>>(StatusCodes.Status200OK)]
    public IActionResult GetUsersFlashcards()
    {
        return Ok(flashcardService.GetAll(User)
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
    /// <summary>
    /// Retrieves a specific flashcard by ID if the user has read access.
    /// </summary>
    /// <returns>
    /// Returns 200 OK with <see cref="GetFlashcardDto"/> if authorized,
    /// 403 Forbidden if the user cannot read, or 404 Not Found if the flashcard does not exist.
    /// </returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<GetFlashcardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Get(Guid id,
        [FromServices] IAuthorizationService authorizationService)
    {
        var flashcard = await flashcardService.GetAsync(id);
        var authRes =
            await authorizationService.AuthorizeAsync(User, flashcard, Requirements.Flashcard.ReadAccess);
        if (!authRes.Succeeded) return Forbid();
        return Ok(GetFlashcardDto.FromFlashcard(flashcard));
    }

    /// <summary>
    /// Creates a new flashcard for the current user.
    /// </summary>
    /// <returns>
    /// Returns 201 Created with <see cref="GetFlashcardDto"/> representing the created flashcard.
    /// </returns>
    [HttpPost]
    [ProducesResponseType<GetFlashcardDto>(StatusCodes.Status201Created)]
    public async ValueTask<IActionResult> Create([FromForm] CreateFlashcardDto dto)
    {
        var flashcard = await flashcardService.CreateAsync(dto, User);
        return CreatedAtAction(nameof(Get), new { id = flashcard.Id }, GetFlashcardDto.FromFlashcard(flashcard));
    }

    /// <summary>
    /// Partially updates a flashcard if the current user has edit access.
    /// </summary>
    /// <param name="id">The ID of the flashcard to update.</param>
    /// <param name="dto">A <see cref="PatchFlashcardDto"/> containing the fields to update.</param>
    /// <param name="authorizationService">Service to evaluate the <see cref="FlashcardAccessRequirement"/> for edit access.</param>
    /// <returns>
    /// Returns 200 OK with the updated <see cref="GetFlashcardDto"/> if successful,
    /// 403 Forbidden if the user lacks edit access,
    /// or 404 Not Found if the flashcard does not exist.
    /// </returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType<GetFlashcardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Patch(Guid id, [FromForm] PatchFlashcardDto dto,
        [FromServices] IAuthorizationService authorizationService)
    {
        return await EditAccess(id, authorizationService, async flashcard =>
        {
            await flashcardService.PatchAsync(flashcard, dto);
            return Ok(GetFlashcardDto.FromFlashcard(flashcard));
        });
    }
    /// <summary>
    /// Fully updates a flashcard if the current user has edit access.
    /// </summary>
    /// <param name="id">The ID of the flashcard to update.</param>
    /// <param name="dto">A <see cref="UpdateFlashcardDto"/> containing the new data for the flashcard.</param>
    /// <param name="authorizationService">Service to evaluate the <see cref="FlashcardAccessRequirement"/> for edit access.</param>
    /// <returns>
    /// Returns 200 OK with the updated <see cref="GetFlashcardDto"/> if successful,
    /// 403 Forbidden if the user lacks edit access,
    /// or 404 Not Found if the flashcard does not exist.
    /// </returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<GetFlashcardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Update(Guid id, [FromForm] UpdateFlashcardDto dto,
        [FromServices] IAuthorizationService authorizationService)
    {
        return await EditAccess(id, authorizationService, async flashcard =>
        {
            await flashcardService.UpdateAsync(flashcard, dto);
            return Ok(GetFlashcardDto.FromFlashcard(flashcard));
        });
    }

    /// <summary>
    /// Deletes a flashcard if the current user has edit access.
    /// </summary>
    /// <param name="id">The ID of the flashcard to delete.</param>
    /// <param name="authorizationService">Service to evaluate the <see cref="FlashcardAccessRequirement"/> for edit access.</param>
    /// <returns>
    /// Returns 204 No Content if the deletion is successful,
    /// 403 Forbidden if the user lacks edit access,
    /// or 404 Not Found if the flashcard does not exist.
    /// </returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Delete(Guid id,
        [FromServices] IAuthorizationService authorizationService)
    {
        return await EditAccess(id, authorizationService, async flashcard =>
        {
            await flashcardService.DeleteAsync(flashcard);
            return NoContent();
        });
    }

    /// <summary>
    /// Evaluates edit access for a specific flashcard and executes a given function if authorized.
    /// </summary>
    /// <param name="id">The ID of the flashcard to check.</param>
    /// <param name="authorizationService">Service to check the user's edit access.</param>
    /// <param name="func">A function to execute if the user is authorized.</param>
    /// <returns>
    /// Returns the result of <paramref name="func"/> if authorized, otherwise 403 Forbidden.
    /// </returns>
    private async ValueTask<IActionResult> EditAccess(Guid id, IAuthorizationService authorizationService,
        Func<Flashcard, ValueTask<IActionResult>> func)
    {
        var flashcard = await flashcardService.GetAsync(id);
        var authRes = await authorizationService.AuthorizeAsync(User, flashcard, Requirements.Flashcard.EditAccess);
        if (!authRes.Succeeded) return Forbid();
        return await func(flashcard);
    }
}