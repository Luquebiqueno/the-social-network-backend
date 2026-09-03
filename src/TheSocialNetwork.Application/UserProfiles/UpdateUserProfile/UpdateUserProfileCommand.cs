namespace TheSocialNetwork.Application.UserProfiles.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    Guid UserProfileId,
    string DisplayName,
    string? Biography,
    string? AvatarUrl);
