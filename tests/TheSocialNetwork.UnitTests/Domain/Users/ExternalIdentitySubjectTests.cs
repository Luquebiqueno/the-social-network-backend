using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.UnitTests.Domain.Users;

[Collection(nameof(UserTestFixture))]
public class ExternalIdentitySubjectTests(UserTestFixture fixture)
{
    private readonly UserTestFixture _fixture = fixture;

    [Fact]
    public void Create_ShouldSucceed_WhenValueIsValid()
    {
        var value = _fixture.GetValidExternalIdentitySubject();

        var result = ExternalIdentitySubject.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Fact]
    public void Create_ShouldTrim_ButPreserveCase()
    {
        var value = $"Mixed{_fixture.GetValidExternalIdentitySubject()}Case";

        var result = ExternalIdentitySubject.Create($"  {value}  ");

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldFail_WhenValueIsBlank(string? value)
    {
        var result = ExternalIdentitySubject.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidExternalIdentitySubject, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenValueExceedsMaxLength()
    {
        var value = _fixture.GetTooLongExternalIdentitySubject();

        var result = ExternalIdentitySubject.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidExternalIdentitySubject, result.Error);
    }
}
