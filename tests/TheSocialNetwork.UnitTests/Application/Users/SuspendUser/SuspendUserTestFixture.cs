using TheSocialNetwork.Application.Users.SuspendUser;
using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Application.TestDoubles;
using TheSocialNetwork.UnitTests.Domain.Users;

namespace TheSocialNetwork.UnitTests.Application.Users.SuspendUser;

public class SuspendUserTestFixture : UserTestFixture
{
    public SuspendUserTestFixture()
        : base() { }

    public SuspendUserCommand GetValidCommand(Guid userId)
        => new(userId, GetValidSuspensionReason());

    public SuspendUserCommandHandler GetHandler(FakeUserRepository repository, FakeUnitOfWork unitOfWork)
        => new(repository, unitOfWork, new SuspendUserCommandValidator());

    public User GetSeededUser(FakeUserRepository repository)
    {
        var user = GetValidUser();
        repository.Seed(user);
        return user;
    }
}

[CollectionDefinition(nameof(SuspendUserTestFixture))]
public class SuspendUserTestFixtureCollection : ICollectionFixture<SuspendUserTestFixture>
{ }
