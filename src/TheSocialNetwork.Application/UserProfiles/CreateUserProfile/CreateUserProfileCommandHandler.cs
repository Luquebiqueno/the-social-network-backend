using FluentValidation;
using TheSocialNetwork.Application.Abstractions;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.UserProfiles.CreateUserProfile;

public sealed class CreateUserProfileCommandHandler(
    IUserProfileRepository repository,
    IUnitOfWork unitOfWork,
    IValidator<CreateUserProfileCommand> validator) : ICommandHandler<CreateUserProfileCommand, Result<Guid>>
{
    private readonly IUserProfileRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<CreateUserProfileCommand> _validator = validator;

    public async Task<Result<Guid>> HandleAsync(
        CreateUserProfileCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Failure<Guid>(validationResult.ToValidationError());

        var usernameResult = Username.Create(command.Username);
        if (usernameResult.IsFailure)
            return Result.Failure<Guid>(usernameResult.Error);

        var displayNameResult = DisplayName.Create(command.DisplayName);
        if (displayNameResult.IsFailure)
            return Result.Failure<Guid>(displayNameResult.Error);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var profileResult = UserProfile.Create(
            command.UserId,
            usernameResult.Value,
            displayNameResult.Value
        );

        if (profileResult.IsFailure)
            return Result.Failure<Guid>(profileResult.Error);

        var addResult = await _repository.AddAsync(profileResult.Value, cancellationToken);
        if (addResult.IsFailure)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure<Guid>(addResult.Error);
        }

        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success(profileResult.Value.Id);
    }
}
