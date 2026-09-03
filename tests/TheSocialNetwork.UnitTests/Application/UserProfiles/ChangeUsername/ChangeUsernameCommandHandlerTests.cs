using TheSocialNetwork.Application.UserProfiles.ChangeUsername;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.UnitTests.Application.TestDoubles;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.ChangeUsername;

[Collection(nameof(ChangeUsernameTestFixture))]
public class ChangeUsernameCommandHandlerTests(ChangeUsernameTestFixture fixture)
{
    private readonly ChangeUsernameTestFixture _fixture = fixture;
    private readonly FakeUserProfileRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldChangeUsername_WhenCommandIsValid()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var profile = _fixture.GetSeededProfile(_repository);
        var command = _fixture.GetValidCommand(profile.Id);

        var result = await handler.Handle(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.NewUsername, profile.Username.Value);
        Assert.Equal(1, _repository.UpdateCallCount);
        Assert.Equal(1, _unitOfWork.CommitCallCount);
        Assert.Equal(0, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCommandFailsValidation()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = new ChangeUsernameCommand(Guid.Empty, "");

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal(0, _repository.UpdateCallCount);
        Assert.Equal(0, _unitOfWork.BeginTransactionCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenProfileDoesNotExist()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = _fixture.GetValidCommand(_fixture.GetValidUserProfileId());

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.NotFound, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNewUsernameIsAlreadyTakenByAnotherProfile()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var profile = _fixture.GetSeededProfile(_repository);
        var otherProfile = _fixture.GetValidUserProfile();
        _repository.Seed(otherProfile);

        var command = new ChangeUsernameCommand(profile.Id, otherProfile.Username.Value);

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.UsernameAlreadyTaken, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNewUsernameIsSameAsCurrentUsername()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var profile = _fixture.GetSeededProfile(_repository);
        var command = new ChangeUsernameCommand(profile.Id, profile.Username.Value);

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.UsernameUnchanged, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }
}
