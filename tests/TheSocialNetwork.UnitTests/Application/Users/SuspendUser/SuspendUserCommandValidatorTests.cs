using FluentValidation.TestHelper;
using TheSocialNetwork.Application.Users.SuspendUser;

namespace TheSocialNetwork.UnitTests.Application.Users.SuspendUser;

[Collection(nameof(SuspendUserTestFixture))]
public class SuspendUserCommandValidatorTests(SuspendUserTestFixture fixture)
{
    private readonly SuspendUserTestFixture _fixture = fixture;
    private readonly SuspendUserCommandValidator _validator = new();

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
        var command = new SuspendUserCommand(Guid.Empty, _fixture.GetValidSuspensionReason());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldHaveError_WhenReasonIsBlank(string reason)
    {
        var command = new SuspendUserCommand(_fixture.GetValidUserId(), reason);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Reason);
    }

    [Fact]
    public void ShouldHaveError_WhenReasonExceedsMaxLength()
    {
        var command = new SuspendUserCommand(_fixture.GetValidUserId(), _fixture.GetTooLongSuspensionReason());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Reason);
    }
}
