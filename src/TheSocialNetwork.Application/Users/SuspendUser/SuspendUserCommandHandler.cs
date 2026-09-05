using FluentValidation;
using TheSocialNetwork.Application.Abstractions;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;
using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.Application.Users.SuspendUser;

public sealed class SuspendUserCommandHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    IValidator<SuspendUserCommand> validator) : ICommandHandler<SuspendUserCommand, Result>
{
    private readonly IUserRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<SuspendUserCommand> _validator = validator;

    public async Task<Result> HandleAsync(
        SuspendUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Failure(validationResult.ToValidationError());

        var reasonResult = SuspensionReason.Create(command.Reason);
        if (reasonResult.IsFailure)
            return Result.Failure(reasonResult.Error);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var user = await _repository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure(UserErrors.NotFound);
        }

        var suspendResult = user.Suspend(reasonResult.Value, DateTimeOffset.UtcNow);
        if (suspendResult.IsFailure)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return suspendResult;
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
