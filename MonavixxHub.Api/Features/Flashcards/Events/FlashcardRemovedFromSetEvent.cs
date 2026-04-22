using MonavixxHub.Api.Common.Events;

namespace MonavixxHub.Api.Features.Flashcards.Events;

public record FlashcardRemovedFromSetEvent (Guid FlashcardSetId, Guid FlashcardId) : IDomainEvent;