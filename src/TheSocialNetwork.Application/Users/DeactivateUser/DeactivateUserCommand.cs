using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.Users.DeactivateUser;

public sealed record DeactivateUserCommand(Guid UserId) : ICommand<Result>;
