# Test Templates

Location mirrors the slice: `tests/TheSocialNetwork.UnitTests/Application/{Feature}/{UseCase}/`. Every UserProfiles use case has exactly three files there — `{UseCase}TestFixture.cs`, `{Handler}Tests.cs`, `{Validator}Tests.cs` — plus shared fakes in `tests/TheSocialNetwork.UnitTests/Application/TestDoubles/`. Stack: xUnit + Bogus. No mocking library, no Shouldly — assertions are plain `Assert.*`, test data comes from hand-written fakes plus a `Faker` wrapped by a fixture.

There is no integration test harness in this repo (`IntegrationTests`/`EndToEndTests` are empty scaffolds) — don't try to write HTTP-level tests until that changes.

## Test fixture

One per use case, extends the feature's base fixture (e.g. `UserProfileTestFixture` for anything under `UserProfiles/`), registered as an `ICollectionFixture` so xUnit shares one instance across the test class.

```csharp
using TheSocialNetwork.Application.UserProfiles.ChangeUsername;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.UnitTests.Application.TestDoubles;
using TheSocialNetwork.UnitTests.Domain.UserProfiles;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.ChangeUsername;

public class ChangeUsernameTestFixture : UserProfileTestFixture
{
    public ChangeUsernameTestFixture()
        : base() { }

    public ChangeUsernameCommand GetValidCommand(Guid userProfileId)
        => new(userProfileId, GetValidUsername());

    public ChangeUsernameCommandHandler GetHandler(FakeUserProfileRepository repository, FakeUnitOfWork unitOfWork)
        => new(repository, unitOfWork, new ChangeUsernameCommandValidator());

    public UserProfile GetSeededProfile(FakeUserProfileRepository repository)
    {
        var profile = GetValidUserProfile();
        repository.Seed(profile);
        return profile;
    }
}

[CollectionDefinition(nameof(ChangeUsernameTestFixture))]
public class ChangeUsernameTestFixtureCollection : ICollectionFixture<ChangeUsernameTestFixture>
{ }
```

`GetValidCommand(...)` and `GetHandler(...)` (wiring the real validator) are the two methods every fixture needs at minimum; add `GetSeeded{Entity}` when tests need an existing row. Random valid values (`GetValidUsername()`, `GetValidDisplayName()`, etc.) live one level up on the shared `{Feature}TestFixture` (e.g. `UserProfileTestFixture`) — reuse those instead of hand-rolling `Faker` calls per use case.

## Handler unit test

```csharp
using TheSocialNetwork.Application.UserProfiles.ChangeUsername;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.UnitTests.Application.TestDoubles;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.ChangeUsername;

[Collection(nameof(ChangeUsernameTestFixture))]
public class ChangeUsernameCommandHandlerTests(ChangeUsernameTestFixture fixture)
{
    private readonly ChangeUsernameTestFixture _fixture = fixture;
    private readonly FakeUserProfileRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Handle_ShouldChangeUsername_WhenCommandIsValid()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var profile = _fixture.GetSeededProfile(_repository);
        var command = _fixture.GetValidCommand(profile.Id);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.NewUsername, profile.Username.Value);
        Assert.Equal(1, _repository.UpdateCallCount);
        Assert.Equal(1, _unitOfWork.CommitCallCount);
        Assert.Equal(0, _unitOfWork.RollbackCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenProfileDoesNotExist()
    {
        var handler = _fixture.GetHandler(_repository, _unitOfWork);
        var command = _fixture.GetValidCommand(_fixture.GetValidUserProfileId());

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.NotFound, result.Error);
        Assert.Equal(1, _unitOfWork.RollbackCallCount);
    }
}
```

- **Class/method naming:** class is `{Handler}Tests`; methods stay `Handle_Should{Outcome}_When{Condition}` even though the method under test is `HandleAsync` — that's the established naming in this repo, don't rename existing tests to `HandleAsync_Should...`.
- Cover every `Result.Failure` branch in the handler plus the happy path. For the happy path, assert both the returned `Result` and the mutated state on the entity you got back from the fake repository (fakes store entities by reference, so mutating the aggregate in the handler is visible on the object the test already holds) plus call counts (`UpdateCallCount`, `CommitCallCount`, `RollbackCallCount`) to prove the transaction was driven correctly.
- No `// Arrange`/`// Act`/`// Assert` comments in this repo's style — three blank-line-separated blocks (setup, invoke, assert) with no comment headers.

## Validator test

```csharp
using FluentValidation.TestHelper;
using TheSocialNetwork.Application.UserProfiles.ChangeUsername;

namespace TheSocialNetwork.UnitTests.Application.UserProfiles.ChangeUsername;

[Collection(nameof(ChangeUsernameTestFixture))]
public class ChangeUsernameCommandValidatorTests(ChangeUsernameTestFixture fixture)
{
    private readonly ChangeUsernameTestFixture _fixture = fixture;
    private readonly ChangeUsernameCommandValidator _validator = new();

    [Fact]
    public void ShouldNotHaveErrors_WhenCommandIsValid()
    {
        var command = _fixture.GetValidCommand(_fixture.GetValidUserProfileId());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("in$valid")]
    public void ShouldHaveError_WhenNewUsernameIsInvalid(string username)
    {
        var command = new ChangeUsernameCommand(_fixture.GetValidUserProfileId(), username);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.NewUsername);
    }
}
```

One `[Theory]`/`[InlineData]` (or separate `[Fact]`s) per rule, plus a single fully-valid happy-path test. Use `FluentValidation.TestHelper`'s `TestValidate`/`ShouldHaveValidationErrorFor`/`ShouldNotHaveAnyValidationErrors` — that's the only assertion style used for validators here.

## Fakes

If the use case needs a repository or unit-of-work method that the existing fakes (`FakeUserProfileRepository`, `FakeUnitOfWork` in `Application/TestDoubles/`) don't implement yet, extend the fake in place rather than mocking — this repo has no NSubstitute/Moq dependency. Track call counts as public getters (`AddCallCount`, `UpdateCallCount`, …) the way the existing fakes do, so handler tests can assert on them.

## Running

```
dotnet test tests/TheSocialNetwork.UnitTests/TheSocialNetwork.UnitTests.csproj
```
