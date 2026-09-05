using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.Users;

public sealed record ExternalIdentityProvider
{
    private ExternalIdentityProvider(string value) => Value = value;

    public string Value { get; }

    public static Result<ExternalIdentityProvider> Create(string? value)
    {
        var normalized = value?.Trim().ToLowerInvariant();

        return string.IsNullOrWhiteSpace(normalized) || normalized.Length > 50
            ? Result.Failure<ExternalIdentityProvider>(UserErrors.InvalidExternalIdentityProvider)
            : Result.Success(new ExternalIdentityProvider(normalized));
    }

    public override string ToString() => Value;
}
