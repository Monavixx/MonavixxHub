using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace MonavixxHub.Api.Common.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (code, message) = MapException(exception);
        httpContext.Response.StatusCode = code;
        await httpContext.Response.WriteAsync(message); // TODO: ProblemDetails
        return true;
    }

    private static (int statusCode, string message) MapException(Exception exception)
        => exception switch
        {
            AppBaseException e => ((int)e.StatusCode, e.Message),
            ArgumentNullException or ArgumentException =>
                (StatusCodes.Status400BadRequest, "Invalid argument provided"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occured")
        };
}