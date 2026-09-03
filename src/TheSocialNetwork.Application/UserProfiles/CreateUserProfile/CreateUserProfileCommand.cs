namespace TheSocialNetwork.Application.UserProfiles.CreateUserProfile;

public sealed record CreateUserProfileCommand(
    Guid UserId,
    string Username,
    string DisplayName);
