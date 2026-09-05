using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Application.TestDoubles;

namespace TheSocialNetwork.UnitTests.Application.Users.ReactivateUser;

[Collection(nameof(ReactivateUserTestFixture))]
public class ReactivateUserCommandHandlerTests(ReactivateUserTestFixture fixture)
{
    private readonly ReactivateUserTestFixture _fixture = fixture;
    private readonly FakeUserRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldReactivateUser_WhenUserIsSuspended()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetSeededSuspendedUser(_repository);
        var command = _fixture.GetValidCommand(user.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserStatus.PendingActivation, user.Status);
        Assert.Null(user.SuspendedAtUtc);
        Assert.Null(user.SuspensionReason);
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
    public async Task Handle_ShouldFail_WhenUserIsNotSuspended()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetValidUser();
        _repository.Seed(user);
        var command = _fixture.GetValidCommand(user.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotSuspended, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }
}
