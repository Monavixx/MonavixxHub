using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Features.Auth.Services;

namespace MonavixxHub.Api.Features.Auth.DTOs.Validation;

/// <summary>
/// Validates <see cref="RegisterDto"/> using FluentValidation rules
/// that are not covered by standard Data Annotations.
/// Includes additional validation for:
/// <list type="bullet">
/// <item>Email format using <see cref="EmailCheckService"/></item>
/// <item>Password complexity (digit, uppercase, lowercase, punctuation)</item>
/// </list>
/// </summary>
public class RegisterDtoValidator: AbstractValidator<RegisterDto>
{
    /// <inheritdoc />
    public RegisterDtoValidator(EmailCheckService emailCheckService)
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