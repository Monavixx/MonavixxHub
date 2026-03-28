using Microsoft.AspNetCore.Authorization;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;

/// <summary>
/// Represents an authorization requirement for accessing a specific flashcard.
/// </summary>
/// <param name="accessType">
/// Specifies the type of access required.
/// </param>
public class FlashcardAccessRequirement(FlashcardAccessType accessType) : IAuthorizationRequirement
{
    public FlashcardAccessType AccessType { get; init; } = accessType;
}