---
name: add-entity
description: Add a new domain entity to this repo — entity class, error catalog, repository interface, and Dapper repository implementation. Use when the user asks to add an entity, aggregate, domain model, or table.
argument-hint: <entity description, e.g. "UserProfile with a Username, DisplayName, Biography, AvatarUrl, and Visibility">
---

# Add a Domain Entity

Create a new entity and wire it through every layer, following the `UserProfile` pattern (`src/TheSocialNetwork.Domain/UserProfiles/`, `src/TheSocialNetwork.Infra.Persistence/Repositories/UserProfiles/`).

## Files to create/modify

### 1. Entity — `src/TheSocialNetwork.Domain/{Feature}/{Entity}.cs`

```csharp
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.UserProfiles;

public sealed class UserProfile : AggregateRoot
{
    private UserProfile() { }

    private UserProfile(
        Guid userId,
        Username username,
        DisplayName displayName,
        DateTimeOffset createdAtUtc)
        : base()
    {
        UserId = userId;
        Username = username;
        DisplayName = displayName;
        Biography = Biography.Create(null).Value;
        AvatarUrl = AvatarUrl.Create(null).Value;
        Visibility = ProfileVisibility.Public;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public Guid UserId { get; private set; }
    public Username Username { get; private set; } = null!;
    public DisplayName DisplayName { get; private set; } = null!;
    public Biography Biography { get; private set; } = null!;
    public AvatarUrl AvatarUrl { get; private set; } = null!;
    public ProfileVisibility Visibility { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<UserProfile> Create(
        Guid userId,
        Username username,
        DisplayName displayName)
    {
        if (userId == Guid.Empty)
            return Result.Failure<UserProfile>(UserProfileErrors.InvalidUserId);

        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(displayName);

        var profile = new UserProfile(userId, username, displayName, DateTimeOffset.UtcNow);

        return Result.Success(profile);
    }

    public Result ChangeUsername(Username username, DateTimeOffset occurredAtUtc)
    {
        ArgumentNullException.ThrowIfNull(username);
        if (Username == username)
            return Result.Failure(UserProfileErrors.UsernameUnchanged);

        Username = username;
        Touch(occurredAtUtc);

        return Result.Success();
    }

    public static UserProfile Rehydrate(
        Guid id,
        Guid userId,
        Username username,
        DisplayName displayName,
        Biography biography,
        AvatarUrl avatarUrl,
        ProfileVisibility visibility,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc)
    {
        return new UserProfile
        {
            Id = id,
            UserId = userId,
            Username = username,
            DisplayName = displayName,
            Biography = biography,
            AvatarUrl = avatarUrl,
            Visibility = visibility,
            CreatedAtUtc = createdAtUtc,
            UpdatedAtUtc = updatedAtUtc
        };
    }

    private void Touch(DateTimeOffset occurredAtUtc) => UpdatedAtUtc = occurredAtUtc;
}
```

`sealed class`, inherits `AggregateRoot` (or `Entity` directly for a non-root entity) from `TheSocialNetwork.Domain.SeedWork` — that base gives it `Guid Id` (auto-generated via `Guid.CreateVersion7()`), nothing else. Mutation methods return `Result`/`Result<T>`, take an `occurredAtUtc` timestamp from the caller (handlers pass `DateTimeOffset.UtcNow` — there's no `IDateTimeProvider` abstraction here), and call a private `Touch(...)` to bump `UpdatedAtUtc`. Add a `Rehydrate(...)` static factory that the repository uses to reconstruct the entity from a persisted row (bypasses invariants — it's trusted data coming back from the DB).

Value objects (like `Username`, `DisplayName`, `Biography`, `AvatarUrl`) go in their own file, each a `sealed class` with a private constructor and a `public static Result<T> Create(...)` factory. Mirror `src/TheSocialNetwork.Domain/UserProfiles/Username.cs` for the pattern.

### 2. Error catalog — `src/TheSocialNetwork.Domain/{Feature}/{Entity}Errors.cs`

```csharp
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.UserProfiles;

public static class UserProfileErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "UserProfile.NotFound",
        "The user profile was not found.");
}
```

Codes are `"{FeaturePlural}.{Reason}"`. Pick the factory by semantics: `Error.NotFound` (→ 404 via `CustomResults.Problem`), `Error.Conflict` (→ 409), `Error.Problem`/`Error.Validation` (→ 400), `Error.Failure` (→ 500).

### 3. Repository interface — `src/TheSocialNetwork.Domain/{Feature}/I{Entity}Repository.cs`

The interface lives in **Domain**, not Application — mirror `IUserProfileRepository`:

```csharp
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.UserProfiles;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken = default);

    Task<Result> AddAsync(UserProfile profile, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(UserProfile profile, CancellationToken cancellationToken = default);
}
```

Queries that can fail to find anything return `Task<{Entity}?>`. Mutations that enforce invariants the DB itself checks (uniqueness, etc.) return `Task<Result>` so the handler can roll back and translate the failure instead of letting a DB exception bubble up.

### 4. Dapper repository — `src/TheSocialNetwork.Infra.Persistence/Repositories/{Feature}/{Entity}Repository.cs`

Implements the Domain interface using `IUnitOfWork` (`TheSocialNetwork.Application.Abstractions.Data`) for the connection/transaction, and Dapper for SQL. Add a private `{Entity}Row` record next to it (`{Entity}Row.cs`) shaped like the table, and map row → domain entity via `{Entity}.Rehydrate(...)`. Mirror `UserProfileRepository`/`UserProfileRow` for exact SQL style, parameter naming, and error mapping (e.g. translating a unique-constraint violation into a domain `Result.Failure`).

### 5. Wire it up

- Register the repository in `Infra.Persistence/DependencyInjection.cs`: `services.AddScoped<I{Entity}Repository, {Entity}Repository>();`
- Add the corresponding table/migration if this repo has a migrations folder — check before assuming one exists.

## Rules

- The Domain project references nothing else — no Dapper, no `Application`/`Infra.Persistence`/`Api` types. All persistence concerns (SQL, parameter mapping, connection handling) live in the Infra.Persistence repository, never on the entity.
- No domain events, no `IDomainEvent`/`Raise(...)` in this codebase yet — don't add them speculatively.
- Run `dotnet build` and `dotnet test tests/TheSocialNetwork.UnitTests` when done.
- If the user also wants use cases (commands/queries) for the entity, continue with the `add-feature` skill.
