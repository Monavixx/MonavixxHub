using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Common.Exceptions;

public class GlobalExceptionHandler
    (IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger, UniqueConstraintResolver uniqueConstraintResolver) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var dbContext = httpContext.RequestServices.GetService<AppDbContext>()!;
        
        var (code, message) = MapException(exception, dbContext);
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

    private (int statusCode, string message) MapException(Exception exception, AppDbContext dbContext)
        => exception switch
        {
            AppBaseException e => ((int)e.StatusCode, e.Message),
            UniqueConstraintException e => (StatusCodes.Status409Conflict,
                uniqueConstraintResolver.Resolve(e,dbContext) ?? e.Message),
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