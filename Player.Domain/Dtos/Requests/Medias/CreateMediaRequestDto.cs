namespace Player.Domain.Dtos.Requests.Medias;

public record CreateMediaRequestDto(string Name, Stream Data);