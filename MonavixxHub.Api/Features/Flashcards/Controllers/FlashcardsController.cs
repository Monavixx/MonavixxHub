using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Common;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.Authorization.Filters;
using MonavixxHub.Api.Features.Flashcards.DTOs.Request;
using MonavixxHub.Api.Features.Flashcards.DTOs.Response;
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
[Authorize(Policy = Policies.EmailConfirmed)]
public class FlashcardsController(IFlashcardService flashcardService) : ControllerBase
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
        return Ok(flashcardService.GetAll(User.GetUserId())
            .Select(GetFlashcardDto.Projection));
    }

    [HttpGet("my/page/{page:int}")]
    public IActionResult GetPageUsersFlashcard(int page, [FromQuery] int limit = 10)
    {
        return Ok(flashcardService.GetPage(User.GetUserId(), page, limit));
    }
    /// <summary>
    /// Retrieves a specific flashcard by ID if the user has read access.
    /// </summary>
    /// <returns>
    /// Returns 200 OK with <see cref="GetFlashcardDto"/> if authorized,
    /// 403 Forbidden if the user cannot read, or 404 Not Found if the flashcard does not exist.
    /// </returns>
    [HttpGet("{flashcardId:guid}")]
    [ProducesResponseType<GetFlashcardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [FlashcardAuthorizationFilter(FlashcardAccessType.Read)]
    public IActionResult Get([FromRoute] Guid flashcardId)
    {
        var flashcard = (Flashcard)HttpContext.Items[FlashcardAuthorizationFilterAttribute.FlashcardKey]!;
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
    public async ValueTask<IActionResult> Create([FromForm] CreateFlashcardDto dto, [FromServices]
        IFlashcardSetEntryService flashcardSetEntryService,
        [FromServices]IAuthorizationService authorizationService,
        [FromServices] IFlashcardSetService flashcardSetService)
    {
        if (dto.FlashcardSetId is { } fsi &&
            !(await authorizationService.AuthorizeAsync(User, await flashcardSetService.GetAsync(fsi),
                FlashcardSetAccessRequirement.EditAccess)).Succeeded)
            return Forbid();
        var flashcard = await flashcardService.CreateAsync(dto, User);
        
        return CreatedAtAction(nameof(Get), new { id = flashcard.Id }, GetFlashcardDto.FromFlashcard(flashcard));
    }
    /// <summary>
    /// Partially updates a flashcard if the current user has edit access.
    /// </summary>
    /// <param name="flashcardId">The ID of the flashcard to update.</param>
    /// <param name="dto">A <see cref="PatchFlashcardDto"/> containing the fields to update.</param>
    /// <returns>
    /// Returns 200 OK with the updated <see cref="GetFlashcardDto"/> if successful,
    /// 403 Forbidden if the user lacks edit access,
    /// or 404 Not Found if the flashcard does not exist.
    /// </returns>
    [HttpPatch("{flashcardId:guid}")]
    [ProducesResponseType<GetFlashcardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [FlashcardAuthorizationFilter(FlashcardAccessType.Edit)]
    public async ValueTask<IActionResult> Patch([FromRoute] Guid flashcardId, [FromForm] PatchFlashcardDto dto)
    {
        var flashcard = (Flashcard)HttpContext.Items[FlashcardAuthorizationFilterAttribute.FlashcardKey]!;
        await flashcardService.PatchAsync(flashcard, dto);
        return Ok(GetFlashcardDto.FromFlashcard(flashcard));
    }
    /// <summary>
    /// Fully updates a flashcard if the current user has edit access.
    /// </summary>
    /// <param name="flashcardId">The ID of the flashcard to update.</param>
    /// <param name="dto">A <see cref="UpdateFlashcardDto"/> containing the new data for the flashcard.</param>
    /// <returns>
    /// Returns 200 OK with the updated <see cref="GetFlashcardDto"/> if successful,
    /// 403 Forbidden if the user lacks edit access,
    /// or 404 Not Found if the flashcard does not exist.
    /// </returns>
    [HttpPut("{flashcardId:guid}")]
    [ProducesResponseType<GetFlashcardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [FlashcardAuthorizationFilter(FlashcardAccessType.Edit)]
    public async ValueTask<IActionResult> Update([FromRoute] Guid flashcardId, [FromForm] UpdateFlashcardDto dto)
    {
        var flashcard = (Flashcard)HttpContext.Items[FlashcardAuthorizationFilterAttribute.FlashcardKey]!;
        await flashcardService.UpdateAsync(flashcard, dto);
        return Ok(GetFlashcardDto.FromFlashcard(flashcard));
    }

    /// <summary>
    /// Deletes a flashcard if the current user has edit access.
    /// </summary>
    /// <param name="flashcardId">The ID of the flashcard to delete.</param>
    /// <returns>
    /// Returns 204 No Content if the deletion is successful,
    /// 403 Forbidden if the user lacks edit access,
    /// or 404 Not Found if the flashcard does not exist.
    /// </returns>
    [HttpDelete("{flashcardId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [FlashcardAuthorizationFilter(FlashcardAccessType.Edit)]
    public async ValueTask<IActionResult> Delete([FromRoute] Guid flashcardId)
    {
        var flashcard = (Flashcard)HttpContext.Items[FlashcardAuthorizationFilterAttribute.FlashcardKey]!;
        await flashcardService.DeleteAsync(flashcard);
        return NoContent();
    }
}