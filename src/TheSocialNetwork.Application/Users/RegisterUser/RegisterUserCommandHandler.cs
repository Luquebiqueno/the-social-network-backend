using FluentValidation;
using TheSocialNetwork.Application.Abstractions;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;
using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.Application.Users.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    IValidator<RegisterUserCommand> validator) : ICommandHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IUserRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<RegisterUserCommand> _validator = validator;

    public async Task<Result<Guid>> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Failure<Guid>(validationResult.ToValidationError());

        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return Result.Failure<Guid>(emailResult.Error);

        var providerResult = ExternalIdentityProvider.Create(command.ExternalIdentityProvider);
        if (providerResult.IsFailure)
            return Result.Failure<Guid>(providerResult.Error);

        var subjectResult = ExternalIdentitySubject.Create(command.ExternalIdentitySubject);
        if (subjectResult.IsFailure)
            return Result.Failure<Guid>(subjectResult.Error);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var userResult = User.Register(emailResult.Value, providerResult.Value, subjectResult.Value);
        if (userResult.IsFailure)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure<Guid>(userResult.Error);
        }

        var addResult = await _repository.AddAsync(userResult.Value, cancellationToken);
        if (addResult.IsFailure)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure<Guid>(addResult.Error);
        }

        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success(userResult.Value.Id);
    }
}
