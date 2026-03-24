namespace MonavixxHub.Api.Features.Flashcards.Models;

public class FlashcardSetEntry
{
    public Guid FlashcardSetId { get; set; }
    public FlashcardSet FlashcardSet { get; set; }
    public Guid FlashcardId { get; set; }
    public Flashcard Flashcard { get; set; }
    public int Order { get; set; }
}