using TheSocialNetwork.Application.UserProfiles.UpdateUserProfile;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.UnitTests.Application.TestDoubles;
using TheSocialNetwork.UnitTests.Domain.UserProfiles;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.UpdateUserProfile;

public class UpdateUserProfileTestFixture : UserProfileTestFixture
{
    public UpdateUserProfileTestFixture()
        : base() { }

    public UpdateUserProfileCommand GetValidCommand(Guid userProfileId)
        => new(userProfileId, GetValidDisplayName(), GetValidBiography(), GetValidAvatarUrl());

    public UpdateUserProfileCommandHandler GetHandler(
        FakeUserProfileRepository repository, FakeUnitOfWork unitOfWork)
        => new(repository, unitOfWork, new UpdateUserProfileCommandValidator());

    public UserProfile GetSeededProfile(FakeUserProfileRepository repository)
    {
        var profile = GetValidUserProfile();
        repository.Seed(profile);
        return profile;
    }
}

[CollectionDefinition(nameof(UpdateUserProfileTestFixture))]
public class UpdateUserProfileTestFixtureCollection
    : ICollectionFixture<UpdateUserProfileTestFixture>
{ }
