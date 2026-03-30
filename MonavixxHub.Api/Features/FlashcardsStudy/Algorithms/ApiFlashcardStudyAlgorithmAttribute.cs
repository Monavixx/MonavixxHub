namespace MonavixxHub.Api.Features.FlashcardsStudy.Algorithms;

[AttributeUsage(AttributeTargets.Class)]
public class ApiFlashcardStudyAlgorithmAttribute (FlashcardStudyAlgorithm flashcardStudyAlgorithm): Attribute
{
    public FlashcardStudyAlgorithm FlashcardStudyAlgorithm => flashcardStudyAlgorithm;
}