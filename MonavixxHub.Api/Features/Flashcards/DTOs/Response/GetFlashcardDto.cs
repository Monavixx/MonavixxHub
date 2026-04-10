using System.Linq.Expressions;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs.Response;

public record GetFlashcardDto
(
    string Front,
    string Back,
    string? Transcription,
    Guid? ImageId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid Id
) {
    public static GetFlashcardDto FromFlashcard(Flashcard flashcard)
        => new GetFlashcardDto
        (
            CreatedAt: flashcard.CreatedAt,
            UpdatedAt: flashcard.UpdatedAt,
            Transcription: flashcard.Transcription,
            Id: flashcard.Id,
            ImageId: flashcard.ImageId,
            Back: flashcard.Back,
            Front: flashcard.Front
        );
    public static readonly Expression<Func<Flashcard, GetFlashcardDto>> Projection =
        f => new GetFlashcardDto(f.Front, f.Back, f.Transcription, f.ImageId, f.CreatedAt, f.UpdatedAt, f.Id);
}