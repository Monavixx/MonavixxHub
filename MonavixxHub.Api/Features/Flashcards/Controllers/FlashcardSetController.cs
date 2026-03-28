using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.DTOs.Request;
using MonavixxHub.Api.Features.Flashcards.DTOs.Response;
using MonavixxHub.Api.Features.Flashcards.Services;

namespace MonavixxHub.Api.Features.Flashcards.Controllers;

/// <summary>
/// Provides CRUD endpoints for working with flashcard sets.
/// </summary>
/// <remarks>
/// All endpoints require authentication (<see cref="AuthorizeAttribute"/>). 
/// Authorization is also applied per endpoint to determine if the user can read or edit a specific flashcard set.
/// </remarks>
[Authorize]
[ApiController]
[Route("api/flashcard-sets")]
public class FlashcardSetController(IAuthorizationService authorizationService) : ControllerBase
{
    /// <summary>
    /// Retrieves a specific flashcard set by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the flashcard set to retrieve.</param>
    /// <param name="includeEntries">Whether to include entries in the response or not. Default to false.</param>
    /// <param name="ordered">Whether to sort the entries in the response or not.
    /// Ignores if <paramref name="includeEntries"/> is false.
    /// Default to true.</param>
    /// <param name="flashcardSetService">Service used to access flashcard sets.</param>
    /// <returns>
    /// - 200 OK with <see cref="GetFlashcardSetDto"/> if the user has read access.
    /// - 403 Forbidden if the user is not authorized.
    /// - 404 Not Found if the flashcard set does not exist.
    /// </returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Get(Guid id, 
        [FromQuery(Name="entries")] bool? includeEntries,
        [FromQuery(Name="ordered")] bool? ordered,
        [FromServices] FlashcardSetService flashcardSetService)
    {
        // TODO: add a query boolean parameter that determines whether to include entries or not.
        var flashcardSet = await flashcardSetService.GetAsync(id);
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, flashcardSet, Requirements.FlashcardSet.ReadAccess);
        if (!authorizationResult.Succeeded) return Forbid();
        
        await flashcardSetService.EnsureSubsetsLoadedAsync(flashcardSet);
        if (includeEntries is true)
        {
            if (ordered is not false)
                await flashcardSetService.EnsureEntriesLoadedAndOrderedWithFlashcardAsync(flashcardSet);
            else
                await flashcardSetService.EnsureEntriesLoadedWithFlashcardAsync(flashcardSet);
            return Ok(new GetFlashcardSetWithEntriesDto(flashcardSet));
        }
        return Ok(new GetFlashcardSetDto(flashcardSet));
    }

    /// <summary>
    /// Creates a new flashcard set for the authenticated user.
    /// </summary>
    /// <param name="dto">Data required to create a flashcard set.</param>
    /// <param name="flashcardSetService">Service used to access flashcard sets.</param>
    /// <returns>
    /// - 201 Created with <see cref="GetFlashcardSetDto"/> for the newly created flashcard set.
    /// - 404 Not Found if a related resource does not exist.
    /// </returns>
    [HttpPost]
    [ProducesResponseType<GetFlashcardSetDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> Create([FromBody] CreateFlashcardSetDto dto,
        [FromServices] FlashcardSetService flashcardSetService)
    {
        var flashcardSet = await flashcardSetService.CreateAsync(dto, User);
        return CreatedAtAction(nameof(Get), 
            new { id = flashcardSet.Id }, new GetFlashcardSetDto(flashcardSet));
    }
    /// <summary>
    /// Updates an existing flashcard set.
    /// </summary>
    /// <param name="id">The ID of the flashcard set to update.</param>
    /// <param name="dto">Data used to update the flashcard set.</param>
    /// <param name="flashcardSetService">Service used to access flashcard sets.</param>
    /// <returns>
    /// - 200 OK with <see cref="GetFlashcardSetDto"/> if the user has edit access.
    /// - 403 Forbidden if the user is not authorized.
    /// - 404 Not Found if the flashcard set does not exist.
    /// </returns>
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
            return Ok(new GetFlashcardSetDto(await flashcardSetService.UpdateAsync(flashcardSet, dto)));
        return Forbid();
    }
}