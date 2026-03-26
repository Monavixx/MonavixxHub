using FluentValidation;

namespace MonavixxHub.Api.Features.Auth.DTOs.Validation;

public class RegisterDtoValidator: AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(EmailCheckService emailCheckService)
    {
        RuleFor(r => r.Email)
            .Must(emailCheckService.Check)
            .WithMessage("Email address is not valid");
    }
}