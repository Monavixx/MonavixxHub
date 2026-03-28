using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs.Request;

public record PatchFlashcardDto
(
    [StringLength(Flashcard.FrontMaxLength)]
    string? Front,
    [StringLength(Flashcard.BackMaxLength)]
    string? Back,
    [StringLength(Flashcard.TranslationMaxLength)]
    string? Transcription,
    IFormFile? Image
);