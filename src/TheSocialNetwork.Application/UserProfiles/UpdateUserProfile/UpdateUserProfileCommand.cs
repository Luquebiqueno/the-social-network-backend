using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.UserProfiles.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    Guid UserProfileId,
    string DisplayName,
    string? Biography,
    string? AvatarUrl) : ICommand<Result>;
