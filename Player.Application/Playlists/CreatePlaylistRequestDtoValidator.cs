using FluentValidation;
using Player.Application.Validation;
using Player.Domain.Dtos.Requests.Playlists;

namespace Player.Application.Playlists;

internal class CreatePlaylistRequestDtoValidator : AbstractValidator<CreatePlaylistRequestDto>
{
    public CreatePlaylistRequestDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .MaximumLength(100)
            .OnlyLetters();
    }
}