namespace MonavixxHub.Api.Features.Flashcards.Exceptions;

public class UserDoesNotHaveFlashcardWithSuchId(Exception? inner = null) 
    : Exception("User doesn't have Flashcard with such Id", inner);