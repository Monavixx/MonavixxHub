using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Features.Flashcards.DTOs.Request;

public record AddFlashcardToSetDto
(
    int? Order
);