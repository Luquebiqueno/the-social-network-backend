using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.UnitTests.Domain.SeedWork;

[Collection(nameof(SeedWorkTestFixture))]
public class ValidationErrorTests(SeedWorkTestFixture fixture)
{
    private readonly SeedWorkTestFixture _fixture = fixture;

    [Fact]
    public void FromResults_ShouldContainOnlyFailedResultsErrors()
    {
        var errorOne = _fixture.GetValidValidationError();
        var errorTwo = _fixture.GetValidValidationError();
        var results = new[]
        {
            Result.Success(),
            Result.Failure(errorOne),
            Result.Failure(errorTwo)
        };

        var validationError = ValidationError.FromResults(results);

        Assert.Equal(ErrorType.Validation, validationError.Type);
        Assert.Equal([errorOne, errorTwo], validationError.Errors);
    }

    [Fact]
    public void FromResults_ShouldReturnEmptyErrors_WhenAllResultsSucceed()
    {
        var results = new[] { Result.Success(), Result.Success() };

        var validationError = ValidationError.FromResults(results);

        Assert.Empty(validationError.Errors);
    }
}
