using FluentValidation;
using Player.Application.Validation;
using Player.Domain;
using Player.Domain.Dtos.Requests.Users;

namespace Player.Application.Users;

internal class RegisterUserRequestDtoValidator : AbstractValidator<RegisterUserRequestDto>
{
    public RegisterUserRequestDtoValidator()
    {
        RuleFor(dto => dto.Email)
            .NotEmpty()
            .EmailAddress()
            .WithGlobalErrorCode();

        RuleFor(dto => dto.FirstName)
            .OnlyLetters()
            .MaximumLength(100)
            .WithGlobalErrorCode();

        RuleFor(dto => dto.LastName)
            .OnlyLetters()
            .MaximumLength(100)
            .WithGlobalErrorCode();

        RuleFor(dto => dto.Password)
            .Password();
    }
}