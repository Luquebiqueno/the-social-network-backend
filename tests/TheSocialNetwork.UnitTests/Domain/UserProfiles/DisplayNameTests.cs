using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.UnitTests.Domain.UserProfiles;

[Collection(nameof(UserProfileTestFixture))]
public class DisplayNameTests(UserProfileTestFixture fixture)
{
    private readonly UserProfileTestFixture _fixture = fixture;

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsValid()
    {
        var value = _fixture.GetValidDisplayName();

        var result = DisplayName.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Fact]
    public void Create_ShouldTrim_LeadingAndTrailingWhitespace()
    {
        var value = _fixture.GetValidDisplayName();

        var result = DisplayName.Create($"  {value}  ");

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldFail_WhenValueIsNullOrWhitespace(string? value)
    {
        var result = DisplayName.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.InvalidDisplayName, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenValueExceedsMaxLength()
    {
        var value = _fixture.GetTooLongDisplayName();

        var result = DisplayName.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserProfileErrors.InvalidDisplayName, result.Error);
    }

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsAtMaxLength()
    {
        var value = _fixture.GetDisplayNameAtMaxLength();

        var result = DisplayName.Create(value);

        Assert.True(result.IsSuccess);
    }
}
