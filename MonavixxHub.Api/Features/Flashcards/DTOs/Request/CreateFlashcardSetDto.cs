using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs.Request;

public record CreateFlashcardSetDto
(
    [StringLength(FlashcardSet.NameMaxLength)]
    [Required] string Name,
    Guid? ParentSetId,
    [Required] bool IsPublic
);