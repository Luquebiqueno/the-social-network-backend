using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.Users.SuspendUser;

public sealed record SuspendUserCommand(
    Guid UserId,
    string Reason) : ICommand<Result>;
