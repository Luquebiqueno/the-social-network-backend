using FluentValidation.TestHelper;
using TheSocialNetwork.Application.UserProfiles.CreateUserProfile;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.CreateUserProfile;

[Collection(nameof(CreateUserProfileTestFixture))]
public class CreateUserProfileCommandValidatorTests(CreateUserProfileTestFixture fixture)
{
    private readonly CreateUserProfileTestFixture _fixture = fixture;
    private readonly CreateUserProfileCommandValidator _validator = new();

    [Fact]
    public void ShouldNotHaveErrors_WhenCommandIsValid()
    {
        var command = _fixture.GetValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldHaveError_WhenUserIdIsEmpty()
    {
        var command = new CreateUserProfileCommand(
            Guid.Empty, _fixture.GetValidUsername(), _fixture.GetValidDisplayName());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("in$valid")]
    [InlineData(".leadingdot")]
    public void ShouldHaveError_WhenUsernameIsInvalid(string username)
    {
        var command = new CreateUserProfileCommand(
            _fixture.GetValidUserId(), username, _fixture.GetValidDisplayName());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Username);
    }

    [Fact]
    public void ShouldHaveError_WhenUsernameExceedsMaxLength()
    {
        var command = new CreateUserProfileCommand(
            _fixture.GetValidUserId(), _fixture.GetTooLongUsername(), _fixture.GetValidDisplayName());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Username);
    }

    [Fact]
    public void ShouldHaveError_WhenDisplayNameIsEmpty()
    {
        var command = new CreateUserProfileCommand(_fixture.GetValidUserId(), _fixture.GetValidUsername(), "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.DisplayName);
    }

    [Fact]
    public void ShouldHaveError_WhenDisplayNameExceedsMaxLength()
    {
        var command = new CreateUserProfileCommand(
            _fixture.GetValidUserId(), _fixture.GetValidUsername(), _fixture.GetTooLongDisplayName());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.DisplayName);
    }
}
