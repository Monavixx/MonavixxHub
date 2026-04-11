using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class InvalidEmailConfirmationTokenException()
    : AppBaseException("Invalid email confirmation token", HttpStatusCode.BadRequest);