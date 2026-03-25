using System.Net;

namespace MonavixxHub.Api.Common.Exceptions;

public abstract class AppBaseException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public AppBaseException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
    
}