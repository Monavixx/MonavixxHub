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
    
    public static readonly FlashcardAccessRequirement ReadAccess = new(FlashcardAccessType.Read);
    public static readonly FlashcardAccessRequirement EditAccess = new(FlashcardAccessType.Edit);

    public static FlashcardAccessRequirement Resolve(FlashcardAccessType accessType)
        => accessType switch
        {
            FlashcardAccessType.Read => ReadAccess,
            FlashcardAccessType.Edit => EditAccess,
            _ => throw new ArgumentOutOfRangeException(nameof(accessType), $"Unsupported access type: {accessType}")
        };
}