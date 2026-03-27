namespace MonavixxHub.Api.Common.Exceptions;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class UniqueDomainExceptionAttribute(Type exceptionType)
    : Attribute
{
    public AppBaseException? GetException() => Activator.CreateInstance(exceptionType) as AppBaseException;
}