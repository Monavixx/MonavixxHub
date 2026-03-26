namespace MonavixxHub.Api.Features.Flashcards.DTOs;

public record AddFlashcardToSetDto
(
    Guid FlashcardId,
    int? Order
);