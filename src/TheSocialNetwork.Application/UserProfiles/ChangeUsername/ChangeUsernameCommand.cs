using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.UserProfiles.ChangeUsername;

public sealed record ChangeUsernameCommand(
    Guid UserProfileId,
    string NewUsername) : ICommand<Result>;
