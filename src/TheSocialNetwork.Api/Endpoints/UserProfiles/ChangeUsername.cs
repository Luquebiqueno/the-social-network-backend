using TheSocialNetwork.Api.Extensions;
using TheSocialNetwork.Api.Infrastructure;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Application.UserProfiles.ChangeUsername;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Api.Endpoints.UserProfiles;

internal sealed class ChangeUsername : IEndpoint
{
    public sealed record Request(string NewUsername);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("user-profiles/{userProfileId:guid}/username", async (
            Guid userProfileId,
            Request request,
            ICommandHandler<ChangeUsernameCommand, Result> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ChangeUsernameCommand(userProfileId, request.NewUsername);

            Result result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.UserProfiles);
    }
}
