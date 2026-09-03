using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.UnitTests.Domain.UserProfiles;

[Collection(nameof(UserProfileTestFixture))]
public class AvatarUrlTests(UserProfileTestFixture fixture)
{
    private readonly UserProfileTestFixture _fixture = fixture;

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsAValidAbsoluteUrl()
    {
        var value = _fixture.GetValidAvatarUrl();

        var result = AvatarUrl.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData("http://example.com/avatar.png")]
    [InlineData("https://example.com/avatar.png")]
    public void Create_ShouldSucceed_WhenValueIsAValidAbsoluteHttpUrl(string value)
    {
        var result = AvatarUrl.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldSucceed_WithNullValue_WhenValueIsNullOrWhitespace(string? value)
    {
        var result = AvatarUrl.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Value);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/avatar.png")]
    [InlineData("/relative/path.png")]
    public void Create_ShouldFail_WhenValueIsNotAValidAbsoluteHttpUrl(string value)
    {
        var result = AvatarUrl.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.InvalidAvatarUrl, result.Error);
    }
}
