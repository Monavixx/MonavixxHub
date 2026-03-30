using System.Reflection;
using MonavixxHub.Api.Features.FlashcardsStudy.Algorithms;

namespace MonavixxHub.Api.Features.FlashcardsStudy;

public static class FlashcardsStudyExtensions
{
    public static IServiceCollection AddFlashcardsStudyAlgorithms(this IServiceCollection services)
    {
        foreach (var type in typeof(Program).Assembly.GetTypes()
                     .Where(t => t.IsDefined(typeof(ApiFlashcardStudyAlgorithmAttribute), false)
                                  && t.IsAssignableTo(typeof(IFlashcardStudyAlgorithm))))
        {
            services.AddKeyedScoped(typeof(IFlashcardStudyAlgorithm),
                type.GetCustomAttribute<ApiFlashcardStudyAlgorithmAttribute>()!.FlashcardStudyAlgorithm, type);
        }

        return services;
    }
}