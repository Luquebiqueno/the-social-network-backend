using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.UnitTests.Domain.UserProfiles;

[Collection(nameof(UserProfileTestFixture))]
public class UsernameTests(UserProfileTestFixture fixture)
{
    private readonly UserProfileTestFixture _fixture = fixture;

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsValid()
    {
        var value = _fixture.GetValidUsername();

        var result = Username.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Fact]
    public void Create_ShouldNormalize_ToLowercaseAndTrimmed()
    {
        var value = _fixture.GetValidUsername();

        var result = Username.Create($"  {value.ToUpperInvariant()}  ");

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("ab")]
    [InlineData(".abc")]
    [InlineData("abc.")]
    [InlineData("ab$c")]
    public void Create_ShouldFail_WhenValueIsInvalid(string? value)
    {
        var result = Username.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.InvalidUsername, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenValueExceedsMaxLength()
    {
        var value = _fixture.GetTooLongUsername();

        var result = Username.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.InvalidUsername, result.Error);
    }

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsAtMaxLength()
    {
        var value = _fixture.GetUsernameAtMaxLength();

        var result = Username.Create(value);

        Assert.True(result.IsSuccess);
    }
}
