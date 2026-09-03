using TheSocialNetwork.Application.UserProfiles.ChangeUserProfileVisibility;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.UnitTests.Application.TestDoubles;
using TheSocialNetwork.UnitTests.Domain.UserProfiles;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.ChangeUserProfileVisibility;

public class ChangeUserProfileVisibilityTestFixture : UserProfileTestFixture
{
    public ChangeUserProfileVisibilityTestFixture()
        : base() { }

    public ChangeUserProfileVisibilityCommand GetValidCommand(Guid userProfileId)
        => new(userProfileId, GetRandomProfileVisibility());

    public ChangeUserProfileVisibilityCommandHandler GetHandler(
        FakeUserProfileRepository repository, FakeUnitOfWork unitOfWork)
        => new(repository, unitOfWork, new ChangeUserProfileVisibilityCommandValidator());

    public UserProfile GetSeededProfile(FakeUserProfileRepository repository)
    {
        var profile = GetValidUserProfile();
        repository.Seed(profile);
        return profile;
    }
}

[CollectionDefinition(nameof(ChangeUserProfileVisibilityTestFixture))]
public class ChangeUserProfileVisibilityTestFixtureCollection
    : ICollectionFixture<ChangeUserProfileVisibilityTestFixture>
{ }
