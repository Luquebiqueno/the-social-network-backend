using System.Text.RegularExpressions;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.Users;

public sealed record Email
{
    private static readonly Regex Pattern = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private Email(string value) => Value = value;

    public string Value { get; }

    public static Result<Email> Create(string? value)
    {
        var normalized = value?.Trim().ToLowerInvariant();

        return string.IsNullOrWhiteSpace(normalized)
            || normalized.Length is < 3 or > 254
            || !Pattern.IsMatch(normalized)
            ? Result.Failure<Email>(UserErrors.InvalidEmail)
            : Result.Success(new Email(normalized));
    }

    public override string ToString() => Value;
}
