using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MonavixxHub.Api.Features.Flashcards.Services;

namespace MonavixxHub.Api.Features.Flashcards.Authorization.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class FlashcardAuthorizationFilterAttribute (FlashcardAccessType accessType) : Attribute, IAsyncActionFilter
{
    public string FlashcardIdName { get; set; } = "flashcardId";
    public static readonly object FlashcardKey = new();
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>()!;
        var flashcardService = context.HttpContext.RequestServices.GetService<IFlashcardService>()!;
        if (!context.ActionArguments.TryGetValue(FlashcardIdName, out var flashcardIdObj) ||
            flashcardIdObj is not Guid flashcardId)
            throw new ArgumentException($"Action argument '{FlashcardIdName}' is missing or not a Guid.");

        var flashcard = await flashcardService.GetAsync(flashcardId);
        var res = await authorizationService.AuthorizeAsync(context.HttpContext.User, flashcard,
            FlashcardAccessRequirement.Resolve(accessType));
        if (res.Succeeded)
        {
            context.HttpContext.Items[FlashcardKey] = flashcard;
            await next();
            return;
        }
        context.Result = new ForbidResult();
    }
}