using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Flashcards.Models;

public class FlashcardSetUser
{
    [Required] public Guid FlashcardSetId { get; set; }
    [Required] public UserIdType UserId { get; set; }
    public FlashcardSet FlashcardSet { get; set; }
    public User User { get; set; }
}