using TheSocialNetwork.Application.Users.DeactivateUser;
using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Application.TestDoubles;
using TheSocialNetwork.UnitTests.Domain.Users;

namespace TheSocialNetwork.UnitTests.Application.Users.DeactivateUser;

public class DeactivateUserTestFixture : UserTestFixture
{
    public DeactivateUserTestFixture()
        : base() { }

    public DeactivateUserCommand GetValidCommand(Guid userId)
        => new(userId);

    public DeactivateUserCommandHandler GetHandler(FakeUserRepository repository, FakeUnitOfWork unitOfWork)
        => new(repository, unitOfWork, new DeactivateUserCommandValidator());

    public User GetSeededUser(FakeUserRepository repository)
    {
        var user = GetValidUser();
        repository.Seed(user);
        return user;
    }
}

[CollectionDefinition(nameof(DeactivateUserTestFixture))]
public class DeactivateUserTestFixtureCollection : ICollectionFixture<DeactivateUserTestFixture>
{ }
