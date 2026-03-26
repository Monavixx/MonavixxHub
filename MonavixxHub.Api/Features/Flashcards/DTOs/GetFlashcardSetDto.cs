using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs;

public record GetFlashcardSetDto
(
    Guid Id,
    string Name,
    Guid? ParentSetId,
    bool IsPublic,
    int OwnerId,
    IEnumerable<Guid> SubsetsIds
) {
    public static GetFlashcardSetDto From(FlashcardSet flashcardSet)
    {
        return new GetFlashcardSetDto(
            Id: flashcardSet.Id,
            Name: flashcardSet.Name,
            ParentSetId: flashcardSet.ParentSetId,
            IsPublic: flashcardSet.IsPublic,
            OwnerId: flashcardSet.OwnerId,
            SubsetsIds: flashcardSet.Subsets.Select(subset => subset.Id)
        );
    }
}