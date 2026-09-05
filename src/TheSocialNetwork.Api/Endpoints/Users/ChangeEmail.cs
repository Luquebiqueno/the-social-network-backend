using TheSocialNetwork.Api.Extensions;
using TheSocialNetwork.Api.Infrastructure;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Application.Users.ChangeUserEmail;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Api.Endpoints.Users;

internal sealed class ChangeEmail : IEndpoint
{
    public sealed record Request(string Email);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/{userId:guid}/email", async (
            Guid userId,
            Request request,
            ICommandHandler<ChangeUserEmailCommand, Result> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ChangeUserEmailCommand(userId, request.Email);

            Result result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
