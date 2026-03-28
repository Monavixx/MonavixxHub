using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Auth.Exceptions;

/// <summary>
/// Represents an exception that is thrown when user authentication fails
/// due to invalid credentials.
/// </summary>
/// <remarks>
/// This exception is raised when the provided username/email or password is incorrect.
/// It results in an HTTP 401 Unauthorized response.
/// </remarks>
public class WrongUserCredentialsException() 
    : AppBaseException("Wrong username/email or password", HttpStatusCode.Unauthorized);