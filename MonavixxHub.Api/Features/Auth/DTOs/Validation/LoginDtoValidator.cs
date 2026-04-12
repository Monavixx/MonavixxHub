using FluentValidation;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Features.Auth.Services;

namespace MonavixxHub.Api.Features.Auth.DTOs.Validation;

/// <summary>
/// Validates <see cref="LoginDto"/> using FluentValidation rules
/// that are not covered by standard Data Annotations.
/// </summary>
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    /// <inheritdoc />
    public LoginDtoValidator(IEmailCheckService emailCheckService)
    {
        RuleFor(d => d.UsernameOrEmail)
            .MinimumLength(User.UsernameMinLength)
            .WithMessage($"Username must contain at least {User.UsernameMinLength} characters.")
            .When(d => !emailCheckService.IsValid(d.UsernameOrEmail));
    }
}