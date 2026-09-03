using TheSocialNetwork.Api.Extensions;
using TheSocialNetwork.Api.Infrastructure;
using TheSocialNetwork.Application.UserProfiles.ChangeUserProfileVisibility;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Api.Endpoints.UserProfiles;

internal sealed class ChangeVisibility : IEndpoint
{
    public sealed record Request(int Visibility);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("user-profiles/{userProfileId:guid}/visibility", async (
            Guid userProfileId,
            Request request,
            ChangeUserProfileVisibilityCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ChangeUserProfileVisibilityCommand(userProfileId, (ProfileVisibility)request.Visibility);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.UserProfiles);
    }
}
