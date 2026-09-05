using TheSocialNetwork.Application.Users.RegisterUser;
using TheSocialNetwork.UnitTests.Application.TestDoubles;
using TheSocialNetwork.UnitTests.Domain.Users;

namespace TheSocialNetwork.UnitTests.Application.Users.RegisterUser;

public class RegisterUserTestFixture : UserTestFixture
{
    public RegisterUserTestFixture()
        : base() { }

    public RegisterUserCommand GetValidCommand()
        => new(GetValidEmail(), GetValidExternalIdentityProvider(), GetValidExternalIdentitySubject());

    public RegisterUserCommandHandler GetHandler(
        FakeUserRepository repository, FakeUnitOfWork unitOfWork)
        => new(repository, unitOfWork, new RegisterUserCommandValidator());
}

[CollectionDefinition(nameof(RegisterUserTestFixture))]
public class RegisterUserTestFixtureCollection : ICollectionFixture<RegisterUserTestFixture>
{ }
