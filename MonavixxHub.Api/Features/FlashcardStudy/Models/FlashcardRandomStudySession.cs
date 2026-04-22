namespace MonavixxHub.Api.Features.FlashcardStudy.Models;

public class FlashcardRandomStudySession
{
    public Guid FlashcardId { get; set; }
    public UserIdType UserId { get; set; }
    public Guid FlashcardSetId { get; set; }
}