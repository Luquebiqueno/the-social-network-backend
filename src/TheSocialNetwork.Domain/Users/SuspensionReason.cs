using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.Users;

public sealed record SuspensionReason
{
    public const int MaxLength = 500;

    private SuspensionReason(string value) => Value = value;

    public string Value { get; }

    public static Result<SuspensionReason> Create(string? value)
    {
        var normalized = value?.Trim();

        return string.IsNullOrWhiteSpace(normalized) || normalized.Length > MaxLength
            ? Result.Failure<SuspensionReason>(UserErrors.InvalidSuspensionReason)
            : Result.Success(new SuspensionReason(normalized));
    }
}
