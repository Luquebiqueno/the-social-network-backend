using TheSocialNetwork.Application.UserProfiles.CreateUserProfile;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.UnitTests.Application.TestDoubles;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.CreateUserProfile;

[Collection(nameof(CreateUserProfileTestFixture))]
public class CreateUserProfileCommandHandlerTests(CreateUserProfileTestFixture fixture)
{
    private readonly CreateUserProfileTestFixture _fixture = fixture;
    private readonly FakeUserProfileRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldCreateProfileAndPersistIt_WhenCommandIsValid()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = _fixture.GetValidCommand();

        var result = await handler.Handle(command);

        Assert.True(result.IsSuccess);

        var persisted = await _repository.GetByUserIdAsync(command.UserId);
        Assert.NotNull(persisted);
        Assert.Equal(persisted.Id, result.Value);
        Assert.Equal(command.Username, persisted.Username.Value);
        Assert.Equal(1, _repository.AddCallCount);
        Assert.Equal(1, _unitOfWork.CommitCallCount);
        Assert.Equal(0, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCommandFailsValidation()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = new CreateUserProfileCommand(Guid.Empty, "", "");

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal(0, _repository.AddCallCount);
        Assert.Equal(0, _unitOfWork.BeginTransactionCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenProfileAlreadyExistsForUser()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var existingProfile = _fixture.GetValidUserProfile();
        _repository.Seed(existingProfile);

        var command = new CreateUserProfileCommand(
            existingProfile.UserId, _fixture.GetValidUsername(), _fixture.GetValidDisplayName());

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.AlreadyExists, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
        Assert.Equal(0, _unitOfWork.CommitCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUsernameIsAlreadyTaken()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var existingProfile = _fixture.GetValidUserProfile();
        _repository.Seed(existingProfile);

        var command = new CreateUserProfileCommand(
            _fixture.GetValidUserId(), existingProfile.Username.Value, _fixture.GetValidDisplayName());

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.UsernameAlreadyTaken, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
        Assert.Equal(0, _unitOfWork.CommitCallCount);
    }
}
