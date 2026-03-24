namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class UserDoesNotExistException (Exception? inner = null)
    : Exception ($"User does not exist", inner);