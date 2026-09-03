using FluentValidation;
using TheSocialNetwork.Application.Abstractions;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.UserProfiles.UpdateUserProfile;

public sealed class UpdateUserProfileCommandHandler(
    IUserProfileRepository repository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateUserProfileCommand> validator)
{
    private readonly IUserProfileRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<UpdateUserProfileCommand> _validator = validator;

    public async Task<Result> Handle(
        UpdateUserProfileCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Failure(validationResult.ToValidationError());

        var displayNameResult = DisplayName.Create(command.DisplayName);
        if (displayNameResult.IsFailure)
            return Result.Failure(displayNameResult.Error);

        var biographyResult = Biography.Create(command.Biography);
        if (biographyResult.IsFailure)
            return Result.Failure(biographyResult.Error);

        var avatarUrlResult = AvatarUrl.Create(command.AvatarUrl);
        if (avatarUrlResult.IsFailure)
            return Result.Failure(avatarUrlResult.Error);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var profile = await _repository.GetByIdAsync(command.UserProfileId, cancellationToken);
        if (profile is null)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure(UserProfileErrors.NotFound);
        }

        var profileUpdateResult = profile.Update(
            displayNameResult.Value,
            biographyResult.Value,
            avatarUrlResult.Value,
            DateTimeOffset.UtcNow);

        if (profileUpdateResult.IsFailure)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return profileUpdateResult;
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
