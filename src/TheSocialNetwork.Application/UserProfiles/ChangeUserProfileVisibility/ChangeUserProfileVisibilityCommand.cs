using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.Application.UserProfiles.ChangeUserProfileVisibility;

public sealed record ChangeUserProfileVisibilityCommand(
    Guid UserProfileId,
    ProfileVisibility Visibility);
