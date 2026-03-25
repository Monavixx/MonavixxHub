using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class WrongUsernameOrPasswordException() 
    : AppBaseException("Wrong username or password", HttpStatusCode.Unauthorized);