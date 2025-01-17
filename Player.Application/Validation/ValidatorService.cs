using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Player.Domain.Dtos;
using Player.Domain.Utils;

namespace Player.Application.Validation;

public class ValidatorService : IValidatorService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ValidatorService> _logger;

    public ValidatorService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = loggerFactory.CreateLogger<ValidatorService>();
    }


    public EmptyResultDto Validate<TRequest>(TRequest dto)
    {
        Check.NotNull(dto, nameof(dto));

        Type type = dto!.GetType();
        _logger.LogInformation("Validating request of type = {Type}...", type);
        IValidator<TRequest> validator = _serviceProvider.GetService<IValidator<TRequest>>() ??
                                         throw new InvalidOperationException($"Did you forget to add a validator for = {type} ?");

        ValidationResult validationResult = validator.Validate(dto);
        if (validationResult.IsValid)
        {
            return EmptyResult.Success();
        }

        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName, e => new { e.ErrorMessage, e.ErrorCode })
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray())
            .SelectMany(e => e.Value)
            .ToArray();

        string errorMsg =
            $"{Environment.NewLine}Errors: {string.Join(Environment.NewLine, errors.Select(s => s.ErrorMessage))}";
        _logger.LogInformation("Validation failed with error = {Error}", errorMsg);
        return EmptyResult.InvalidRequest(errorMsg);
    }
}