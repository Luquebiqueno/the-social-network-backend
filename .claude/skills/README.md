# TheSocialNetwork Agent Skills

A skill pack that teaches Claude Code the conventions actually used in this repo — vertical-slice use cases, the custom `ICommand`/`IQuery` + `ICommandHandler`/`IQueryHandler` abstractions, `Result`-based error handling, minimal API endpoints, and the `UserProfile` test style (Bogus-backed fixtures, hand-written fakes, xUnit).

## What's inside

| Skill | Invoke with | What it does |
|---|---|---|
| **add-feature** | `/add-feature archive a user profile` | Scaffolds a complete vertical slice: command/query, handler, validator, endpoint, DI registration, and handler + validator unit tests. |
| **add-entity** | `/add-entity Project with a name and owner` | Adds a domain entity end to end: entity, error catalog, repository interface + Dapper repository. |
| **add-tests** | `/add-tests CreateUserProfileCommand` | Backfills handler and validator unit tests for an existing use case. |
| **ca-review** | `/ca-review` | Reviews pending changes against this repo's conventions: layer boundaries, error handling, slice structure, DI registration, and test coverage. |

You don't have to invoke them explicitly — once installed, Claude Code picks the right skill automatically when you say things like "add an endpoint to change a user's biography."

## What this project actually looks like

- Projects: `TheSocialNetwork.Domain`, `TheSocialNetwork.Application`, `TheSocialNetwork.Infra.Persistence` (Dapper, not EF Core), `TheSocialNetwork.Api` (ASP.NET Core minimal APIs).
- Shared primitives live in `TheSocialNetwork.Domain.SeedWork` (`Result`, `Result<T>`, `Error`, `ValidationError`, `Entity`, `AggregateRoot`) — there is no `SharedKernel` project.
- No MediatR. Commands/queries implement `ICommand`/`ICommand<TResponse>`/`IQuery<TResponse>` (`TheSocialNetwork.Application.Abstractions.Messaging`); handlers implement `ICommandHandler<TCommand>` / `ICommandHandler<TCommand, TResult>` / `IQueryHandler<TQuery, TResult>` and are resolved directly from DI as endpoint parameters — no dispatcher/sender.
- No auto-registration for handlers or validators — every handler and validator is registered by hand in `Application/DependencyInjection.cs`. Endpoints *are* auto-discovered (`IEndpoint` + `AddEndpoints`/`MapEndpoints`), so those never need manual registration.
- Validation runs manually at the top of `HandleAsync` (`await validator.ValidateAsync(...)`) — there is no decorator pipeline.
- No caching, no domain events, no auth/`IUserContext` yet. Don't invent them — if a skill ever needs them, add the infrastructure first and update the skill.
- Tests: xUnit + Bogus only. Repository/unit-of-work doubles are hand-written fakes (`FakeUserProfileRepository`, `FakeUnitOfWork`) under `tests/TheSocialNetwork.UnitTests/Application/TestDoubles/` — no mocking library. Each slice gets a `{UseCase}TestFixture` (`ICollectionFixture`) providing valid data via Bogus. `IntegrationTests`/`EndToEndTests` projects exist but are empty scaffolds — there's no HTTP test harness yet, so feature work stops at unit tests.

## Installation

The skills live in `.claude/skills/`. If you're in this repo, they're already active.

To use them in another project based on this template, copy the folder:

```
your-project/
└── .claude/
    └── skills/
        ├── add-feature/
        ├── add-entity/
        ├── add-tests/
        └── ca-review/
```

## Try it

```
/add-feature change a user profile's biography
```

Claude will create the command, validator, handler, DI registration, the endpoint, and the handler + validator unit tests — then build and run the tests.

## Customizing

Each skill is a plain Markdown file (`SKILL.md`, plus templates under `add-feature/references/`). If you add caching, auth, or domain events to this codebase later, update the relevant `SKILL.md` and reference files so future features pick it up automatically — the skills are the executable version of this repo's conventions, so they only stay useful if they track what the code actually does.
