using FluentValidation;
using FluentValidation.Internal;
using Player.Domain;
using Player.Domain.Enums;
using Player.Domain.Extensions;

namespace Player.Application.Validation;

internal static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithGlobalErrorMsgAndCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        string errorMsg,
        string errorCode)
    {
        return rule.Configure(c =>
        {
            foreach (var ruleComponent in c.Components)
            {
                var component = (RuleComponent<T, TProperty>)ruleComponent;
                component.SetErrorMessage(errorMsg);
                component.ErrorCode = errorCode;
            }
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithGlobalErrorCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        string errorCode)
    {
        return rule.Configure(c =>
        {
            foreach (var ruleComponent in c.Components)
            {
                var component = (RuleComponent<T, TProperty>)ruleComponent;
                component.ErrorCode = errorCode;
            }
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithGlobalErrorCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        AppMessageType error = AppMessageType.InvalidRequest)
    {
        return rule.WithGlobalErrorCode(error.GetErrorCode());
    }

    public static IRuleBuilderOptions<T, TProperty> WithGlobalErrorMsgAndCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        string errorMsg,
        AppMessageType error = AppMessageType.InvalidRequest)
    {
        return rule.WithGlobalErrorMsgAndCode(errorMsg, error.GetErrorCode());
    }

    public static IRuleBuilder<T, string> OnlyLetters<T>(this IRuleBuilder<T, string> rule)
    {
        return rule.NotEmpty()
            .Matches("^[a-zA-Z]+$")
            .WithGlobalErrorCode();
    }

    public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> rule)
    {
        return rule.NotEmpty()
            .MinimumLength(8)
            .MaximumLength(16)
            .Matches(AppConstants.PasswordRegex)
            .WithGlobalErrorCode();;
    }
}