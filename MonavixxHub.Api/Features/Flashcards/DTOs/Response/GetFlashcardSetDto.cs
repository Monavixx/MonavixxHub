using System.Linq.Expressions;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs.Response;

public record GetFlashcardSetDto
(
    Guid Id,
    string Name,
    Guid? ParentSetId,
    bool IsPublic,
    UserIdType OwnerId,
    IEnumerable<Guid> SubsetsIds
)
{
    public GetFlashcardSetDto(FlashcardSet flashcardSet) : this(flashcardSet.Id, flashcardSet.Name,
        flashcardSet.ParentSetId, flashcardSet.IsPublic, flashcardSet.OwnerId,
        flashcardSet.Subsets.Select(subset => subset.Id))
    {
    }

    public static readonly Expression<Func<FlashcardSet, GetFlashcardSetDto>> Projection =
        s => new GetFlashcardSetDto(s.Id, s.Name, s.ParentSetId, s.IsPublic, s.OwnerId,
            s.Subsets.Select(subset => subset.Id));
}