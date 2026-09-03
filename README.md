# The Social Network — Backend

.NET backend built with Clean Architecture, a `Result`-based error model,
and raw SQL persistence via Dapper against PostgreSQL.

## Tech stack

- **.NET 10** / C# (nullable reference types + implicit usings enabled everywhere)
- **ASP.NET Core** (minimal API host)
- **PostgreSQL** via **Npgsql** + **Dapper** (no ORM, no EF Core)
- **FluentValidation** for input-shape validation
- **xUnit** + **Bogus** for unit tests

## Architecture

The solution follows Clean Architecture / DDD-lite layering. Dependencies
only point inward:

```
Api  ──▶  Infra.Persistence  ──▶  Application  ──▶  Domain
 │                                                     ▲
 └─────────────────────────────────────────────────────┘
```

- **`TheSocialNetwork.Domain`** — no dependencies on any other project or
  package. Aggregates, value objects (smart constructors returning
  `Result<T>`, never throwing on bad input), domain errors, and shared
  `SeedWork` primitives (`Entity`, `AggregateRoot`, `Result`, `Error`).
- **`TheSocialNetwork.Application`** — one folder per aggregate, one
  subfolder per use case, each with a `Command`, a `CommandHandler`, and a
  `CommandValidator`. Depends only on `Domain` and on abstractions it
  declares itself (repository interfaces live in `Domain`). Exposes
  `AddApplication()` as its composition-root entry point.
- **`TheSocialNetwork.Infra.Persistence`** — implements the repository and
  unit-of-work interfaces declared in `Domain`/`Application` using Dapper
  and Npgsql against PostgreSQL. Exposes `AddPersistence(IConfiguration)`
  as its composition-root entry point.
- **`TheSocialNetwork.Api`** — ASP.NET Core minimal API host. Endpoints are
  self-registering classes discovered by assembly scan instead of being
  declared in `Program.cs`. `Result` failures are translated to RFC 7807
  `ProblemDetails`, and unhandled exceptions are caught by a global handler
  so the process never crashes on an infrastructure failure. Exposes
  `AddPresentation()` as its composition-root entry point.

### Project structure

```
src/
  TheSocialNetwork.Domain/
  TheSocialNetwork.Application/
  TheSocialNetwork.Infra.Persistence/
  TheSocialNetwork.Api/

tests/
  TheSocialNetwork.UnitTests/
  TheSocialNetwork.IntegrationTests/
  TheSocialNetwork.EndToEndTests/
```

## Key patterns

- **Result pattern, not exceptions.** Operations that can fail for a
  business reason return `Result` / `Result<T>` instead of throwing.
  Exceptions are reserved for programmer errors and truly exceptional
  infrastructure failures.
- **Value objects with smart constructors.** Domain value objects are
  immutable and validate themselves via a `Create(...)` factory returning
  `Result<T>`, acting as the single source of truth for their own
  validation rules.
- **One error vocabulary.** Business-rule errors live in one place per
  aggregate; the persistence layer translates low-level failures back into
  those same `Error` instances instead of inventing its own.
- **Command handlers.** Each handler follows the same shape: validate the
  command → build/validate the relevant value objects → open a transaction
  → load the aggregate → apply the domain mutation → persist → commit.

## Getting started

### Prerequisites

- .NET 10 SDK
- A PostgreSQL instance reachable with the connection string configured in
  `src/TheSocialNetwork.Api/appsettings.json`

### Build & test

```bash
dotnet build TheSocialNetwork.slnx
dotnet test tests/TheSocialNetwork.UnitTests
```

### Run the API

```bash
dotnet run --project src/TheSocialNetwork.Api
```

In `Development`, Swagger UI is served at `/swagger`.

## Testing conventions

Unit tests follow a fixture-per-aggregate/use-case convention built on top
of the [Bogus](https://github.com/bchavez/Bogus) `Faker`, with test doubles
mirroring the real repository/unit-of-work contracts so handler tests
exercise realistic failure paths.

## Current status

This backend is under active development.
