using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Flashcards.Exceptions;

public class FlashcardSetNotFoundException() 
    : AppBaseException("FlashcardSet not found", HttpStatusCode.NotFound);