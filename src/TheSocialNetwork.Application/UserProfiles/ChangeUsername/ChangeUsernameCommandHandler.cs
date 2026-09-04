using FluentValidation;
using TheSocialNetwork.Application.Abstractions;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.Domain.SeedWork;

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
        CancellationToken cancellationToken = default
    )
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

        var isUsernameAvailable = await _repository.IsUsernameAvailableAsync(
            usernameResult.Value, profile.Id, cancellationToken);

        if (!isUsernameAvailable)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure(UserProfileErrors.UsernameAlreadyTaken);
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
