---
name: add-tests
description: Backfill missing unit tests for an existing use case in this repo — handler tests and FluentValidation validator tests, following the UserProfiles test fixture/fake pattern. Use when the user asks to add, improve, or backfill test coverage.
argument-hint: <use case or feature to cover, e.g. "ChangeUsernameCommand" or "the UserProfiles feature">
---

# Add Tests for an Existing Use Case

Backfill the two test types this repo has for a slice: handler unit tests and validator tests. There is no integration/HTTP test harness here (`IntegrationTests`/`EndToEndTests` are empty scaffolds), so stop at unit tests. Read the target command, handler, and validator first, then mirror the closest existing test class under `tests/TheSocialNetwork.UnitTests/Application/UserProfiles/`.

## Workflow

1. **Locate the slice** in `src/TheSocialNetwork.Application/{Feature}/{UseCase}/` — command, validator, handler. List every distinct outcome: each `return Result.Failure(...)` branch plus the happy path.
2. **Check what already exists** in `tests/TheSocialNetwork.UnitTests/Application/{Feature}/{UseCase}/` — extend the existing `{UseCase}TestFixture`/`{Handler}Tests`/`{Validator}Tests` classes rather than duplicating them. If the fixture is missing entirely, create it first (see template below).
3. **Write handler unit tests** — one test per `Result.Failure` branch in `HandleAsync`, plus one happy-path test asserting the returned `Result`, the mutated entity state, and the fake's call counts (`AddCallCount`/`UpdateCallCount`/`CommitCallCount`/`RollbackCallCount`).
4. **Write validator tests** (commands only — queries have no validator in this codebase) — one failing case per FluentValidation rule, plus one fully-valid command.
5. **Run** `dotnet test tests/TheSocialNetwork.UnitTests/TheSocialNetwork.UnitTests.csproj` and fix failures before finishing.

## Conventions

- **Stack:** xUnit + Bogus (via each fixture's `Faker`) for handler/domain tests; `FluentValidation.TestHelper` for validator tests. No mocking library — repository/unit-of-work doubles are hand-written fakes in `tests/TheSocialNetwork.UnitTests/Application/TestDoubles/` (`FakeUserProfileRepository`, `FakeUnitOfWork`). Extend a fake in place if it's missing a method you need; don't reach for NSubstitute/Moq.
- **Fixture pattern:** every use case has a `{UseCase}TestFixture : {Feature}TestFixture` (e.g. `ChangeUsernameTestFixture : UserProfileTestFixture`) registered via `[CollectionDefinition(nameof(...))] : ICollectionFixture<...>`, and test classes opt in with `[Collection(nameof({UseCase}TestFixture))]`. The fixture exposes `GetValidCommand(...)` and `GetHandler(repository, unitOfWork)` at minimum.
- **Naming:** test class `{Handler}Tests` / `{Validator}Tests`; methods `Handle_Should{Outcome}_When{Condition}` for handler tests (yes, `Handle_...` even though the method is `HandleAsync` — that's this repo's existing convention) and `ShouldHaveError_When{Condition}` / `ShouldNotHaveAnyValidationErrors` for validator tests.
- **Structure:** no `// Arrange`/`// Act`/`// Assert` comments — three blank-line-separated blocks (setup, invoke, assert).
- **Assertions:** plain xUnit (`Assert.True`, `Assert.Equal`, …) — no Shouldly. Compare exact domain errors with `Assert.Equal(UserProfileErrors.X, result.Error)`. Assert persisted state by reading the mutated entity you already hold a reference to (fakes store entities by reference) rather than re-querying.

Full annotated templates: [../add-feature/references/tests.md](../add-feature/references/tests.md), or mirror `ChangeUsernameCommandHandlerTests`, `ChangeUsernameCommandValidatorTests`, and `ChangeUsernameTestFixture` in this repo.
