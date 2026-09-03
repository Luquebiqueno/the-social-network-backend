using FluentValidation.TestHelper;
using TheSocialNetwork.Application.UserProfiles.UpdateUserProfile;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.UpdateUserProfile;

[Collection(nameof(UpdateUserProfileTestFixture))]
public class UpdateUserProfileCommandValidatorTests(UpdateUserProfileTestFixture fixture)
{
    private readonly UpdateUserProfileTestFixture _fixture = fixture;
    private readonly UpdateUserProfileCommandValidator _validator = new();

    [Fact]
    public void ShouldNotHaveErrors_WhenCommandIsValid()
    {
        var command = _fixture.GetValidCommand(_fixture.GetValidUserProfileId());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldNotHaveErrors_WhenBiographyAndAvatarUrlAreNull()
    {
        var command = new UpdateUserProfileCommand(
            _fixture.GetValidUserProfileId(), _fixture.GetValidDisplayName(), null, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldHaveError_WhenUserProfileIdIsEmpty()
    {
        var command = new UpdateUserProfileCommand(
            Guid.Empty, _fixture.GetValidDisplayName(), null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserProfileId);
    }

    [Fact]
    public void ShouldHaveError_WhenDisplayNameIsEmpty()
    {
        var command = new UpdateUserProfileCommand(_fixture.GetValidUserProfileId(), "", null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.DisplayName);
    }

    [Fact]
    public void ShouldHaveError_WhenBiographyExceedsMaxLength()
    {
        var command = new UpdateUserProfileCommand(
            _fixture.GetValidUserProfileId(), _fixture.GetValidDisplayName(), _fixture.GetTooLongBiography(), null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Biography);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/avatar.png")]
    public void ShouldHaveError_WhenAvatarUrlIsInvalid(string avatarUrl)
    {
        var command = new UpdateUserProfileCommand(
            _fixture.GetValidUserProfileId(), _fixture.GetValidDisplayName(), null, avatarUrl);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.AvatarUrl);
    }
}
