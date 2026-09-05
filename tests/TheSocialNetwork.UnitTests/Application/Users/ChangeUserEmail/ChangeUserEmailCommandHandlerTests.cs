using TheSocialNetwork.Application.Users.ChangeUserEmail;
using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Application.TestDoubles;

namespace TheSocialNetwork.UnitTests.Application.Users.ChangeUserEmail;

[Collection(nameof(ChangeUserEmailTestFixture))]
public class ChangeUserEmailCommandHandlerTests(ChangeUserEmailTestFixture fixture)
{
    private readonly ChangeUserEmailTestFixture _fixture = fixture;
    private readonly FakeUserRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldChangeEmail_WhenCommandIsValid()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetSeededUser(_repository);
        var command = _fixture.GetValidCommand(user.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.Email, user.Email.Value);
        Assert.Equal(1, _repository.UpdateCallCount);
        Assert.Equal(1, _unitOfWork.CommitCallCount);
        Assert.Equal(0, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCommandFailsValidation()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = new ChangeUserEmailCommand(_fixture.GetValidUserId(), "not-an-email");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(0, _unitOfWork.BeginTransactionCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserDoesNotExist()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = _fixture.GetValidCommand(_fixture.GetValidUserId());

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotFound, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEmailIsUnchanged()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetSeededUser(_repository);
        var command = new ChangeUserEmailCommand(user.Id, user.Email.Value);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.EmailUnchanged, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEmailIsAlreadyRegisteredToAnotherUser()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var existingUser = _fixture.GetValidUser();
        _repository.Seed(existingUser);
        var user = _fixture.GetSeededUser(_repository);
        var command = new ChangeUserEmailCommand(user.Id, existingUser.Email.Value);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.EmailAlreadyRegistered, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
        Assert.Equal(0, _unitOfWork.CommitCallCount);
    }
}
