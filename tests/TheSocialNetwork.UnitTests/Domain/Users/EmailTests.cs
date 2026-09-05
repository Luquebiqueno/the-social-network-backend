using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.UnitTests.Domain.Users;

[Collection(nameof(UserTestFixture))]
public class EmailTests(UserTestFixture fixture)
{
    private readonly UserTestFixture _fixture = fixture;

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsValid()
    {
        var value = _fixture.GetValidEmail();

        var result = Email.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Fact]
    public void Create_ShouldNormalize_ToLowercaseAndTrimmed()
    {
        var value = _fixture.GetValidEmail();

        var result = Email.Create($"  {value.ToUpperInvariant()}  ");

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("not-an-email")]
    [InlineData("missing-domain@")]
    [InlineData("@missing-local.com")]
    [InlineData("no-at-sign.com")]
    public void Create_ShouldFail_WhenValueIsInvalid(string? value)
    {
        var result = Email.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidEmail, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenValueExceedsMaxLength()
    {
        var localPart = new string('a', 250);
        var value = $"{localPart}@a.co";

        var result = Email.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidEmail, result.Error);
    }
}
