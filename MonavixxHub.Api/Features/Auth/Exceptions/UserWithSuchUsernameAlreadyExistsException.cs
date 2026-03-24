namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class UserWithSuchUsernameAlreadyExistsException(Exception? inner = null) 
    : Exception($"This username is already being used", inner);