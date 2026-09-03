using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.UnitTests.Domain.SeedWork;

[Collection(nameof(SeedWorkTestFixture))]
public class ResultTests(SeedWorkTestFixture fixture)
{
    private readonly SeedWorkTestFixture _fixture = fixture;

    [Fact]
    public void Success_ShouldCreateSuccessfulResult_WithNoError()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult_WithGivenError()
    {
        var error = _fixture.GetValidError();

        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void GenericSuccess_ShouldExposeValue()
    {
        var value = _fixture.GetValidValue();

        var result = Result.Success(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void GenericFailure_ShouldThrow_WhenAccessingValue()
    {
        var result = Result.Failure<int>(_fixture.GetValidError());

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnSuccess_WhenValueIsNotNull()
    {
        var value = _fixture.GetValidStringValue();

        Result<string> result = value;

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnFailure_WhenValueIsNull()
    {
        string? value = null;
        Result<string> result = value;

        Assert.True(result.IsFailure);
        Assert.Equal(Error.NullValue, result.Error);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenSuccessHasError()
    {
        Assert.Throws<ArgumentException>(
            () => new Result(true, _fixture.GetValidError()));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenFailureHasNoError()
    {
        Assert.Throws<ArgumentException>(() => new Result(false, Error.None));
    }
}
