using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Features.Images.Models;

namespace MonavixxHub.Api.Features.Flashcards.Models;

public class Flashcard
{
    public const int FrontMaxLength = 500;
    public const int BackMaxLength = 500;
    public const int TranslationMaxLength = 500;
    public const int ImageMaxSize = 5242880;
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Front { get; set; }
    public string Back { get; set; }
    public string? Transcription { get; set; }
    public int OwnerId { get; set; }
    public User Owner { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid? ImageId { get; set; }
    public Image? Image { get; set; }
    public ICollection<FlashcardSetEntry> Entries { get; } = [];
}