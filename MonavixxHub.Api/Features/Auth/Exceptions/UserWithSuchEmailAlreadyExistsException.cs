namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class UserWithSuchEmailAlreadyExistsException(Exception? inner = null)
    : Exception($"Email is already being used", inner);