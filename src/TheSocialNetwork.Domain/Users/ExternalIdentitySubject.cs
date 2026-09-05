using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.Users;

public sealed record ExternalIdentitySubject
{
    private ExternalIdentitySubject(string value) => Value = value;

    public string Value { get; }

    public static Result<ExternalIdentitySubject> Create(string? value)
    {
        var normalized = value?.Trim();

        return string.IsNullOrWhiteSpace(normalized) || normalized.Length > 255
            ? Result.Failure<ExternalIdentitySubject>(UserErrors.InvalidExternalIdentitySubject)
            : Result.Success(new ExternalIdentitySubject(normalized));
    }

    public override string ToString() => Value;
}
