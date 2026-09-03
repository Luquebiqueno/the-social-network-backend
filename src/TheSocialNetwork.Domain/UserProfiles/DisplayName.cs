using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.UserProfiles;

public sealed record DisplayName
{
    private DisplayName(string value) => Value = value;
    public string Value { get; }

    public static Result<DisplayName> Create(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) || normalized.Length > 80
            ? Result.Failure<DisplayName>(UserProfileErrors.InvalidDisplayName)
            : Result.Success(new DisplayName(normalized));
    }
}
