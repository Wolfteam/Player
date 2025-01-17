using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Users;

namespace Player.Application.Users;

public interface IUserService
{
    Task<EmptyResultDto> Register(RegisterUserRequestDto dto);
    Task<EmptyResultDto> Login(LoginRequestDto dto);
}