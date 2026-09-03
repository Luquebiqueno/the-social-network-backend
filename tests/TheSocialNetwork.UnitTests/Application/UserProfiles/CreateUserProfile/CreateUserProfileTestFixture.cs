using TheSocialNetwork.Application.UserProfiles.CreateUserProfile;
using TheSocialNetwork.UnitTests.Application.TestDoubles;
using TheSocialNetwork.UnitTests.Domain.UserProfiles;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.CreateUserProfile;

public class CreateUserProfileTestFixture : UserProfileTestFixture
{
    public CreateUserProfileTestFixture()
        : base() { }

    public CreateUserProfileCommand GetValidCommand()
        => new(GetValidUserId(), GetValidUsername(), GetValidDisplayName());

    public CreateUserProfileCommandHandler GetHandler(
        FakeUserProfileRepository repository, FakeUnitOfWork unitOfWork)
        => new(repository, unitOfWork, new CreateUserProfileCommandValidator());
}

[CollectionDefinition(nameof(CreateUserProfileTestFixture))]
public class CreateUserProfileTestFixtureCollection : ICollectionFixture<CreateUserProfileTestFixture>
{ }
