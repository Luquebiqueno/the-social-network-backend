using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.UserProfiles;

public static class UserProfileErrors
{
    public static readonly Error InvalidUserId = Error.Validation(
        "UserProfile.InvalidUserId",
        "The user identifier is invalid."
    );
    public static readonly Error InvalidUserProfileId = Error.Validation(
        "UserProfile.InvalidUserProfileId",
        "The user profile identifier is invalid."
    );
    public static readonly Error InvalidUsername = Error.Validation(
        "UserProfile.InvalidUsername", 
        "The username must contain 3 to 30 letters, numbers, periods or underscores."
    );
    public static readonly Error InvalidDisplayName = Error.Validation(
        "UserProfile.InvalidDisplayName", 
        "The display name must contain 1 to 80 characters."
    );
    public static readonly Error BiographyTooLong = Error.Validation(
        "UserProfile.BiographyTooLong", 
        "The biography cannot exceed 500 characters."
    );
    public static readonly Error InvalidAvatarUrl = Error.Validation(
        "UserProfile.InvalidAvatarUrl", 
        "The avatar URL must be an absolute HTTP or HTTPS URL."
    );
    public static readonly Error UsernameUnchanged = Error.Validation(
        "UserProfile.UsernameUnchanged",
        "The username is equal to the current username."
    );
    public static readonly Error NotFound = Error.NotFound(
        "UserProfile.NotFound",
        "The user profile was not found."
    );
    public static readonly Error AlreadyExists = Error.Conflict(
        "UserProfile.AlreadyExists",
        "A profile already exists for this user."
    );
    public static readonly Error UsernameAlreadyTaken = Error.Conflict(
        "UserProfile.UsernameAlreadyTaken",
        "The username is already taken."
    );
}
