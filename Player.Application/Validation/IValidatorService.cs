using Player.Domain.Dtos;

namespace Player.Application.Validation;

public interface IValidatorService
{
    EmptyResultDto Validate<TRequest>(TRequest dto);
}