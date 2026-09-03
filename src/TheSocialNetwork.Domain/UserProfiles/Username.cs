using System.Text.RegularExpressions;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.UserProfiles;

public sealed record Username
{
    private static readonly Regex Pattern = new(
        @"^[a-z0-9](?:[a-z0-9._]{1,28}[a-z0-9])?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private Username(string value) => Value = value;

    public string Value { get; }

    public static Result<Username> Create(string? value)
    {
        var normalized = value?.Trim().ToLowerInvariant();

        return string.IsNullOrWhiteSpace(normalized) || !Pattern.IsMatch(normalized)
            ? Result.Failure<Username>(UserProfileErrors.InvalidUsername)
            : Result.Success(new Username(normalized));
    }

    public override string ToString() => Value;
}
