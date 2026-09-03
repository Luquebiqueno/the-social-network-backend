using TheSocialNetwork.Application.UserProfiles.ChangeUsername;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.UnitTests.Application.TestDoubles;
using TheSocialNetwork.UnitTests.Domain.UserProfiles;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.ChangeUsername;

public class ChangeUsernameTestFixture : UserProfileTestFixture
{
    public ChangeUsernameTestFixture()
        : base() { }

    public ChangeUsernameCommand GetValidCommand(Guid userProfileId)
        => new(userProfileId, GetValidUsername());

    public ChangeUsernameCommandHandler GetHandler(FakeUserProfileRepository repository, FakeUnitOfWork unitOfWork)
        => new(repository, unitOfWork, new ChangeUsernameCommandValidator());

    public UserProfile GetSeededProfile(FakeUserProfileRepository repository)
    {
        var profile = GetValidUserProfile();
        repository.Seed(profile);
        return profile;
    }
}

[CollectionDefinition(nameof(ChangeUsernameTestFixture))]
public class ChangeUsernameTestFixtureCollection : ICollectionFixture<ChangeUsernameTestFixture>
{ }
