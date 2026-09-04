---
name: ca-review
description: Review pending changes against this repo's conventions — layer boundaries, Result-based error handling, slice structure, manual DI registration, and test coverage. Use when the user asks to review changes, check conventions, or audit a feature before committing.
---

# Convention Review

Review the given scope (default: `git diff` + untracked files) against this repo's conventions. Report findings with file:line references, ordered by severity. Do not fix anything unless asked.

## Checklist

### Layer boundaries (violations are blockers)
- `TheSocialNetwork.Domain` references nothing else — no Dapper, no `Application`/`Infra.Persistence`/`Api` types.
- `TheSocialNetwork.Application` references `Domain` only (plus FluentValidation, `Microsoft.Extensions.DependencyInjection.Abstractions`); data access exclusively through the Domain repository interfaces (e.g. `IUserProfileRepository`) and `Application.Abstractions.Data.IUnitOfWork` — no `using TheSocialNetwork.Infra.Persistence.*` anywhere in Application.
- SQL, parameter mapping, and row→entity translation live in `Infra.Persistence` repositories (via `{Entity}.Rehydrate(...)`), never on the domain entity.
- `TheSocialNetwork.Api` endpoints contain no business logic — only request→command/query mapping and `result.Match(...)`.

### Slice structure
- One folder per use case under `src/TheSocialNetwork.Application/{Feature}/{UseCase}/`; the endpoint mirrors it at `src/TheSocialNetwork.Api/Endpoints/{Feature}/{UseCase}.cs`.
- Naming: `{Verb}{Entity}Command` / `Get{X}Query` / `{Command/Query}Handler` / `{Command}Validator`.
- Handlers are `public sealed` with primary constructors, implementing `ICommandHandler<TCommand, TResult>` / `IQueryHandler<TQuery, TResult>`, method `HandleAsync` (not `Handle`).
- Commands/queries carry `Result`/`Result<T>` in their own type parameter (`ICommand<Result>`, `ICommand<Result<Guid>>`) rather than a bare value type — check the handler's return type actually reports failure, not just success.
- **DI registration is manual, not auto-discovered.** Every new handler needs `services.AddScoped<ICommandHandler<TCommand, TResult>, THandler>()` (or `IQueryHandler<...>`) in `Application/DependencyInjection.cs`, and every command validator needs `services.AddScoped<IValidator<TCommand>, TValidator>()` alongside it. Flag a new handler/validator with no matching registration as a blocker — it will 500 at runtime via `GetRequiredService`. Endpoints are the one thing that *is* auto-discovered (`IEndpoint` + `AddEndpoints`/`MapEndpoints`) — don't flag a missing manual registration for those.

### Error handling
- Expected failures return `Result`/`Result<T>` — no exceptions for control flow, no try/catch around business rules.
- Errors come from static members on `{Entity}Errors` (in Domain) with `"{Feature}.{Reason}"` codes and the semantically correct `ErrorType` (`NotFound`/`Conflict`/`Problem`/`Validation`/`Failure`).
- Endpoints translate failures only via `result.Match(Results.Ok|Results.Created|Results.NoContent, CustomResults.Problem)` — no hand-rolled status codes.

### Validation
- Every command has a FluentValidation `{Command}Validator`, registered in DI, and the handler calls `await validator.ValidateAsync(command, cancellationToken)` as the **first** thing it does — there is no pipeline/decorator doing this automatically, so a handler that skips this call is a bug, not a style nit.
- Value-object validation is reused via `Must(value => {ValueObject}.Create(value).IsSuccess)` rather than re-implemented as raw length/regex rules in the validator.
- Queries have no validators in this codebase — don't ask for one.

### Transactions
- Commands that mutate an aggregate call `IUnitOfWork.BeginTransactionAsync` before loading it, and `CommitAsync` only on the success path — every failure branch after `BeginTransactionAsync` must call `RollbackAsync` before returning.

### Out of scope for this codebase (do not flag as missing)
- Domain events / `IDomainEvent` / `Raise(...)` — not implemented yet.
- Caching / `HybridCache` / cache-key invalidation — not implemented yet.
- Authentication / `IUserContext` / `.RequireAuthorization()` — not implemented yet.
- `IDateTimeProvider` — handlers pass `DateTimeOffset.UtcNow` directly to entity methods; that's correct here, not a smell.
- HTTP-level integration tests — `IntegrationTests`/`EndToEndTests` are empty scaffolds with no harness.

If a change introduces one of the above (e.g. the user is actively adding auth), review it on its own merits instead of comparing it to a convention that doesn't exist yet — just don't invent a *missing*-this-pattern finding for code that predates the feature.

### Tests
- New/changed handlers have unit tests in `tests/TheSocialNetwork.UnitTests/Application/{Feature}/{UseCase}/` covering every `Result.Failure` branch plus the happy path (returned `Result`, mutated entity state, fake call counts).
- New/changed validators have one `TestValidate` case per rule plus a fully-valid command.
- Test doubles are the hand-written fakes in `Application/TestDoubles/` (`FakeUserProfileRepository`, `FakeUnitOfWork`) — a new mocking library dependency (NSubstitute, Moq) is a convention violation, not a legitimate addition, unless the user explicitly asked for it.
- Test naming matches existing tests: `Handle_Should{Outcome}_When{Condition}` (handlers), `ShouldHaveError_When{Condition}` / `ShouldNotHaveAnyValidationErrors` (validators).

## Output format

Group findings as **Blockers** (layer violations, missing DI registration, thrown exceptions for expected failures, missing rollback), **Convention violations** (naming, structure, error codes, skipped manual validation call), and **Test gaps**. For each: `file:line`, what's wrong, and the one-line fix. Close with a verdict: ready to commit, or what must change first. If everything passes, say so and run `dotnet build` + `dotnet test tests/TheSocialNetwork.UnitTests/TheSocialNetwork.UnitTests.csproj` to confirm.
