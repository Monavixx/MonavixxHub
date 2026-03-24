namespace MonavixxHub.Api.Features.Flashcards.DTOs;

public class UpdateFlashcardSetDto
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentSetId { get; set; } = null;
    public bool IsPublic  { get; set; } = false;
}