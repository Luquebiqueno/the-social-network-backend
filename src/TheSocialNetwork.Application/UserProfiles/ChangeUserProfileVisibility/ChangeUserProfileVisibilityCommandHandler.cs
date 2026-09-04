using FluentValidation;
using TheSocialNetwork.Application.Abstractions;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.UserProfiles.ChangeUserProfileVisibility;

public sealed class ChangeUserProfileVisibilityCommandHandler(
    IUserProfileRepository repository,
    IUnitOfWork unitOfWork,
    IValidator<ChangeUserProfileVisibilityCommand> validator) : ICommandHandler<ChangeUserProfileVisibilityCommand, Result>
{
    private readonly IUserProfileRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<ChangeUserProfileVisibilityCommand> _validator = validator;

    public async Task<Result> HandleAsync(
        ChangeUserProfileVisibilityCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Failure(validationResult.ToValidationError());

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var profile = await _repository.GetByIdAsync(command.UserProfileId, cancellationToken);
        if (profile is null)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure(UserProfileErrors.NotFound);
        }

        var changeResult = profile.ChangeVisibility(command.Visibility, DateTimeOffset.UtcNow);
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
