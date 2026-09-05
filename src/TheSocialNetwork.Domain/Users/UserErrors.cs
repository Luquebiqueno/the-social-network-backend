using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.Users;

public static class UserErrors
{
    public static readonly Error InvalidEmail = Error.Validation(
        "User.InvalidEmail",
        "The email address is invalid."
    );
    public static readonly Error InvalidExternalIdentityProvider = Error.Validation(
        "User.InvalidExternalIdentityProvider",
        "The external identity provider must contain 1 to 50 characters."
    );
    public static readonly Error InvalidExternalIdentitySubject = Error.Validation(
        "User.InvalidExternalIdentitySubject",
        "The external identity subject must contain 1 to 255 characters."
    );
    public static readonly Error InvalidSuspensionReason = Error.Validation(
        "User.InvalidSuspensionReason",
        "The suspension reason must contain 1 to 500 characters."
    );
    public static readonly Error NotFound = Error.NotFound(
        "User.NotFound",
        "The user was not found."
    );
    public static readonly Error EmailAlreadyRegistered = Error.Conflict(
        "User.EmailAlreadyRegistered",
        "A user is already registered with this email address."
    );
    public static readonly Error ExternalIdentityAlreadyRegistered = Error.Conflict(
        "User.ExternalIdentityAlreadyRegistered",
        "A user is already registered with this external identity."
    );
    public static readonly Error EmailAlreadyVerified = Error.Validation(
        "User.EmailAlreadyVerified",
        "The email address is already verified."
    );
    public static readonly Error EmailUnchanged = Error.Validation(
        "User.EmailUnchanged",
        "The email is equal to the current email."
    );
    public static readonly Error AlreadySuspended = Error.Validation(
        "User.AlreadySuspended",
        "The user is already suspended."
    );
    public static readonly Error AlreadyDeactivated = Error.Validation(
        "User.AlreadyDeactivated",
        "The user is already deactivated."
    );
    public static readonly Error NotSuspended = Error.Validation(
        "User.NotSuspended",
        "The user is not suspended."
    );
    public static readonly Error CannotSuspendDeactivatedUser = Error.Validation(
        "User.CannotSuspendDeactivatedUser",
        "A deactivated user cannot be suspended."
    );
}
