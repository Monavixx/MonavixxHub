using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs;

public class GetFlashcardDto
{
    public string Front { get; set; }
    public string Back { get; set; }
    public string? Transcription { get; set; }
    public Guid? ImageId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid Id { get; set; }

    public static GetFlashcardDto FromFlashcard(Flashcard flashcard)
        => new GetFlashcardDto
        {
            CreatedAt = flashcard.CreatedAt,
            UpdatedAt = flashcard.UpdatedAt,
            Transcription = flashcard.Transcription,
            Id = flashcard.Id,
            ImageId = flashcard.ImageId,
            Back = flashcard.Back,
            Front = flashcard.Front
        };
}