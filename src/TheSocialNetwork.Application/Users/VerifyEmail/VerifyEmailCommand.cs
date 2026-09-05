using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.Users.VerifyEmail;

public sealed record VerifyEmailCommand(Guid UserId) : ICommand<Result>;
