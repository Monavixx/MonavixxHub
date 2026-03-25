using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Flashcards.Exceptions;

public class FlashcardAlreadyInSetException()
: AppBaseException("Flashcard already in set", HttpStatusCode.Conflict);