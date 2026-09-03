using FluentValidation.Results;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.Abstractions;

public static class ValidationResultExtensions
{
    public static ValidationError ToValidationError(this ValidationResult validationResult) =>
        new(validationResult.Errors
            .Select(failure => Error.Validation(failure.PropertyName, failure.ErrorMessage))
            .ToArray());
}
