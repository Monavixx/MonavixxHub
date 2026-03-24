namespace MonavixxHub.Api.Features.Auth.Exceptions;

public class WrongPasswordException(Exception? inner = null) : Exception("Wrong password", inner);