using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class SessionExpiredException()
    : AppBaseException("Session expired. Please, log in.", HttpStatusCode.Unauthorized);