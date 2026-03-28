using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs.Response;

public record GetFlashcardSetWithEntriesDto(
    Guid Id,
    string Name,
    Guid? ParentSetId,
    bool IsPublic,
    int OwnerId,
    IEnumerable<Guid> SubsetsIds,
    IEnumerable<GetFlashcardDto> Entries) : GetFlashcardSetDto(Id, Name, ParentSetId, IsPublic, OwnerId, SubsetsIds)
{
    /// <summary>
    /// Takes data from the provided flashcard set.
    /// </summary>
    /// <param name="flashcardSet">
    /// The flashcard set to take data from.
    /// Requires <see cref="FlashcardSet.Entries"/> to be included as well as <see cref="FlashcardSetEntry.Flashcard"/>
    /// </param>
    public GetFlashcardSetWithEntriesDto(FlashcardSet flashcardSet) : this(flashcardSet.Id, flashcardSet.Name,
        flashcardSet.ParentSetId, flashcardSet.IsPublic, flashcardSet.OwnerId,
        flashcardSet.Subsets.Select(subset => subset.Id), 
        flashcardSet.Entries.Select(e => GetFlashcardDto.FromFlashcard(e.Flashcard)))
    { }
}