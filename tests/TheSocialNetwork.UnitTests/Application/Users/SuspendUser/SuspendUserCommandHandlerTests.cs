using TheSocialNetwork.Application.Users.SuspendUser;
using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Application.TestDoubles;

namespace TheSocialNetwork.UnitTests.Application.Users.SuspendUser;

[Collection(nameof(SuspendUserTestFixture))]
public class SuspendUserCommandHandlerTests(SuspendUserTestFixture fixture)
{
    private readonly SuspendUserTestFixture _fixture = fixture;
    private readonly FakeUserRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldSuspendUser_WhenCommandIsValid()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetSeededUser(_repository);
        var command = _fixture.GetValidCommand(user.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserStatus.Suspended, user.Status);
        Assert.Equal(command.Reason, user.SuspensionReason?.Value);
        Assert.Equal(1, _repository.UpdateCallCount);
        Assert.Equal(1, _unitOfWork.CommitCallCount);
        Assert.Equal(0, _unitOfWork.RollbackCallCount);
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
    public async Task Handle_ShouldFail_WhenUserIsAlreadySuspended()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetSuspendedUser();
        _repository.Seed(user);
        var command = _fixture.GetValidCommand(user.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AlreadySuspended, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserIsDeactivated()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetDeactivatedUser();
        _repository.Seed(user);
        var command = _fixture.GetValidCommand(user.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.CannotSuspendDeactivatedUser, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }
}
