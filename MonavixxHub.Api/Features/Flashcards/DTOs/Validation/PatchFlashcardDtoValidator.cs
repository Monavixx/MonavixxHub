using FluentValidation;
using MonavixxHub.Api.Features.Flashcards.DTOs.Request;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs.Validation;

public class PatchFlashcardDtoValidator : AbstractValidator<PatchFlashcardDto>
{
    public PatchFlashcardDtoValidator()
    {
        RuleFor(x => x.Image)
            .ChildRules(rules => rules
                .RuleFor(x=>x!.Length)
                .LessThan(Flashcard.ImageMaxSize) // 5 MB
            ); 
    }
}