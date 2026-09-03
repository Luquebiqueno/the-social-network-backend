using TheSocialNetwork.Api.Extensions;
using TheSocialNetwork.Api.Infrastructure;
using TheSocialNetwork.Application.UserProfiles.CreateUserProfile;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Api.Endpoints.UserProfiles;

internal sealed class Create : IEndpoint
{
    public sealed record Request(Guid UserId, string Username, string DisplayName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("user-profiles", async (
            Request request,
            CreateUserProfileCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateUserProfileCommand(request.UserId, request.Username, request.DisplayName);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Created($"user-profiles/{id}", id),
                CustomResults.Problem);
        })
        .WithTags(Tags.UserProfiles);
    }
}
