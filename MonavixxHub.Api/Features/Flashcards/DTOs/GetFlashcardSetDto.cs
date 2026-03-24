using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs;

public class GetFlashcardSetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentSetId { get; set; } = null;
    public bool IsPublic  { get; set; } = false;
    public int OwnerId { get; set; }
    public IEnumerable<Guid> SubsetsIds { get; init; } = [];

    public static GetFlashcardSetDto From(FlashcardSet flashcardSet)
    {
        return new GetFlashcardSetDto()
        {
            Id = flashcardSet.Id,
            Name = flashcardSet.Name,
            ParentSetId = flashcardSet.ParentSetId,
            IsPublic = flashcardSet.IsPublic,
            OwnerId = flashcardSet.OwnerId,
            SubsetsIds = flashcardSet.Subsets.Select(subset => subset.Id)
        };
    }
}