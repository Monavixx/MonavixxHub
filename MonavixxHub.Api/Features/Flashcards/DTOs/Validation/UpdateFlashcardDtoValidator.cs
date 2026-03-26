using FluentValidation;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Flashcards.DTOs.Validation;

public class UpdateFlashcardDtoValidator : AbstractValidator<UpdateFlashcardDto>
{
    public UpdateFlashcardDtoValidator()
    {
        RuleFor(x => x.Image)
            .ChildRules(rules => rules
                .RuleFor(x=>x!.Length)
                .LessThan(Flashcard.ImageMaxSize) // 5 MB
            ); 
    }
}