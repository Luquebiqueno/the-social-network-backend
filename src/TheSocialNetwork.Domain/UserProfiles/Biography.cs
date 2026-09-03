using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.UserProfiles;

public sealed record Biography
{
    public const int MaxLength = 500;
    private Biography(string value) => Value = value;
    public string Value { get; }

    public static Result<Biography> Create(string? value)
    {
        var normalized = value?.Trim() ?? string.Empty;
        return normalized.Length > MaxLength
            ? Result.Failure<Biography>(UserProfileErrors.BiographyTooLong)
            : Result.Success(new Biography(normalized));
    }
}
