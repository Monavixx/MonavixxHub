using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class SessionNotFoundException() : AppBaseException("Session not found", HttpStatusCode.NotFound);