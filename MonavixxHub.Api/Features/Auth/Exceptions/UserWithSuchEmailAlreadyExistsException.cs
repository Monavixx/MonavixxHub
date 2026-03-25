using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class UserWithSuchEmailAlreadyExistsException()
    : AppBaseException("This email is already being used", HttpStatusCode.Conflict);