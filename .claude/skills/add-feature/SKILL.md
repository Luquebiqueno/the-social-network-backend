---
name: add-feature
description: Scaffold a complete feature slice in this repo — command or query, custom handler, FluentValidation validator, DI registration, minimal API endpoint, and unit tests. Use when the user asks to add a feature, use case, command, query, or endpoint.
argument-hint: <feature description, e.g. "change a user profile's biography" or "get a user profile by username">
---

# Add a Feature (Vertical Slice)

Scaffold a full use case following this repo's conventions: an Application-layer use case with a custom command/query handler, an Api minimal-API endpoint, DI registration, and unit tests. No MediatR, no auto-registration, no validation pipeline — this codebase uses its own `ICommand`/`IQuery` abstractions, manually registered in `Application/DependencyInjection.cs`, with validation called explicitly inside the handler.

## Workflow

1. **Classify the use case.** A state change is a **command**; a read is a **query**. Derive names from the existing pattern: use case verb + entity, e.g. `ChangeUsernameCommand`, `GetUserProfileByUsernameQuery`.
2. **Check the Domain layer.** If the entity, its `{Entity}Errors` class, or the repository method you need doesn't exist yet, add it first (see the `add-entity` skill).
3. **Create the Application slice** in `src/TheSocialNetwork.Application/{Feature}/{UseCase}/` — command/query, validator (commands only), handler. Templates: [references/command-slice.md](references/command-slice.md) and [references/query-slice.md](references/query-slice.md).
4. **Register the validator and handler** in `src/TheSocialNetwork.Application/DependencyInjection.cs` — this is not auto-discovered, it's a manual `services.AddScoped<...>()` pair per use case (see existing entries for the pattern).
5. **Create the endpoint** in `src/TheSocialNetwork.Api/Endpoints/{Feature}/{UseCase}.cs`. Template: [references/endpoint.md](references/endpoint.md).
6. **Write unit tests** — handler tests, and validator tests for commands. Template: [references/tests.md](references/tests.md). There is no integration test harness in this repo yet (`IntegrationTests`/`EndToEndTests` projects are empty scaffolds) — unit tests are the deliverable.
7. **Verify:** `dotnet build` then `dotnet test tests/TheSocialNetwork.UnitTests/TheSocialNetwork.UnitTests.csproj`.

## Non-negotiable conventions

- **Folder = use case.** One folder per use case under `src/TheSocialNetwork.Application/{Feature}/` (e.g. `UserProfiles/ChangeUsername/`), containing every file for that slice.
- **Commands carry the `Result` in their own type parameter.** A void-ish command implements `ICommand<Result>`; a command returning a value implements `ICommand<Result<TValue>>` (e.g. `ICommand<Result<Guid>>`). Don't use the bare `ICommand`/`ICommandHandler<TCommand>` (no type param) unless the use case truly has nothing that can fail and nothing to report — every existing UserProfiles command uses the `Result`-wrapped form.
- **Handlers are `public sealed`** with a primary constructor, implementing `ICommandHandler<TCommand, TResult>` or `IQueryHandler<TQuery, TResult>`. The interface method is `HandleAsync`, not `Handle`.
- **Manual DI registration, always.** Handlers and validators are never auto-discovered — add both lines to `Application/DependencyInjection.cs` (`services.AddScoped<IValidator<TCommand>, TValidator>()` and `services.AddScoped<ICommandHandler<TCommand, TResult>, THandler>()`). Endpoints *are* auto-discovered via `IEndpoint` + `AddEndpoints`, so those need no registration.
- **Validate first, explicitly, inside the handler.** The first lines of `HandleAsync` call `await validator.ValidateAsync(command, cancellationToken)` and return `Result.Failure(validationResult.ToValidationError())` (or the generic form) on failure. There is no pipeline/decorator that does this for you.
- **Never throw for expected failures** — return `Result`/`Result<T>`. Errors come from static factory members on `{Entity}Errors` in the Domain layer with codes like `"UserProfiles.NotFound"`.
- **Data access via the Domain repository interface** (e.g. `IUserProfileRepository`, from `TheSocialNetwork.Domain.{Feature}`) plus `IUnitOfWork` (from `TheSocialNetwork.Application.Abstractions.Data`) for transactions around mutations — begin/commit/rollback explicitly, mirroring the existing handlers.
- **No caching, no domain events, no auth/`IUserContext`.** None of that exists in this codebase yet. Don't add `HybridCache`, `Raise(...)`, or `.RequireAuthorization()` — if the user explicitly asks for auth or caching, treat it as new infrastructure work, not something this skill already assumes.
- **Endpoints** implement `IEndpoint`, resolve the handler interface (`ICommandHandler<...>`/`IQueryHandler<...>`) directly as a lambda parameter, call `handler.HandleAsync(...)`, and translate the result with `result.Match(Results.Ok|Results.Created|Results.NoContent, CustomResults.Problem)`. Tag with `.WithTags(Tags.{Feature})` (add the constant to `Endpoints/Tags.cs` if new).

## Naming reference

| Artifact | Pattern | Example |
|---|---|---|
| Command | `{Verb}{Entity}Command` | `ChangeUsernameCommand` |
| Query | `Get{X}Query` | `GetUserProfileByUsernameQuery` |
| Handler | `{Command/Query}Handler` | `ChangeUsernameCommandHandler` |
| Validator | `{Command}Validator` | `ChangeUsernameCommandValidator` |
| Endpoint | `{UseCase}.cs` in `Endpoints/{Feature}/` | `Endpoints/UserProfiles/ChangeUsername.cs` |
| Test fixture | `{UseCase}TestFixture` | `ChangeUsernameTestFixture` |
| Handler test | `{Handler}Tests` | `ChangeUsernameCommandHandlerTests` |
| Test method | `Handle_Should{Outcome}_When{Condition}` | `Handle_ShouldFail_WhenProfileDoesNotExist` |
