namespace Player.Domain.Dtos.Responses.Medias;

public record GetAllMediaResponseDto(long Id, long PlaylistId, string Name, float Length, float Duration);