using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.UnitTests.Common;

namespace TheSocialNetwork.UnitTests.Domain.UserProfiles;

public class UserProfileTestFixture : BaseFixture
{
    public UserProfileTestFixture()
        : base() { }

    public Guid GetValidUserId()
        => Guid.CreateVersion7();

    public Guid GetValidUserProfileId()
        => Guid.CreateVersion7();

    public string GetValidUsername()
        => Faker.Random.AlphaNumeric(Faker.Random.Int(3, 30)).ToLowerInvariant();

    public string GetUsernameAtMaxLength()
        => Faker.Random.AlphaNumeric(30).ToLowerInvariant();

    public string GetTooLongUsername()
        => Faker.Random.AlphaNumeric(31).ToLowerInvariant();

    public Username GetValidUsernameValueObject()
        => Username.Create(GetValidUsername()).Value;

    public string GetValidDisplayName()
    {
        var displayName = Faker.Name.FullName();
        return displayName.Length > 80 ? displayName[..80] : displayName;
    }

    public string GetDisplayNameAtMaxLength()
        => Faker.Random.AlphaNumeric(80);

    public string GetTooLongDisplayName()
        => Faker.Random.AlphaNumeric(81);

    public DisplayName GetValidDisplayNameValueObject()
        => DisplayName.Create(GetValidDisplayName()).Value;

    public string GetValidBiography()
    {
        var biography = Faker.Lorem.Sentence();
        return biography.Length > Biography.MaxLength
            ? biography[..Biography.MaxLength]
            : biography;
    }

    public string GetBiographyAtMaxLength()
        => Faker.Random.AlphaNumeric(Biography.MaxLength);

    public string GetTooLongBiography()
        => Faker.Random.AlphaNumeric(Biography.MaxLength + 1);

    public Biography GetValidBiographyValueObject()
        => Biography.Create(GetValidBiography()).Value;

    public string GetValidAvatarUrl()
        => Faker.Internet.Url();

    public AvatarUrl GetValidAvatarUrlValueObject()
        => AvatarUrl.Create(GetValidAvatarUrl()).Value;

    public ProfileVisibility GetRandomProfileVisibility()
    {
        var values = Enum.GetValues<ProfileVisibility>();
        return values[Faker.Random.Int(0, values.Length - 1)];
    }

    public UserProfile GetValidUserProfile()
        => UserProfile.Create(
            GetValidUserId(),
            GetValidUsernameValueObject(),
            GetValidDisplayNameValueObject()
        ).Value;
}

[CollectionDefinition(nameof(UserProfileTestFixture))]
public class UserProfileTestFixtureCollection : ICollectionFixture<UserProfileTestFixture>
{ }
