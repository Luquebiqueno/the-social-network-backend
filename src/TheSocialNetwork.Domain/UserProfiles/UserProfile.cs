using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.UserProfiles;

public sealed class UserProfile : AggregateRoot
{
    private UserProfile() { }

    private UserProfile(
        Guid userId,
        Username username,
        DisplayName displayName,
        DateTimeOffset createdAtUtc)
        : base()
    {
        UserId = userId;
        Username = username;
        DisplayName = displayName;
        Biography = Biography.Create(null).Value;
        AvatarUrl = AvatarUrl.Create(null).Value;
        Visibility = ProfileVisibility.Public;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public Guid UserId { get; private set; }
    public Username Username { get; private set; } = null!;
    public DisplayName DisplayName { get; private set; } = null!;
    public Biography Biography { get; private set; } = null!;
    public AvatarUrl AvatarUrl { get; private set; } = null!;
    public ProfileVisibility Visibility { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<UserProfile> Create(
        Guid userId,
        Username username,
        DisplayName displayName
    )
    {
        if (userId == Guid.Empty)
            return Result.Failure<UserProfile>(UserProfileErrors.InvalidUserId);

        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(displayName);

        var profile = new UserProfile(userId, username, displayName, DateTimeOffset.UtcNow);

        return Result.Success(profile);
    }

    public Result ChangeUsername(Username username, DateTimeOffset occurredAtUtc)
    {
        ArgumentNullException.ThrowIfNull(username);
        if (Username == username)
            return Result.Failure(UserProfileErrors.UsernameUnchanged);

        Username = username;
        Touch(occurredAtUtc);

        return Result.Success();
    }

    public Result Update(
        DisplayName displayName,
        Biography biography,
        AvatarUrl avatarUrl,
        DateTimeOffset occurredAtUtc)
    {
        ArgumentNullException.ThrowIfNull(displayName);
        ArgumentNullException.ThrowIfNull(biography);
        ArgumentNullException.ThrowIfNull(avatarUrl);

        if (DisplayName == displayName && Biography == biography && AvatarUrl == avatarUrl)
            return Result.Success();

        DisplayName = displayName;
        Biography = biography;
        AvatarUrl = avatarUrl;
        Touch(occurredAtUtc);

        return Result.Success();
    }

    public Result ChangeVisibility(ProfileVisibility visibility, DateTimeOffset occurredAtUtc)
    {
        if (Visibility == visibility)
            return Result.Success();

        Visibility = visibility;
        Touch(occurredAtUtc);

        return Result.Success();
    }

    public static UserProfile Rehydrate(
        Guid id,
        Guid userId,
        Username username,
        DisplayName displayName,
        Biography biography,
        AvatarUrl avatarUrl,
        ProfileVisibility visibility,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc
    )
    {
        return new UserProfile
        {
            Id = id,
            UserId = userId,
            Username = username,
            DisplayName = displayName,
            Biography = biography,
            AvatarUrl = avatarUrl,
            Visibility = visibility,
            CreatedAtUtc = createdAtUtc,
            UpdatedAtUtc = updatedAtUtc
        };
    }

    private void Touch(DateTimeOffset occurredAtUtc) => UpdatedAtUtc = occurredAtUtc;
}
