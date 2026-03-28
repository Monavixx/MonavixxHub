using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs.Request;

public record CreateFlashcardDto (
    [StringLength(Flashcard.FrontMaxLength, MinimumLength = 1)]
    [Required]
    string Front,
    [StringLength(Flashcard.BackMaxLength, MinimumLength = 1)]
    [Required]
    string Back,
    [StringLength(Flashcard.TranslationMaxLength)]
    string? Transcription,
    IFormFile? Image
);