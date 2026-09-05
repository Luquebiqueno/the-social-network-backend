using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.UnitTests.Domain.Users;

[Collection(nameof(UserTestFixture))]
public class ExternalIdentityProviderTests(UserTestFixture fixture)
{
    private readonly UserTestFixture _fixture = fixture;

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsValid()
    {
        var value = _fixture.GetValidExternalIdentityProvider();

        var result = ExternalIdentityProvider.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Fact]
    public void Create_ShouldNormalize_ToLowercaseAndTrimmed()
    {
        var value = _fixture.GetValidExternalIdentityProvider();

        var result = ExternalIdentityProvider.Create($"  {value.ToUpperInvariant()}  ");

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldFail_WhenValueIsBlank(string? value)
    {
        var result = ExternalIdentityProvider.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidExternalIdentityProvider, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenValueExceedsMaxLength()
    {
        var value = _fixture.GetTooLongExternalIdentityProvider();

        var result = ExternalIdentityProvider.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidExternalIdentityProvider, result.Error);
    }
}
