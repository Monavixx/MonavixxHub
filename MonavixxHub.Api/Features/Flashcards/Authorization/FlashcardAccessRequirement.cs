using Microsoft.AspNetCore.Authorization;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;

public class FlashcardAccessRequirement : IAuthorizationRequirement
{
    public FlashcardAccessType AccessType { get; init; }

    public FlashcardAccessRequirement(FlashcardAccessType accessType)
    {
        AccessType = accessType;
    }
}