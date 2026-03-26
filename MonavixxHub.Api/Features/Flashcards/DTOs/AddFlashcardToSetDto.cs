using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Features.Flashcards.DTOs;

public record AddFlashcardToSetDto
(
    [Required] Guid FlashcardId,
    int? Order
);