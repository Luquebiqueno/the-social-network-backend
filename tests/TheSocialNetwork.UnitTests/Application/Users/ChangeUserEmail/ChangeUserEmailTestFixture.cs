using TheSocialNetwork.Application.Users.ChangeUserEmail;
using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Application.TestDoubles;
using TheSocialNetwork.UnitTests.Domain.Users;

namespace TheSocialNetwork.UnitTests.Application.Users.ChangeUserEmail;

public class ChangeUserEmailTestFixture : UserTestFixture
{
    public ChangeUserEmailTestFixture()
        : base() { }

    public ChangeUserEmailCommand GetValidCommand(Guid userId)
        => new(userId, GetValidEmail());

    public ChangeUserEmailCommandHandler GetHandler(FakeUserRepository repository, FakeUnitOfWork unitOfWork)
        => new(repository, unitOfWork, new ChangeUserEmailCommandValidator());

    public User GetSeededUser(FakeUserRepository repository)
    {
        var user = GetValidUser();
        repository.Seed(user);
        return user;
    }
}

[CollectionDefinition(nameof(ChangeUserEmailTestFixture))]
public class ChangeUserEmailTestFixtureCollection : ICollectionFixture<ChangeUserEmailTestFixture>
{ }
