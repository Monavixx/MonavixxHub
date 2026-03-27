using System.Net;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Common.Exceptions;

public class GlobalExceptionHandler
    (IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var dbContext = httpContext.RequestServices.GetService<AppDbContext>()!;
        if (exception is UniqueConstraintException uniqueConstraintException
            && uniqueConstraintException.MapToDomain(dbContext) is { } appBaseException)
            exception = appBaseException;
        
        var (code, message) = MapException(exception);
        Log(code, message);
        httpContext.Response.StatusCode = code;
        var problemDetails = new ProblemDetails()
        {
            Status = code,
            Title = message,
            Detail = GetSafeDetails(exception, httpContext)
        };
        
         await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
        {
            HttpContext =  httpContext,
            ProblemDetails = problemDetails
        });
        return true;
    }

    private static (int statusCode, string message) MapException(Exception exception)
        => exception switch
        {
            AppBaseException e => ((int)e.StatusCode, e.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occured")
        };

    private static string? GetSafeDetails(Exception exception, HttpContext httpContext)
    {
        if (httpContext.RequestServices.GetService<IHostEnvironment>()!.IsDevelopment())
        {
            return exception.Message;
        }
        return (exception is AppBaseException e ? e.Message : null);
    }

    private void Log(int statusCode, string message)
    {
        switch (statusCode)
        {
            case >= 500:
                logger.LogError("Status code {StatusCode}: {Message}", statusCode, message);
                break;
            case >= 400:
                logger.LogInformation("Status code {StatusCode}: {Message}", statusCode, message);
                break;
            default:
                logger.LogDebug("Status code {StatusCode}: {Message}", statusCode, message);
                break;
        }
    }
}