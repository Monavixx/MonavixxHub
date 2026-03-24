namespace MonavixxHub.Api.Features.Flashcards.DTOs;

public class PatchFlashcardDto
{
    public string? Front { get; set; }
    public string? Back { get; set; }
    public string? Transcription { get; set; }
    public IFormFile? Image { get; set; }
}