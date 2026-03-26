using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs;

public record CreateFlashcardSetDto
(
    [StringLength(FlashcardSet.NameMaxLength)]
    string Name,
    Guid? ParentSetId,
    bool IsPublic
);