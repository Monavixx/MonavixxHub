namespace MonavixxHub.Api.Features.Flashcards.Exceptions;

public class FlashcardNotFoundException(Exception? inner = null) 
    : Exception("Flashcard not found", inner);