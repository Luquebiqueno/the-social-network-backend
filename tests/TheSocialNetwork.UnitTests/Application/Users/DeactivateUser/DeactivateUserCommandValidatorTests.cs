using FluentValidation.TestHelper;
using TheSocialNetwork.Application.Users.DeactivateUser;

namespace TheSocialNetwork.UnitTests.Application.Users.DeactivateUser;

[Collection(nameof(DeactivateUserTestFixture))]
public class DeactivateUserCommandValidatorTests(DeactivateUserTestFixture fixture)
{
    private readonly DeactivateUserTestFixture _fixture = fixture;
    private readonly DeactivateUserCommandValidator _validator = new();

    [Fact]
    public void ShouldNotHaveErrors_WhenCommandIsValid()
    {
        var command = _fixture.GetValidCommand(_fixture.GetValidUserId());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldHaveError_WhenUserIdIsEmpty()
    {
        var command = _fixture.GetValidCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserId);
    }
}
