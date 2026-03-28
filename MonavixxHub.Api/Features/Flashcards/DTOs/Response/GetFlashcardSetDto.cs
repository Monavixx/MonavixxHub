using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs.Response;

public record GetFlashcardSetDto
(
    Guid Id,
    string Name,
    Guid? ParentSetId,
    bool IsPublic,
    int OwnerId,
    IEnumerable<Guid> SubsetsIds
)
{
    public GetFlashcardSetDto(FlashcardSet flashcardSet) : this(flashcardSet.Id, flashcardSet.Name,
        flashcardSet.ParentSetId, flashcardSet.IsPublic, flashcardSet.OwnerId,
        flashcardSet.Subsets.Select(subset => subset.Id))
    {
    }
}