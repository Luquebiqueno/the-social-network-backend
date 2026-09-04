# Endpoint Templates

One file per use case in `src/TheSocialNetwork.Api/Endpoints/{Feature}/{UseCase}.cs`. Endpoints implement `IEndpoint` and are auto-discovered by `AddEndpoints`/`MapEndpoints` (`Api/Extensions/EndpointExtensions.cs`) — no registration needed. Handlers and validators are **not** auto-discovered; that registration is manual (see `references/command-slice.md`).

## Command with response body (POST → 201 + value)

```csharp
using TheSocialNetwork.Api.Extensions;
using TheSocialNetwork.Api.Infrastructure;
using TheSocialNetwork.Application.Abstractions.Messaging;
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
            ICommandHandler<CreateUserProfileCommand, Result<Guid>> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateUserProfileCommand(request.UserId, request.Username, request.DisplayName);

            Result<Guid> result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(
                id => Results.Created($"user-profiles/{id}", id),
                CustomResults.Problem);
        })
        .WithTags(Tags.UserProfiles);
    }
}
```

## Void command from route parameter (PUT → 204)

```csharp
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
```

## Query (GET → 200)

```csharp
using TheSocialNetwork.Api.Extensions;
using TheSocialNetwork.Api.Infrastructure;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Application.UserProfiles.GetUserProfileByUsername;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Api.Endpoints.UserProfiles;

internal sealed class GetByUsername : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("user-profiles/by-username/{username}", async (
            string username,
            IQueryHandler<GetUserProfileByUsernameQuery, Result<UserProfileResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserProfileByUsernameQuery(username);

            Result<UserProfileResponse> result = await handler.HandleAsync(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.UserProfiles);
    }
}
```

## Rules

- Routes are lowercase, plural, no leading slash: `user-profiles`, `user-profiles/{userProfileId:guid}`. Route constraints (`:guid`) on all typed parameters.
- The nested `Request` record exists only when there's a JSON body; it maps 1:1 to the command inside the lambda (enum values arrive as `int` and get cast — see `ChangeVisibility.cs`).
- Resolve the handler interface (`ICommandHandler<TCommand, TResult>` / `IQueryHandler<TQuery, TResult>`) directly as a lambda parameter — there's no dispatcher, DI hands you the concrete registered handler.
- Call `handler.HandleAsync(command, cancellationToken)` — the interface method is `HandleAsync`, not `Handle`.
- Always end with `.WithTags(Tags.{Feature})` (add the constant to `Endpoints/Tags.cs` if new). There's no `.RequireAuthorization()` in this codebase yet — don't add it unless auth infrastructure exists.
- Failures never get hand-rolled responses — `CustomResults.Problem` (`Api/Infrastructure/CustomResults.cs`) translates the `Error` to RFC 7807 ProblemDetails with the right status code from `Error.Type`.
- The endpoint class itself stays `internal sealed` — only the handler interfaces it depends on need to be resolvable from the DI container.
