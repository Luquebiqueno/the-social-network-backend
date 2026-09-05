using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.UnitTests.Domain.Users;

[Collection(nameof(UserTestFixture))]
public class SuspensionReasonTests(UserTestFixture fixture)
{
    private readonly UserTestFixture _fixture = fixture;

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsValid()
    {
        var value = _fixture.GetValidSuspensionReason();

        var result = SuspensionReason.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldFail_WhenValueIsBlank(string? value)
    {
        var result = SuspensionReason.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidSuspensionReason, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenValueExceedsMaxLength()
    {
        var value = _fixture.GetTooLongSuspensionReason();

        var result = SuspensionReason.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidSuspensionReason, result.Error);
    }

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsAtMaxLength()
    {
        var value = new string('a', SuspensionReason.MaxLength);

        var result = SuspensionReason.Create(value);

        Assert.True(result.IsSuccess);
    }
}
