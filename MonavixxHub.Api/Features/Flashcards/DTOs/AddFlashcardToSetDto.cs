namespace MonavixxHub.Api.Features.Flashcards.DTOs;

public class AddFlashcardToSetDto
{
    public Guid FlashcardId { get; set; }
    public int? Order { get; set; }
}