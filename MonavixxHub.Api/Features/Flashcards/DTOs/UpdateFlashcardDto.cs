namespace MonavixxHub.Api.Features.Flashcards.DTOs;

public class UpdateFlashcardDto
{
    public string Front { get; set; }
    public string Back { get; set; }
    public string? Transcription { get; set; }
    public IFormFile? Image { get; set; }
}