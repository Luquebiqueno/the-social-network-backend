using FluentValidation.TestHelper;
using TheSocialNetwork.Application.Users.ReactivateUser;

namespace TheSocialNetwork.UnitTests.Application.Users.ReactivateUser;

[Collection(nameof(ReactivateUserTestFixture))]
public class ReactivateUserCommandValidatorTests(ReactivateUserTestFixture fixture)
{
    private readonly ReactivateUserTestFixture _fixture = fixture;
    private readonly ReactivateUserCommandValidator _validator = new();

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
