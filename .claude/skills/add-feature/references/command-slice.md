# Command Slice Templates

Files go in `src/TheSocialNetwork.Application/{Feature}/{UseCase}/`. Replace `{Feature}` (plural, e.g. `UserProfiles`), `{Entity}`, and use-case names throughout. Mirror `UserProfiles/ChangeUsername/` for the closest real example.

## Command

```csharp
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.UserProfiles.ChangeUsername;

public sealed record ChangeUsernameCommand(
    Guid UserProfileId,
    string NewUsername) : ICommand<Result>;
```

For a command that returns data, wrap it in `Result<T>` — don't return the raw value:

```csharp
public sealed record CreateUserProfileCommand(
    Guid UserId,
    string Username,
    string DisplayName) : ICommand<Result<Guid>>;
```

This matters because `ICommandHandler<TCommand, TResult>.HandleAsync` returns `Task<TResult>` verbatim — there's no unwrapping step. Making `TResult` be `Result` or `Result<Guid>` (instead of `Guid`) is what lets the handler report failures instead of only successes.

## Validator

Public class, same folder, `AbstractValidator<TCommand>` from FluentValidation. **Nothing calls this automatically** — the handler calls `ValidateAsync` itself (see below), and `Application/DependencyInjection.cs` must register it as `IValidator<TCommand>`.

```csharp
using FluentValidation;
using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.Application.UserProfiles.ChangeUsername;

public sealed class ChangeUsernameCommandValidator : AbstractValidator<ChangeUsernameCommand>
{
    public ChangeUsernameCommandValidator()
    {
        RuleFor(c => c.UserProfileId)
            .NotEmpty();

        RuleFor(c => c.NewUsername)
            .Must(value => Username.Create(value).IsSuccess)
            .WithMessage(UserProfileErrors.InvalidUsername.Description);
    }
}
```

Reuse the value object's own `Create(...)` for shape validation (`Must(value => Username.Create(value).IsSuccess)`) instead of re-implementing length/format rules in the validator.

## Handler

`public sealed`, primary constructor, repository + `IUnitOfWork` for data access, `IValidator<TCommand>` injected for the manual validation step. Guard clauses return `Result.Failure`/`Result.Failure<T>` with Domain errors; the happy path mutates, persists, and returns success.

```csharp
using FluentValidation;
using TheSocialNetwork.Application.Abstractions;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;
using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.Application.UserProfiles.ChangeUsername;

public sealed class ChangeUsernameCommandHandler(
    IUserProfileRepository repository,
    IUnitOfWork unitOfWork,
    IValidator<ChangeUsernameCommand> validator) : ICommandHandler<ChangeUsernameCommand, Result>
{
    private readonly IUserProfileRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<ChangeUsernameCommand> _validator = validator;

    public async Task<Result> HandleAsync(
        ChangeUsernameCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Failure(validationResult.ToValidationError());

        var usernameResult = Username.Create(command.NewUsername);
        if (usernameResult.IsFailure)
            return Result.Failure(usernameResult.Error);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var profile = await _repository.GetByIdAsync(command.UserProfileId, cancellationToken);
        if (profile is null)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure(UserProfileErrors.NotFound);
        }

        var changeResult = profile.ChangeUsername(usernameResult.Value, DateTimeOffset.UtcNow);
        if (changeResult.IsFailure)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return changeResult;
        }

        var updateResult = await _repository.UpdateAsync(profile, cancellationToken);
        if (updateResult.IsFailure)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return updateResult;
        }

        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
```

Notes:
- Order inside `HandleAsync`: validate the command shape → parse/create value objects → begin transaction → load the aggregate (404 if missing) → apply the domain mutation → persist → commit. Roll back on every failure branch after `BeginTransactionAsync`.
- Timestamps: pass `DateTimeOffset.UtcNow` directly to the entity method that needs one — there's no `IDateTimeProvider` in this codebase.
- No domain events, no cache invalidation — this codebase doesn't have that infrastructure yet.
- For a command returning a value (`ICommand<Result<Guid>>`), the handler is `ICommandHandler<TCommand, Result<Guid>>` and returns `Result.Success(value)`/`Result.Failure<Guid>(error)` — see `CreateUserProfileCommandHandler`.

## DI registration

Add both lines to `src/TheSocialNetwork.Application/DependencyInjection.cs` inside `AddApplication`:

```csharp
services.AddScoped<IValidator<ChangeUsernameCommand>, ChangeUsernameCommandValidator>();
services.AddScoped<ICommandHandler<ChangeUsernameCommand, Result>, ChangeUsernameCommandHandler>();
```

## Domain additions (if needed)

Error factory on the existing `{Entity}Errors` class in `src/TheSocialNetwork.Domain/{Feature}/`:

```csharp
public static readonly Error UsernameAlreadyTaken = Error.Conflict(
    "UserProfile.UsernameAlreadyTaken",
    "The username is already taken.");
```

Error type → HTTP status (via `CustomResults.Problem`): `NotFound` → 404, `Conflict` → 409, `Problem`/`Validation` → 400, `Failure` → 500.
