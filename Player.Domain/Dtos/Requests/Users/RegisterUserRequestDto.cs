namespace Player.Domain.Dtos.Requests.Users;

public record RegisterUserRequestDto(string Email, string FirstName, string LastName, string Password);