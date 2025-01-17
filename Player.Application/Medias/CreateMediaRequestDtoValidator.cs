using FluentValidation;
using Player.Application.Validation;
using Player.Domain.Dtos.Requests.Medias;

namespace Player.Application.Medias;

internal class CreateMediaRequestDtoValidator : AbstractValidator<CreateMediaRequestDto>
{
    public CreateMediaRequestDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .Must(name => Path.GetExtension(name).Equals(".wav", StringComparison.OrdinalIgnoreCase))
            .WithGlobalErrorCode();

        RuleFor(dto => dto.Data)
            .NotEmpty()
            .Must(source => WavFormat.IsWav(source, out _))
            .WithGlobalErrorCode();
    }
}