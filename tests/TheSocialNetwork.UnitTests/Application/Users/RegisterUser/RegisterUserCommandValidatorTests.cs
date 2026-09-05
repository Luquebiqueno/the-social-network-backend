using FluentValidation.TestHelper;
using TheSocialNetwork.Application.Users.RegisterUser;

namespace TheSocialNetwork.UnitTests.Application.Users.RegisterUser;

[Collection(nameof(RegisterUserTestFixture))]
public class RegisterUserCommandValidatorTests(RegisterUserTestFixture fixture)
{
    private readonly RegisterUserTestFixture _fixture = fixture;
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void ShouldNotHaveErrors_WhenCommandIsValid()
    {
        var command = _fixture.GetValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing-domain@")]
    public void ShouldHaveError_WhenEmailIsInvalid(string email)
    {
        var command = new RegisterUserCommand(
            email, _fixture.GetValidExternalIdentityProvider(), _fixture.GetValidExternalIdentitySubject());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void ShouldHaveError_WhenExternalIdentityProviderIsEmpty()
    {
        var command = new RegisterUserCommand(
            _fixture.GetValidEmail(), "", _fixture.GetValidExternalIdentitySubject());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.ExternalIdentityProvider);
    }

    [Fact]
    public void ShouldHaveError_WhenExternalIdentityProviderExceedsMaxLength()
    {
        var command = new RegisterUserCommand(
            _fixture.GetValidEmail(),
            _fixture.GetTooLongExternalIdentityProvider(),
            _fixture.GetValidExternalIdentitySubject());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.ExternalIdentityProvider);
    }

    [Fact]
    public void ShouldHaveError_WhenExternalIdentitySubjectIsEmpty()
    {
        var command = new RegisterUserCommand(
            _fixture.GetValidEmail(), _fixture.GetValidExternalIdentityProvider(), "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.ExternalIdentitySubject);
    }

    [Fact]
    public void ShouldHaveError_WhenExternalIdentitySubjectExceedsMaxLength()
    {
        var command = new RegisterUserCommand(
            _fixture.GetValidEmail(),
            _fixture.GetValidExternalIdentityProvider(),
            _fixture.GetTooLongExternalIdentitySubject());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.ExternalIdentitySubject);
    }
}
