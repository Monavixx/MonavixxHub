using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Features.Flashcards.Models;

public class FlashcardSetEntry
{
    [Required] public Guid FlashcardSetId { get; set; }
    public FlashcardSet FlashcardSet { get; set; }
    [Required] public Guid FlashcardId { get; set; }
    public Flashcard Flashcard { get; set; }
    [Required] public int Order { get; set; }
    
    //TODO: consider adding bool ReadOnly. Users will be able to add foreign flashcards. If user decides to modify
    // :ReadOnly flashcard, a new flashcard is created.
}