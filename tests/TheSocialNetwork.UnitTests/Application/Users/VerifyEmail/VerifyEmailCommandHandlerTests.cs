using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Application.TestDoubles;

namespace TheSocialNetwork.UnitTests.Application.Users.VerifyEmail;

[Collection(nameof(VerifyEmailTestFixture))]
public class VerifyEmailCommandHandlerTests(VerifyEmailTestFixture fixture)
{
    private readonly VerifyEmailTestFixture _fixture = fixture;
    private readonly FakeUserRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldVerifyEmail_WhenCommandIsValid()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetSeededUser(_repository);
        var command = _fixture.GetValidCommand(user.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.True(user.IsEmailVerified);
        Assert.Equal(UserStatus.Active, user.Status);
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
    public async Task Handle_ShouldFail_WhenEmailIsAlreadyVerified()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var user = _fixture.GetSeededUser(_repository);
        user.VerifyEmail(DateTimeOffset.UtcNow);
        var command = _fixture.GetValidCommand(user.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.EmailAlreadyVerified, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }
}
