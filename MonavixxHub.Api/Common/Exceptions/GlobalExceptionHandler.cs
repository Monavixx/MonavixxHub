using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Common.Exceptions.Resolvers;
using MonavixxHub.Api.Infrastructure;
using Npgsql;

namespace MonavixxHub.Api.Common.Exceptions;

public class GlobalExceptionHandler
    (IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger, UniqueConstraintResolver uniqueConstraintResolver,
        ForeignKeyConstraintResolver foreignKeyConstraintResolver) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var dbContext = httpContext.RequestServices.GetService<AppDbContext>()!;

        var (code, message) = MapException(exception, dbContext);
        Log(code, message, exception);
        httpContext.Response.StatusCode = code;
        var problemDetails = new ProblemDetails()
        {
            Status = code,
            Title = message,
            Detail = GetSafeDetails(exception, httpContext)
        };

        await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
        return true;
    }

    private (int statusCode, string message) MapException(Exception exception, AppDbContext dbContext)
        => exception switch
        {
            AppBaseException e => ((int)e.StatusCode, e.Message),
            DbUpdateException { InnerException: PostgresException { ConstraintName: not null } pe } =>
                pe.SqlState switch
                {
                    PostgresErrorCodes.UniqueViolation =>
                        (StatusCodes.Status409Conflict, uniqueConstraintResolver.Resolve(pe, dbContext) ?? pe.Message),
                    PostgresErrorCodes.ForeignKeyViolation =>
                        (StatusCodes.Status422UnprocessableEntity,
                            foreignKeyConstraintResolver.Resolve(pe, dbContext) ?? pe.Message),
                    _ => (StatusCodes.Status500InternalServerError,
                        "An unexpected postgres error occured: [" + pe.SqlState + "] " + pe.Message)
                },
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occured: " + exception)
        };

    private static string? GetSafeDetails(Exception exception, HttpContext httpContext)
    {
        if (httpContext.RequestServices.GetService<IHostEnvironment>()!.IsDevelopment())
        {
            return exception.InnerException?.Message ?? exception.Message;
        }
        return (exception is AppBaseException e ? e.Message : null);
    }

    private void Log(int statusCode, string message, Exception exception)
    {
        switch (statusCode)
        {
            case >= 500:
                logger.LogError("[Code {StatusCode}] {ExceptionClass}: {Message}", statusCode,
                    exception.GetType().Name, message);
                break;
            case >= 400:
                logger.LogInformation("[Code {StatusCode}] {ExceptionClass}: {Message}", statusCode,
                    exception.GetType().Name, message);
                break;
            default:
                logger.LogDebug("[Code {StatusCode}] {ExceptionClass}: {Message}", statusCode,
                    exception.GetType().Name, message);
                break;
        }
    }
}