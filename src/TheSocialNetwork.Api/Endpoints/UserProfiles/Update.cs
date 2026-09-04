using TheSocialNetwork.Api.Extensions;
using TheSocialNetwork.Api.Infrastructure;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Application.UserProfiles.UpdateUserProfile;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Api.Endpoints.UserProfiles;

internal sealed class Update : IEndpoint
{
    public sealed record Request(string DisplayName, string? Biography, string? AvatarUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("user-profiles/{userProfileId:guid}", async (
            Guid userProfileId,
            Request request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateUserProfileCommand(
                userProfileId, request.DisplayName, request.Biography, request.AvatarUrl);

            Result result = await sender.SendAsync<UpdateUserProfileCommand, Result>(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.UserProfiles);
    }
}
