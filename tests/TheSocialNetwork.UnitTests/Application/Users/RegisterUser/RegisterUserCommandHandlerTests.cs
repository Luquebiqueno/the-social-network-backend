using TheSocialNetwork.Application.Users.RegisterUser;
using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Application.TestDoubles;

namespace TheSocialNetwork.UnitTests.Application.Users.RegisterUser;

[Collection(nameof(RegisterUserTestFixture))]
public class RegisterUserCommandHandlerTests(RegisterUserTestFixture fixture)
{
    private readonly RegisterUserTestFixture _fixture = fixture;
    private readonly FakeUserRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldRegisterUserAndPersistIt_WhenCommandIsValid()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = _fixture.GetValidCommand();

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);

        var persisted = await _repository.GetByIdAsync(result.Value);
        Assert.NotNull(persisted);
        Assert.Equal(command.Email, persisted.Email.Value);
        Assert.Equal(UserStatus.PendingActivation, persisted.Status);
        Assert.Equal(1, _repository.AddCallCount);
        Assert.Equal(1, _unitOfWork.CommitCallCount);
        Assert.Equal(0, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCommandFailsValidation()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = new RegisterUserCommand("", "", "");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(0, _repository.AddCallCount);
        Assert.Equal(0, _unitOfWork.BeginTransactionCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEmailIsAlreadyRegistered()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var existingUser = _fixture.GetValidUser();
        _repository.Seed(existingUser);

        var command = new RegisterUserCommand(
            existingUser.Email.Value,
            _fixture.GetValidExternalIdentityProvider(),
            _fixture.GetValidExternalIdentitySubject());

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.EmailAlreadyRegistered, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
        Assert.Equal(0, _unitOfWork.CommitCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenExternalIdentityIsAlreadyRegistered()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var existingUser = _fixture.GetValidUser();
        _repository.Seed(existingUser);

        var command = new RegisterUserCommand(
            _fixture.GetValidEmail(),
            existingUser.ExternalIdentityProvider.Value,
            existingUser.ExternalIdentitySubject.Value);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.ExternalIdentityAlreadyRegistered, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
        Assert.Equal(0, _unitOfWork.CommitCallCount);
    }
}
