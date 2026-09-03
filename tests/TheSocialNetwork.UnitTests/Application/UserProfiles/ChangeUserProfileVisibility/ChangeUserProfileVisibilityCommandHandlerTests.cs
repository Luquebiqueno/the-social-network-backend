using TheSocialNetwork.Application.UserProfiles.ChangeUserProfileVisibility;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.UnitTests.Application.TestDoubles;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.ChangeUserProfileVisibility;

[Collection(nameof(ChangeUserProfileVisibilityTestFixture))]
public class ChangeUserProfileVisibilityCommandHandlerTests(ChangeUserProfileVisibilityTestFixture fixture)
{
    private readonly ChangeUserProfileVisibilityTestFixture _fixture = fixture;
    private readonly FakeUserProfileRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldChangeVisibility_WhenCommandIsValid()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var profile = _fixture.GetSeededProfile(_repository);
        var command = new ChangeUserProfileVisibilityCommand(profile.Id, ProfileVisibility.Private);

        var result = await handler.Handle(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(ProfileVisibility.Private, profile.Visibility);
        Assert.Equal(1, _repository.UpdateCallCount);
        Assert.Equal(1, _unitOfWork.CommitCallCount);
        Assert.Equal(0, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCommandFailsValidation()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = new ChangeUserProfileVisibilityCommand(Guid.Empty, (ProfileVisibility)999);

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal(0, _repository.UpdateCallCount);
        Assert.Equal(0, _unitOfWork.BeginTransactionCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenProfileDoesNotExist()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = new ChangeUserProfileVisibilityCommand(_fixture.GetValidUserProfileId(), ProfileVisibility.Private);

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.NotFound, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }
}
