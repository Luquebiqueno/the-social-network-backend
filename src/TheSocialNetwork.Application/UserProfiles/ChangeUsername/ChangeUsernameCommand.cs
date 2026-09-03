namespace TheSocialNetwork.Application.UserProfiles.ChangeUsername;

public sealed record ChangeUsernameCommand(
    Guid UserProfileId,
    string NewUsername);
