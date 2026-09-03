using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.UnitTests.Domain.UserProfiles;

[Collection(nameof(UserProfileTestFixture))]
public class BiographyTests(UserProfileTestFixture fixture)
{
    private readonly UserProfileTestFixture _fixture = fixture;

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsValid()
    {
        var value = _fixture.GetValidBiography();

        var result = Biography.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldSucceed_WithEmptyValue_WhenValueIsNullOrWhitespace(string? value)
    {
        var result = Biography.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, result.Value.Value);
    }

    [Fact]
    public void Create_ShouldFail_WhenValueExceedsMaxLength()
    {
        var value = _fixture.GetTooLongBiography();

        var result = Biography.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.BiographyTooLong, result.Error);
    }

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsAtMaxLength()
    {
        var value = _fixture.GetBiographyAtMaxLength();

        var result = Biography.Create(value);

        Assert.True(result.IsSuccess);
    }
}
