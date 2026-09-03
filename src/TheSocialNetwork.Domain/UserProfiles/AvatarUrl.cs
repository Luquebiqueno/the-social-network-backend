using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.UserProfiles;

public sealed record AvatarUrl
{
    private AvatarUrl(string? value) => Value = value;
    public string? Value { get; }

    public static Result<AvatarUrl> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Success(new AvatarUrl((string?)null));

        var normalized = value.Trim();
        var valid = Uri.TryCreate(normalized, UriKind.Absolute, out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);

        return valid
            ? Result.Success(new AvatarUrl(normalized))
            : Result.Failure<AvatarUrl>(UserProfileErrors.InvalidAvatarUrl);
    }
}
