namespace TheSocialNetwork.Infra.Persistence.Repositories.UserProfiles;

internal sealed class UserProfileRow
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public required string Username { get; init; }
    public required string DisplayName { get; init; }
    public required string Biography { get; init; }
    public string? AvatarUrl { get; init; }
    public required string Visibility { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset UpdatedAtUtc { get; init; }
}
