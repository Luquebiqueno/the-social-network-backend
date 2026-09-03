using FluentValidation.TestHelper;
using TheSocialNetwork.Application.UserProfiles.ChangeUsername;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.ChangeUsername;

[Collection(nameof(ChangeUsernameTestFixture))]
public class ChangeUsernameCommandValidatorTests(ChangeUsernameTestFixture fixture)
{
    private readonly ChangeUsernameTestFixture _fixture = fixture;
    private readonly ChangeUsernameCommandValidator _validator = new();

    [Fact]
    public void ShouldNotHaveErrors_WhenCommandIsValid()
    {
        var command = _fixture.GetValidCommand(_fixture.GetValidUserProfileId());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldHaveError_WhenUserProfileIdIsEmpty()
    {
        var command = new ChangeUsernameCommand(Guid.Empty, _fixture.GetValidUsername());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserProfileId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("in$valid")]
    public void ShouldHaveError_WhenNewUsernameIsInvalid(string newUsername)
    {
        var command = new ChangeUsernameCommand(_fixture.GetValidUserProfileId(), newUsername);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.NewUsername);
    }
}
