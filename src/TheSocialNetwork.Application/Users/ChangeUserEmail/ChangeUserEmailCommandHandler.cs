using FluentValidation;
using TheSocialNetwork.Application.Abstractions;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;
using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.Application.Users.ChangeUserEmail;

public sealed class ChangeUserEmailCommandHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    IValidator<ChangeUserEmailCommand> validator) : ICommandHandler<ChangeUserEmailCommand, Result>
{
    private readonly IUserRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<ChangeUserEmailCommand> _validator = validator;

    public async Task<Result> HandleAsync(
        ChangeUserEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Failure(validationResult.ToValidationError());

        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return Result.Failure(emailResult.Error);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var user = await _repository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure(UserErrors.NotFound);
        }

        var changeResult = user.ChangeEmail(emailResult.Value);
        if (changeResult.IsFailure)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return changeResult;
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
