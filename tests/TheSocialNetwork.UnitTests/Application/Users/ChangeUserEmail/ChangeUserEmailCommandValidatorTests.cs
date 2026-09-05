using FluentValidation.TestHelper;
using TheSocialNetwork.Application.Users.ChangeUserEmail;

namespace TheSocialNetwork.UnitTests.Application.Users.ChangeUserEmail;

[Collection(nameof(ChangeUserEmailTestFixture))]
public class ChangeUserEmailCommandValidatorTests(ChangeUserEmailTestFixture fixture)
{
    private readonly ChangeUserEmailTestFixture _fixture = fixture;
    private readonly ChangeUserEmailCommandValidator _validator = new();

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
        var command = new ChangeUserEmailCommand(Guid.Empty, _fixture.GetValidEmail());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing-domain@")]
    public void ShouldHaveError_WhenEmailIsInvalid(string email)
    {
        var command = new ChangeUserEmailCommand(_fixture.GetValidUserId(), email);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Email);
    }
}
