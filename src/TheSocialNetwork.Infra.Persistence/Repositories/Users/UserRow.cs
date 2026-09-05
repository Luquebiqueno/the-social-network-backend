namespace TheSocialNetwork.Infra.Persistence.Repositories.Users;

internal sealed class UserRow
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public required string ExternalIdentityProvider { get; init; }
    public required string ExternalIdentitySubject { get; init; }
    public required string Status { get; init; }
    public bool IsEmailVerified { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? EmailVerifiedAtUtc { get; init; }
    public DateTimeOffset? SuspendedAtUtc { get; init; }
    public DateTimeOffset? DeactivatedAtUtc { get; init; }
    public string? SuspensionReason { get; init; }
}
