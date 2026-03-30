using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.FlashcardsStudy.Models;

public class RandomStudySession
{
    [Required] public Guid FlashcardSetId { get; set; }
    [Required] public UserIdType UserId { get; set; }
    [Required] public Guid LastFlashcardId { get; set; }
    [Required] public bool IsAnswered { get; set; } = false;
    
    public FlashcardSet FlashcardSet { get; set; }
    public User User { get; set; }
    public Flashcard LastFlashcard { get; set; }
}