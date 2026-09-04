# Query Slice Templates

Files go in `src/TheSocialNetwork.Application/{Feature}/{UseCase}/`, same layout as a command slice. **No query use case exists in this repo yet** — this template extrapolates from the command pattern and the `IQuery`/`IQueryHandler` abstractions already defined in `Application/Abstractions/Messaging/`. Treat it as a starting point, not a copy of an established convention, and prefer consistency with whatever the first real query ends up looking like.

## Query

```csharp
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application.UserProfiles.GetUserProfileByUsername;

public sealed record GetUserProfileByUsernameQuery(string Username) : IQuery<Result<UserProfileResponse>>;
```

Same reasoning as commands: wrap the payload in `Result<T>` so `IQueryHandler<TQuery, TResult>.HandleAsync` can report "not found" without throwing.

## Response DTO

A `sealed record` in the same folder, projected from the domain entity — never return the entity itself across the Application boundary.

```csharp
namespace TheSocialNetwork.Application.UserProfiles.GetUserProfileByUsername;

public sealed record UserProfileResponse(
    Guid Id,
    string Username,
    string DisplayName,
    string? Biography,
    string? AvatarUrl,
    string Visibility);
```

## Handler

`public sealed`, primary constructor, repository only (no `IUnitOfWork` — a query doesn't write, so no transaction). No validator: queries in this codebase's abstractions have no validation step, so keep parameter checks (if any) inline in the handler.

```csharp
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Domain.SeedWork;
using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.Application.UserProfiles.GetUserProfileByUsername;

public sealed class GetUserProfileByUsernameQueryHandler(
    IUserProfileRepository repository) : IQueryHandler<GetUserProfileByUsernameQuery, Result<UserProfileResponse>>
{
    private readonly IUserProfileRepository _repository = repository;

    public async Task<Result<UserProfileResponse>> HandleAsync(
        GetUserProfileByUsernameQuery query,
        CancellationToken cancellationToken = default)
    {
        var usernameResult = Username.Create(query.Username);
        if (usernameResult.IsFailure)
            return Result.Failure<UserProfileResponse>(usernameResult.Error);

        var profile = await _repository.GetByUsernameAsync(usernameResult.Value, cancellationToken);
        if (profile is null)
            return Result.Failure<UserProfileResponse>(UserProfileErrors.NotFound);

        return new UserProfileResponse(
            profile.Id,
            profile.Username.Value,
            profile.DisplayName.Value,
            profile.Biography.Value,
            profile.AvatarUrl.Value,
            profile.Visibility.ToString());
    }
}
```

## DI registration

```csharp
services.AddScoped<IQueryHandler<GetUserProfileByUsernameQuery, Result<UserProfileResponse>>, GetUserProfileByUsernameQueryHandler>();
```

No `IValidator<>` line — queries don't have validators in this codebase.
