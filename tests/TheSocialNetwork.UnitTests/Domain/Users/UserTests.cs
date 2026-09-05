using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.UnitTests.Domain.Users;

[Collection(nameof(UserTestFixture))]
public class UserTests(UserTestFixture fixture)
{
    private readonly UserTestFixture _fixture = fixture;

    [Fact]
    public void Register_ShouldSucceed_WhenInputIsValid()
    {
        var email = _fixture.GetValidEmailValueObject();
        var provider = _fixture.GetValidExternalIdentityProviderValueObject();
        var subject = _fixture.GetValidExternalIdentitySubjectValueObject();

        var result = User.Register(email, provider, subject);

        Assert.True(result.IsSuccess);
        var user = result.Value;
        Assert.Equal(email, user.Email);
        Assert.Equal(provider, user.ExternalIdentityProvider);
        Assert.Equal(subject, user.ExternalIdentitySubject);
        Assert.Equal(UserStatus.PendingActivation, user.Status);
        Assert.False(user.IsEmailVerified);
        Assert.Null(user.EmailVerifiedAtUtc);
        Assert.Null(user.SuspendedAtUtc);
        Assert.Null(user.DeactivatedAtUtc);
        Assert.Null(user.SuspensionReason);
    }

    [Theory]
    [InlineData("email")]
    [InlineData("provider")]
    [InlineData("subject")]
    public void Register_ShouldThrow_WhenAnyArgumentIsNull(string nullArgument)
    {
        var email = nullArgument == "email" ? null : _fixture.GetValidEmailValueObject();
        var provider = nullArgument == "provider" ? null : _fixture.GetValidExternalIdentityProviderValueObject();
        var subject = nullArgument == "subject" ? null : _fixture.GetValidExternalIdentitySubjectValueObject();

        Assert.Throws<ArgumentNullException>(() => User.Register(email!, provider!, subject!));
    }

    [Fact]
    public void ChangeEmail_ShouldUpdateEmail_WhenEmailIsDifferent()
    {
        var user = _fixture.GetValidUser();
        var newEmail = _fixture.GetValidEmailValueObject();

        var result = user.ChangeEmail(newEmail);

        Assert.True(result.IsSuccess);
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void ChangeEmail_ShouldFail_WhenEmailIsUnchanged()
    {
        var user = _fixture.GetValidUser();

        var result = user.ChangeEmail(user.Email);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.EmailUnchanged, result.Error);
    }

    [Fact]
    public void ChangeEmail_ShouldThrow_WhenEmailIsNull()
    {
        var user = _fixture.GetValidUser();

        Assert.Throws<ArgumentNullException>(() => user.ChangeEmail(null!));
    }

    [Fact]
    public void VerifyEmail_ShouldMarkVerifiedAndActivate_WhenPendingActivation()
    {
        var user = _fixture.GetValidUser();
        var occurredAt = user.CreatedAtUtc.AddMinutes(1);

        var result = user.VerifyEmail(occurredAt);

        Assert.True(result.IsSuccess);
        Assert.True(user.IsEmailVerified);
        Assert.Equal(occurredAt, user.EmailVerifiedAtUtc);
        Assert.Equal(UserStatus.Active, user.Status);
    }

    [Fact]
    public void VerifyEmail_ShouldFail_WhenAlreadyVerified()
    {
        var user = _fixture.GetValidUser();
        user.VerifyEmail(DateTimeOffset.UtcNow);

        var result = user.VerifyEmail(DateTimeOffset.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.EmailAlreadyVerified, result.Error);
    }

    [Fact]
    public void VerifyEmail_ShouldFail_WhenUserIsDeactivated()
    {
        var user = _fixture.GetDeactivatedUser();

        var result = user.VerifyEmail(DateTimeOffset.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AlreadyDeactivated, result.Error);
    }

    [Fact]
    public void Suspend_ShouldSucceed_WhenUserIsActive()
    {
        var user = _fixture.GetValidUser();
        var reason = _fixture.GetValidSuspensionReasonValueObject();
        var occurredAt = user.CreatedAtUtc.AddMinutes(1);

        var result = user.Suspend(reason, occurredAt);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserStatus.Suspended, user.Status);
        Assert.Equal(occurredAt, user.SuspendedAtUtc);
        Assert.Equal(reason, user.SuspensionReason);
    }

    [Fact]
    public void Suspend_ShouldFail_WhenAlreadySuspended()
    {
        var user = _fixture.GetSuspendedUser();

        var result = user.Suspend(_fixture.GetValidSuspensionReasonValueObject(), DateTimeOffset.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AlreadySuspended, result.Error);
    }

    [Fact]
    public void Suspend_ShouldFail_WhenUserIsDeactivated()
    {
        var user = _fixture.GetDeactivatedUser();

        var result = user.Suspend(_fixture.GetValidSuspensionReasonValueObject(), DateTimeOffset.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.CannotSuspendDeactivatedUser, result.Error);
    }

    [Fact]
    public void Suspend_ShouldThrow_WhenReasonIsNull()
    {
        var user = _fixture.GetValidUser();

        Assert.Throws<ArgumentNullException>(() => user.Suspend(null!, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Reactivate_ShouldRestoreActiveStatus_WhenEmailIsVerified()
    {
        var user = _fixture.GetValidUser();
        user.VerifyEmail(DateTimeOffset.UtcNow);
        user.Suspend(_fixture.GetValidSuspensionReasonValueObject(), DateTimeOffset.UtcNow);

        var result = user.Reactivate(DateTimeOffset.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.Null(user.SuspendedAtUtc);
        Assert.Null(user.SuspensionReason);
    }

    [Fact]
    public void Reactivate_ShouldRestorePendingActivation_WhenEmailIsNotVerified()
    {
        var user = _fixture.GetSuspendedUser();

        var result = user.Reactivate(DateTimeOffset.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserStatus.PendingActivation, user.Status);
        Assert.Null(user.SuspendedAtUtc);
        Assert.Null(user.SuspensionReason);
    }

    [Fact]
    public void Reactivate_ShouldFail_WhenUserIsNotSuspended()
    {
        var user = _fixture.GetValidUser();

        var result = user.Reactivate(DateTimeOffset.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotSuspended, result.Error);
    }

    [Fact]
    public void Deactivate_ShouldSucceed_WhenUserIsActive()
    {
        var user = _fixture.GetValidUser();
        var occurredAt = user.CreatedAtUtc.AddMinutes(1);

        var result = user.Deactivate(occurredAt);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserStatus.Deactivated, user.Status);
        Assert.Equal(occurredAt, user.DeactivatedAtUtc);
    }

    [Fact]
    public void Deactivate_ShouldClearSuspensionFields_WhenUserWasSuspended()
    {
        var user = _fixture.GetSuspendedUser();

        var result = user.Deactivate(DateTimeOffset.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserStatus.Deactivated, user.Status);
        Assert.Null(user.SuspendedAtUtc);
        Assert.Null(user.SuspensionReason);
    }

    [Fact]
    public void Deactivate_ShouldFail_WhenAlreadyDeactivated()
    {
        var user = _fixture.GetDeactivatedUser();

        var result = user.Deactivate(DateTimeOffset.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AlreadyDeactivated, result.Error);
    }
}
