using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.UnitTests.Common;

namespace TheSocialNetwork.UnitTests.Domain.Users;

public class UserTestFixture : BaseFixture
{
    public UserTestFixture()
        : base() { }

    public Guid GetValidUserId()
        => Guid.CreateVersion7();

    public string GetValidEmail()
        => Faker.Internet.Email().ToLowerInvariant();

    public Email GetValidEmailValueObject()
        => Email.Create(GetValidEmail()).Value;

    public string GetValidExternalIdentityProvider()
        => Faker.Random.AlphaNumeric(Faker.Random.Int(1, 50)).ToLowerInvariant();

    public string GetTooLongExternalIdentityProvider()
        => Faker.Random.AlphaNumeric(51).ToLowerInvariant();

    public ExternalIdentityProvider GetValidExternalIdentityProviderValueObject()
        => ExternalIdentityProvider.Create(GetValidExternalIdentityProvider()).Value;

    public string GetValidExternalIdentitySubject()
        => Faker.Random.AlphaNumeric(Faker.Random.Int(1, 255));

    public string GetTooLongExternalIdentitySubject()
        => Faker.Random.AlphaNumeric(256);

    public ExternalIdentitySubject GetValidExternalIdentitySubjectValueObject()
        => ExternalIdentitySubject.Create(GetValidExternalIdentitySubject()).Value;

    public string GetValidSuspensionReason()
        => Faker.Lorem.Sentence();

    public string GetTooLongSuspensionReason()
        => Faker.Random.AlphaNumeric(SuspensionReason.MaxLength + 1);

    public SuspensionReason GetValidSuspensionReasonValueObject()
        => SuspensionReason.Create(GetValidSuspensionReason()).Value;

    public User GetValidUser()
        => User.Register(
            GetValidEmailValueObject(),
            GetValidExternalIdentityProviderValueObject(),
            GetValidExternalIdentitySubjectValueObject()
        ).Value;

    public User GetSuspendedUser()
    {
        var user = GetValidUser();
        user.Suspend(GetValidSuspensionReasonValueObject(), DateTimeOffset.UtcNow);
        return user;
    }

    public User GetDeactivatedUser()
    {
        var user = GetValidUser();
        user.Deactivate(DateTimeOffset.UtcNow);
        return user;
    }
}

[CollectionDefinition(nameof(UserTestFixture))]
public class UserTestFixtureCollection : ICollectionFixture<UserTestFixture>
{ }
