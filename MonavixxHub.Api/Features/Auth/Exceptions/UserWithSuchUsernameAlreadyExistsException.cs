using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class UserWithSuchUsernameAlreadyExistsException(Exception? inner = null) 
    : AppBaseException("This username is already being used", HttpStatusCode.Conflict);