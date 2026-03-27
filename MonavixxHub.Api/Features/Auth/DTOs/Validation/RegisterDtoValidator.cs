using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.DTOs.Validation;

public class RegisterDtoValidator: AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(Services.EmailCheckService emailCheckService)
    {
        RuleFor(r => r.Email)
            .Must(emailCheckService.IsValid)
            .WithMessage("Email address is not valid");
        RuleFor(reg => reg.Password)
            .Must(s => s.Any(char.IsDigit))
            .WithMessage("Password must contain at least 1 digit")
            .Must(s => s.Any(char.IsUpper))
            .WithMessage("Password must contain at least 1 uppercase letter")
            .Must(s => s.Any(char.IsLower))
            .WithMessage("Password must contain at least 1 lowercase letter")
            .Must(s => s.Any(char.IsPunctuation))
            .WithMessage("Password must contain at least 1 punctuation");
    }
}