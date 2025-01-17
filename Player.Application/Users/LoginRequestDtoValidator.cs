using FluentValidation;
using Player.Application.Validation;
using Player.Domain.Dtos.Requests.Users;

namespace Player.Application.Users;

internal class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(dto => dto.Email)
            .NotEmpty()
            .EmailAddress()
            .WithGlobalErrorCode();

        RuleFor(dto => dto.Password)
            .Password();
    }
}