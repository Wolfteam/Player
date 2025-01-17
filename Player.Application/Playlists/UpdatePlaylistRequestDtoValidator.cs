using FluentValidation;
using Player.Application.Validation;
using Player.Domain.Dtos.Requests.Playlists;

namespace Player.Application.Playlists;

internal class UpdatePlaylistRequestDtoValidator : AbstractValidator<UpdatePlaylistRequestDto>
{
    public UpdatePlaylistRequestDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .MaximumLength(100)
            .OnlyLetters();
    }
}