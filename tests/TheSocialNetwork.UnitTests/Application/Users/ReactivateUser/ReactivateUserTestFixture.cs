using TheSocialNetwork.Application.Users.ReactivateUser;
using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Application.TestDoubles;
using TheSocialNetwork.UnitTests.Domain.Users;

namespace TheSocialNetwork.UnitTests.Application.Users.ReactivateUser;

public class ReactivateUserTestFixture : UserTestFixture
{
    public ReactivateUserTestFixture()
        : base() { }

    public ReactivateUserCommand GetValidCommand(Guid userId)
        => new(userId);

    public ReactivateUserCommandHandler GetHandler(FakeUserRepository repository, FakeUnitOfWork unitOfWork)
        => new(repository, unitOfWork, new ReactivateUserCommandValidator());

    public User GetSeededSuspendedUser(FakeUserRepository repository)
    {
        var user = GetSuspendedUser();
        repository.Seed(user);
        return user;
    }
}

[CollectionDefinition(nameof(ReactivateUserTestFixture))]
public class ReactivateUserTestFixtureCollection : ICollectionFixture<ReactivateUserTestFixture>
{ }
