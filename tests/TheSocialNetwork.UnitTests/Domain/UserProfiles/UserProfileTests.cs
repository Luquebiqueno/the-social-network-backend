using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.UnitTests.Domain.UserProfiles;

[Collection(nameof(UserProfileTestFixture))]
public class UserProfileTests(UserProfileTestFixture fixture)
{
    private readonly UserProfileTestFixture _fixture = fixture;

    [Fact]
    public void Create_ShouldSucceed_WhenInputIsValid()
    {
        var userId = _fixture.GetValidUserId();
        var username = _fixture.GetValidUsernameValueObject();
        var displayName = _fixture.GetValidDisplayNameValueObject();

        var result = UserProfile.Create(userId, username, displayName);

        Assert.True(result.IsSuccess);
        var profile = result.Value;
        Assert.Equal(userId, profile.UserId);
        Assert.Equal(username, profile.Username);
        Assert.Equal(displayName, profile.DisplayName);
        Assert.Equal(string.Empty, profile.Biography.Value);
        Assert.Null(profile.AvatarUrl.Value);
        Assert.Equal(ProfileVisibility.Public, profile.Visibility);
        Assert.Equal(profile.CreatedAtUtc, profile.UpdatedAtUtc);
    }

    [Fact]
    public void Create_ShouldFail_WhenUserIdIsEmpty()
    {
        var result = UserProfile.Create(
            Guid.Empty,
            _fixture.GetValidUsernameValueObject(),
            _fixture.GetValidDisplayNameValueObject());

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.InvalidUserId, result.Error);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUsernameIsNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => UserProfile.Create(
                _fixture.GetValidUserId(), null!, _fixture.GetValidDisplayNameValueObject()));
    }

    [Fact]
    public void Create_ShouldThrow_WhenDisplayNameIsNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => UserProfile.Create(
                _fixture.GetValidUserId(), _fixture.GetValidUsernameValueObject(), null!));
    }

    [Fact]
    public void ChangeUsername_ShouldUpdateUsernameAndTouch_WhenUsernameIsDifferent()
    {
        var profile = _fixture.GetValidUserProfile();
        var newUsername = _fixture.GetValidUsernameValueObject();
        var occurredAt = profile.CreatedAtUtc.AddDays(1);

        var result = profile.ChangeUsername(newUsername, occurredAt);

        Assert.True(result.IsSuccess);
        Assert.Equal(newUsername, profile.Username);
        Assert.Equal(occurredAt, profile.UpdatedAtUtc);
    }

    [Fact]
    public void ChangeUsername_ShouldFail_WhenUsernameIsUnchanged()
    {
        var profile = _fixture.GetValidUserProfile();
        var occurredAt = profile.CreatedAtUtc.AddDays(1);

        var result = profile.ChangeUsername(profile.Username, occurredAt);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.UsernameUnchanged, result.Error);
        Assert.Equal(profile.CreatedAtUtc, profile.UpdatedAtUtc);
    }

    [Fact]
    public void ChangeUsername_ShouldThrow_WhenUsernameIsNull()
    {
        var profile = _fixture.GetValidUserProfile();

        Assert.Throws<ArgumentNullException>(
            () => profile.ChangeUsername(null!, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Update_ShouldUpdateFieldsAndTouch_WhenFieldsAreDifferent()
    {
        var profile = _fixture.GetValidUserProfile();
        var newDisplayName = _fixture.GetValidDisplayNameValueObject();
        var newBiography = _fixture.GetValidBiographyValueObject();
        var newAvatarUrl = _fixture.GetValidAvatarUrlValueObject();
        var occurredAt = profile.CreatedAtUtc.AddDays(1);

        var result = profile.Update(newDisplayName, newBiography, newAvatarUrl, occurredAt);

        Assert.True(result.IsSuccess);
        Assert.Equal(newDisplayName, profile.DisplayName);
        Assert.Equal(newBiography, profile.Biography);
        Assert.Equal(newAvatarUrl, profile.AvatarUrl);
        Assert.Equal(occurredAt, profile.UpdatedAtUtc);
    }

    [Fact]
    public void Update_ShouldNotTouch_WhenFieldsAreUnchanged()
    {
        var profile = _fixture.GetValidUserProfile();
        var occurredAt = profile.CreatedAtUtc.AddDays(1);

        var result = profile.Update(profile.DisplayName, profile.Biography, profile.AvatarUrl, occurredAt);

        Assert.True(result.IsSuccess);
        Assert.Equal(profile.CreatedAtUtc, profile.UpdatedAtUtc);
    }

    [Theory]
    [InlineData("displayName")]
    [InlineData("biography")]
    [InlineData("avatarUrl")]
    public void Update_ShouldThrow_WhenAnyArgumentIsNull(string nullArgument)
    {
        var profile = _fixture.GetValidUserProfile();
        var displayName = nullArgument == "displayName" ? null : _fixture.GetValidDisplayNameValueObject();
        var biography = nullArgument == "biography" ? null : Biography.Create(null).Value;
        var avatarUrl = nullArgument == "avatarUrl" ? null : AvatarUrl.Create(null).Value;

        Assert.Throws<ArgumentNullException>(
            () => profile.Update(displayName!, biography!, avatarUrl!, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void ChangeVisibility_ShouldUpdateVisibilityAndTouch_WhenVisibilityIsDifferent()
    {
        var profile = _fixture.GetValidUserProfile();
        var occurredAt = profile.CreatedAtUtc.AddDays(1);

        var result = profile.ChangeVisibility(ProfileVisibility.Private, occurredAt);

        Assert.True(result.IsSuccess);
        Assert.Equal(ProfileVisibility.Private, profile.Visibility);
        Assert.Equal(occurredAt, profile.UpdatedAtUtc);
    }

    [Fact]
    public void ChangeVisibility_ShouldNotTouch_WhenVisibilityIsUnchanged()
    {
        var profile = _fixture.GetValidUserProfile();
        var occurredAt = profile.CreatedAtUtc.AddDays(1);

        var result = profile.ChangeVisibility(ProfileVisibility.Public, occurredAt);

        Assert.True(result.IsSuccess);
        Assert.Equal(profile.CreatedAtUtc, profile.UpdatedAtUtc);
    }
}
