using Microsoft.AspNetCore.Authorization;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;

public class FlashcardSetAccessRequirement : IAuthorizationRequirement
{
    public FlashcardSetAccessType AccessType { get; init; }
    public FlashcardSetAccessRequirement(FlashcardSetAccessType accessType)
    {
        AccessType = accessType;
    }
}