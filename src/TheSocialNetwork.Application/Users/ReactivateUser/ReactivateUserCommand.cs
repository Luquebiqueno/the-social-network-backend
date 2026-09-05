using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.Users.ReactivateUser;

public sealed record ReactivateUserCommand(Guid UserId) : ICommand<Result>;
