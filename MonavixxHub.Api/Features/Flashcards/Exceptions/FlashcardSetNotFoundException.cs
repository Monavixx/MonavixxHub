namespace MonavixxHub.Api.Features.Flashcards.Exceptions;

public class FlashcardSetNotFoundException(Exception? inner = null) : Exception("FlashcardSet not found", inner);