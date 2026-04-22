using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MonavixxHub.Api.Features.Flashcards.Services;

namespace MonavixxHub.Api.Features.Flashcards.Authorization.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class FlashcardSetAuthorizationFilterAttribute(FlashcardSetAccessType accessType) : Attribute, IAsyncActionFilter
{
    public string FlashcardSetIdArgName { get; set; } = "flashcardSetId";
    public string IncludeEntriesArgName { get; set; } = "includeEntries";
    public string IncludeSubsetsArgName { get; set; } = "includeSubsets";
    public IncludeVariant IncludeEntries { get; set; } = IncludeVariant.Argument;
    public IncludeVariant IncludeSubsets { get; set; } = IncludeVariant.Argument;

    public static readonly object FlashcardSetKey = new();
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>()!;
        var flashcardSetService = context.HttpContext.RequestServices.GetService<IFlashcardSetService>()!;
        if (!context.ActionArguments.TryGetValue(FlashcardSetIdArgName, out var flashcardSetIdObj) ||
            flashcardSetIdObj is not Guid flashcardSetId)
            throw new ArgumentException($"Action argument '{FlashcardSetIdArgName}' is missing or not a Guid.");

        bool includeEntries = IncludeEntries switch
        {
            IncludeVariant.Include => true,
            IncludeVariant.Exclude => false,
            IncludeVariant.Argument => context.ActionArguments.TryGetValue(IncludeEntriesArgName,
                                           out var includeEntriesObj) &&
                                       includeEntriesObj is true,
            _ => throw new ArgumentOutOfRangeException(nameof(IncludeEntries))
        };

        bool includeSubsets = IncludeSubsets switch
        {
            IncludeVariant.Include => true,
            IncludeVariant.Exclude => false,
            IncludeVariant.Argument => !context.ActionArguments.TryGetValue(IncludeSubsetsArgName,
                                           out var includeSubsetsObj) ||
                                       includeSubsetsObj is true,
            _ => throw new ArgumentOutOfRangeException(nameof(IncludeSubsets))
        };

        var flashcardSet = await flashcardSetService.GetAsync(flashcardSetId, includeEntries: includeEntries,
            includeSubsets: includeSubsets, thenIncludeFlashcard: includeEntries);
        context.HttpContext.Items[FlashcardSetKey] = flashcardSet;

        var res = await authorizationService.AuthorizeAsync(context.HttpContext.User, flashcardSet,
            FlashcardSetAccessRequirement.Resolve(accessType));
        if (res.Succeeded)
        {
            await next();
            return;
        }

        context.Result = new ForbidResult();
    }

}