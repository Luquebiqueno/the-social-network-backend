using TheSocialNetwork.Application.UserProfiles.UpdateUserProfile;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.UnitTests.Application.TestDoubles;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.UpdateUserProfile;

[Collection(nameof(UpdateUserProfileTestFixture))]
public class UpdateUserProfileCommandHandlerTests(UpdateUserProfileTestFixture fixture)
{
    private readonly UpdateUserProfileTestFixture _fixture = fixture;
    private readonly FakeUserProfileRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldUpdateProfile_WhenCommandIsValid()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var profile = _fixture.GetSeededProfile(_repository);
        var command = _fixture.GetValidCommand(profile.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.DisplayName, profile.DisplayName.Value);
        Assert.Equal(command.Biography, profile.Biography.Value);
        Assert.Equal(command.AvatarUrl, profile.AvatarUrl.Value);
        Assert.Equal(1, _repository.UpdateCallCount);
        Assert.Equal(1, _unitOfWork.CommitCallCount);
        Assert.Equal(0, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCommandFailsValidation()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = new UpdateUserProfileCommand(Guid.Empty, "", null, null);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(0, _repository.UpdateCallCount);
        Assert.Equal(0, _unitOfWork.BeginTransactionCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenProfileDoesNotExist()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = new UpdateUserProfileCommand(
            _fixture.GetValidUserProfileId(), _fixture.GetValidDisplayName(), null, null);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.NotFound, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }
}
