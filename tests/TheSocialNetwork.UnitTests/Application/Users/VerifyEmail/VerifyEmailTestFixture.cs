using TheSocialNetwork.Application.Users.VerifyEmail;
using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Application.TestDoubles;
using TheSocialNetwork.UnitTests.Domain.Users;

namespace TheSocialNetwork.UnitTests.Application.Users.VerifyEmail;

public class VerifyEmailTestFixture : UserTestFixture
{
    public VerifyEmailTestFixture()
        : base() { }

    public VerifyEmailCommand GetValidCommand(Guid userId)
        => new(userId);

    public VerifyEmailCommandHandler GetHandler(FakeUserRepository repository, FakeUnitOfWork unitOfWork)
        => new(repository, unitOfWork, new VerifyEmailCommandValidator());

    public User GetSeededUser(FakeUserRepository repository)
    {
        var user = GetValidUser();
        repository.Seed(user);
        return user;
    }
}

[CollectionDefinition(nameof(VerifyEmailTestFixture))]
public class VerifyEmailTestFixtureCollection : ICollectionFixture<VerifyEmailTestFixture>
{ }
