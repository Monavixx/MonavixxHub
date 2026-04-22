using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Flashcards.Exceptions;

public class FlashcardNotFoundInSetException() 
    : AppBaseException("Flashcard not found in set", HttpStatusCode.NotFound);