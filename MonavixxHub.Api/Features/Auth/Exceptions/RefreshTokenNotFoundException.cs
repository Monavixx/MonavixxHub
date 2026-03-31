using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class RefreshTokenNotFoundException() 
    : AppBaseException("Refresh token not found. Please, log in.", HttpStatusCode.Unauthorized);