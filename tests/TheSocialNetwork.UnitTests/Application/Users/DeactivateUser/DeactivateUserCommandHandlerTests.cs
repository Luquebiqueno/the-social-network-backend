using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Application.TestDoubles;

namespace TheSocialNetwork.UnitTests.Application.Users.DeactivateUser;

[Collection(nameof(DeactivateUserTestFixture))]
public class DeactivateUserCommandHandlerTests(DeactivateUserTestFixture fixture)
{
    private readonly DeactivateUserTestFixture _fixture = fixture;
    private readonly FakeUserRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldDeactivateUser_WhenCommandIsValid()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetSeededUser(_repository);
        var command = _fixture.GetValidCommand(user.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserStatus.Deactivated, user.Status);
        Assert.NotNull(user.DeactivatedAtUtc);
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
    public async Task Handle_ShouldFail_WhenUserIsAlreadyDeactivated()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetDeactivatedUser();
        _repository.Seed(user);
        var command = _fixture.GetValidCommand(user.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AlreadyDeactivated, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldClearSuspensionFields_WhenUserWasSuspended()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetSuspendedUser();
        _repository.Seed(user);
        var command = _fixture.GetValidCommand(user.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Null(user.SuspendedAtUtc);
        Assert.Null(user.SuspensionReason);
    }
}
