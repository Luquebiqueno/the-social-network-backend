using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string ExternalIdentityProvider,
    string ExternalIdentitySubject) : ICommand<Result<Guid>>;
