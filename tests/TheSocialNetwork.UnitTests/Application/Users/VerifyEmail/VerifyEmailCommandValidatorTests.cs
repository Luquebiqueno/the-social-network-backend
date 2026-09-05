using FluentValidation.TestHelper;
using TheSocialNetwork.Application.Users.VerifyEmail;

namespace TheSocialNetwork.UnitTests.Application.Users.VerifyEmail;

[Collection(nameof(VerifyEmailTestFixture))]
public class VerifyEmailCommandValidatorTests(VerifyEmailTestFixture fixture)
{
    private readonly VerifyEmailTestFixture _fixture = fixture;
    private readonly VerifyEmailCommandValidator _validator = new();

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
