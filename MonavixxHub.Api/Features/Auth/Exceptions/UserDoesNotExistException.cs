using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class UserDoesNotExistException()
    : AppBaseException ("User does not exist", HttpStatusCode.NotFound);