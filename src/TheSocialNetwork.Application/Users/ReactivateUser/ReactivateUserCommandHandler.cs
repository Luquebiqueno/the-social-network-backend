using FluentValidation;
using TheSocialNetwork.Application.Abstractions;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;
using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.Application.Users.ReactivateUser;

public sealed class ReactivateUserCommandHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    IValidator<ReactivateUserCommand> validator) : ICommandHandler<ReactivateUserCommand, Result>
{
    private readonly IUserRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<ReactivateUserCommand> _validator = validator;

    public async Task<Result> HandleAsync(
        ReactivateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Failure(validationResult.ToValidationError());

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var user = await _repository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure(UserErrors.NotFound);
        }

        var reactivateResult = user.Reactivate(DateTimeOffset.UtcNow);
        if (reactivateResult.IsFailure)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return reactivateResult;
        }

        var updateResult = await _repository.UpdateAsync(user, cancellationToken);
        if (updateResult.IsFailure)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return updateResult;
        }

        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
