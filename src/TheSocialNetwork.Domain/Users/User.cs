using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.Users;

public sealed class User : AggregateRoot
{
    private User() { }

    private User(
        Email email,
        ExternalIdentityProvider externalIdentityProvider,
        ExternalIdentitySubject externalIdentitySubject,
        DateTimeOffset createdAtUtc)
        : base()
    {
        Email = email;
        ExternalIdentityProvider = externalIdentityProvider;
        ExternalIdentitySubject = externalIdentitySubject;
        Status = UserStatus.PendingActivation;
        IsEmailVerified = false;
        CreatedAtUtc = createdAtUtc;
    }

    public Email Email { get; private set; } = null!;
    public ExternalIdentityProvider ExternalIdentityProvider { get; private set; } = null!;
    public ExternalIdentitySubject ExternalIdentitySubject { get; private set; } = null!;
    public UserStatus Status { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? EmailVerifiedAtUtc { get; private set; }
    public DateTimeOffset? SuspendedAtUtc { get; private set; }
    public DateTimeOffset? DeactivatedAtUtc { get; private set; }
    public SuspensionReason? SuspensionReason { get; private set; }

    public static Result<User> Register(
        Email email,
        ExternalIdentityProvider externalIdentityProvider,
        ExternalIdentitySubject externalIdentitySubject)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(externalIdentityProvider);
        ArgumentNullException.ThrowIfNull(externalIdentitySubject);

        var user = new User(email, externalIdentityProvider, externalIdentitySubject, DateTimeOffset.UtcNow);

        return Result.Success(user);
    }

    public Result ChangeEmail(Email newEmail)
    {
        ArgumentNullException.ThrowIfNull(newEmail);

        if (Email == newEmail)
            return Result.Failure(UserErrors.EmailUnchanged);

        Email = newEmail;

        return Result.Success();
    }

    public Result VerifyEmail(DateTimeOffset occurredAtUtc)
    {
        if (Status == UserStatus.Deactivated)
            return Result.Failure(UserErrors.AlreadyDeactivated);

        if (IsEmailVerified)
            return Result.Failure(UserErrors.EmailAlreadyVerified);

        IsEmailVerified = true;
        EmailVerifiedAtUtc = occurredAtUtc;

        if (Status == UserStatus.PendingActivation)
            Status = UserStatus.Active;

        return Result.Success();
    }

    public Result Suspend(SuspensionReason reason, DateTimeOffset occurredAtUtc)
    {
        ArgumentNullException.ThrowIfNull(reason);

        if (Status == UserStatus.Suspended)
            return Result.Failure(UserErrors.AlreadySuspended);

        if (Status == UserStatus.Deactivated)
            return Result.Failure(UserErrors.CannotSuspendDeactivatedUser);

        Status = UserStatus.Suspended;
        SuspendedAtUtc = occurredAtUtc;
        SuspensionReason = reason;

        return Result.Success();
    }

    public Result Reactivate(DateTimeOffset occurredAtUtc)
    {
        if (Status != UserStatus.Suspended)
            return Result.Failure(UserErrors.NotSuspended);

        Status = IsEmailVerified ? UserStatus.Active : UserStatus.PendingActivation;
        SuspendedAtUtc = null;
        SuspensionReason = null;

        return Result.Success();
    }

    public Result Deactivate(DateTimeOffset occurredAtUtc)
    {
        if (Status == UserStatus.Deactivated)
            return Result.Failure(UserErrors.AlreadyDeactivated);

        Status = UserStatus.Deactivated;
        DeactivatedAtUtc = occurredAtUtc;
        SuspendedAtUtc = null;
        SuspensionReason = null;

        return Result.Success();
    }

    public static User Rehydrate(
        Guid id,
        Email email,
        ExternalIdentityProvider externalIdentityProvider,
        ExternalIdentitySubject externalIdentitySubject,
        UserStatus status,
        bool isEmailVerified,
        DateTimeOffset createdAtUtc,
        DateTimeOffset? emailVerifiedAtUtc,
        DateTimeOffset? suspendedAtUtc,
        DateTimeOffset? deactivatedAtUtc,
        SuspensionReason? suspensionReason)
    {
        return new User
        {
            Id = id,
            Email = email,
            ExternalIdentityProvider = externalIdentityProvider,
            ExternalIdentitySubject = externalIdentitySubject,
            Status = status,
            IsEmailVerified = isEmailVerified,
            CreatedAtUtc = createdAtUtc,
            EmailVerifiedAtUtc = emailVerifiedAtUtc,
            SuspendedAtUtc = suspendedAtUtc,
            DeactivatedAtUtc = deactivatedAtUtc,
            SuspensionReason = suspensionReason
        };
    }
}
