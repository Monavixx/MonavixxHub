using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Features.Images.Models;

namespace MonavixxHub.Api.Features.Flashcards.Models;

public class Flashcard
{
    public const int FrontMaxLength = 500;
    public const int BackMaxLength = 500;
    public const int TranslationMaxLength = 500;
    public const int ImageMaxSize = 1024*1024*50;
    [Required] public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public string Front { get; set; }
    [Required] public string Back { get; set; }
    public string? Transcription { get; set; }
    [Required] public UserIdType OwnerId { get; set; }
    public User Owner { get; set; }
    [Required] public DateTimeOffset CreatedAt { get; set; }
    [Required] public DateTimeOffset UpdatedAt { get; set; }
    public Guid? ImageId { get; set; }
    public Image? Image { get; set; }
    public ICollection<FlashcardSetEntry> Entries { get; } = [];
}