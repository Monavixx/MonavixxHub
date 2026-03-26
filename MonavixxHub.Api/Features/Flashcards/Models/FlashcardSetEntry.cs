using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Features.Flashcards.Models;

public class FlashcardSetEntry
{
    [Required] public Guid FlashcardSetId { get; set; }
    public FlashcardSet FlashcardSet { get; set; }
    [Required] public Guid FlashcardId { get; set; }
    public Flashcard Flashcard { get; set; }
    [Required] public int Order { get; set; }
}