using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.FlashcardStudy.Models;

public class RandomStudySession
{
    [Required] public Guid FlashcardSetId { get; set; }
    [Required] public UserIdType UserId { get; set; }
    public ICollection<Flashcard> AnsweredFlashcards { get; } = new List<Flashcard>();

    public FlashcardSet FlashcardSet { get; set; }
    public User User { get; set; }
}