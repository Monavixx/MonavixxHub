using Microsoft.AspNetCore.Authorization;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;

/// <summary>
/// Represents an authorization requirement for accessing a specific flashcard set.
/// </summary>
/// <param name="accessType">
/// Specifies the type of access required for the flashcard set, 
/// </param>
public class FlashcardSetAccessRequirement(FlashcardSetAccessType accessType) : IAuthorizationRequirement
{
    public FlashcardSetAccessType AccessType { get; init; } = accessType;
}