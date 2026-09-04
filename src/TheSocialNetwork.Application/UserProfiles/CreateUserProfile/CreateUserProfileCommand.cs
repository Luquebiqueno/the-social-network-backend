using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.UserProfiles.CreateUserProfile;

public sealed record CreateUserProfileCommand(
    Guid UserId,
    string Username,
    string DisplayName) : ICommand<Result<Guid>>;
