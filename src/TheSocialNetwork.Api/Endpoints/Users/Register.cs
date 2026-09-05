using TheSocialNetwork.Api.Extensions;
using TheSocialNetwork.Api.Infrastructure;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Application.Users.RegisterUser;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Api.Endpoints.Users;

internal sealed class Register : IEndpoint
{
    public sealed record Request(string Email, string ExternalIdentityProvider, string ExternalIdentitySubject);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users", async (
            Request request,
            ICommandHandler<RegisterUserCommand, Result<Guid>> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(
                request.Email, request.ExternalIdentityProvider, request.ExternalIdentitySubject);

            Result<Guid> result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(
                id => Results.Created($"users/{id}", id),
                CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
