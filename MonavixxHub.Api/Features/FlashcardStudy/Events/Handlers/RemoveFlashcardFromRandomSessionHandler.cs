using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Common.Events;
using MonavixxHub.Api.Features.Flashcards.Events;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.FlashcardStudy.Events.Handlers;

public class RemoveFlashcardFromRandomSessionHandler(
    AppDbContext dbContext,
    ILogger<RemoveFlashcardFromRandomSessionHandler> logger)
    : IDomainEventHandler<FlashcardRemovedFromSetEvent>
{
    public async Task HandleAsync(FlashcardRemovedFromSetEvent @event, CancellationToken cancellationToken = default)
    {
        int rows = await dbContext.FlashcardRandomStudySessions
            .Where(f => f.FlashcardSetId == @event.FlashcardSetId && f.FlashcardId == @event.FlashcardId)
            .ExecuteDeleteAsync(cancellationToken);
        logger.LogInformation("Removed flashcard with id {flashcardId} from {rows} random study sessions", 
            @event.FlashcardId, rows);
    }
}