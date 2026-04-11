using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;

namespace MonavixxHub.Api.Common.Authorization;

public class CustomAuthorizationMiddlewareResultHandler(IProblemDetailsService problemDetailsService)
    : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
    
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            var reason = authorizeResult.AuthorizationFailure?.FailureReasons.FirstOrDefault()?.Message ??
                         "Access denied";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = reason
                },
                HttpContext = context,
            });
            return;
        }
        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}