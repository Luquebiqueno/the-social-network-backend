using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.Users.ChangeUserEmail;

public sealed record ChangeUserEmailCommand(
    Guid UserId,
    string Email) : ICommand<Result>;
