using FluentValidation.TestHelper;
using TheSocialNetwork.Application.UserProfiles.ChangeUserProfileVisibility;
using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.ChangeUserProfileVisibility;

[Collection(nameof(ChangeUserProfileVisibilityTestFixture))]
public class ChangeUserProfileVisibilityCommandValidatorTests(ChangeUserProfileVisibilityTestFixture fixture)
{
    private readonly ChangeUserProfileVisibilityTestFixture _fixture = fixture;
    private readonly ChangeUserProfileVisibilityCommandValidator _validator = new();

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
        var command = new ChangeUserProfileVisibilityCommand(Guid.Empty, _fixture.GetRandomProfileVisibility());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserProfileId);
    }

    [Fact]
    public void ShouldHaveError_WhenVisibilityIsNotAValidEnumValue()
    {
        var command = new ChangeUserProfileVisibilityCommand(_fixture.GetValidUserProfileId(), (ProfileVisibility)999);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Visibility);
    }
}
