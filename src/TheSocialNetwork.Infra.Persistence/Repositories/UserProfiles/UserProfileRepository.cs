using Dapper;
using Npgsql;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.Domain.SeedWork;
using TheSocialNetwork.Infra.Persistence.Data;

namespace TheSocialNetwork.Infra.Persistence.Repositories.UserProfiles;

public sealed class UserProfileRepository(
    IUnitOfWork unitOfWork,
    DatabaseOptions options) : IUserProfileRepository
{
    private const string SelectColumns = """
        id,
        user_id AS UserId,
        username,
        display_name AS DisplayName,
        biography,
        avatar_url AS AvatarUrl,
        visibility,
        created_at_utc AS CreatedAtUtc,
        updated_at_utc AS UpdatedAtUtc
        """;

    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly DatabaseOptions _options = options;
    public async Task<UserProfile?> GetByIdAsync(
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM social.user_profiles
            WHERE id = @ProfileId;
            """;

        var row = await QuerySingleOrDefaultAsync(
            sql,
            new { ProfileId = profileId },
            cancellationToken);

        return row is null ? null : Map(row);
    }

    public async Task<UserProfile?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM social.user_profiles
            WHERE user_id = @UserId;
            """;

        var row = await QuerySingleOrDefaultAsync(
            sql,
            new { UserId = userId },
            cancellationToken);

        return row is null ? null : Map(row);
    }

    public async Task<UserProfile?> GetByUsernameAsync(
        Username username,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM social.user_profiles
            WHERE username = @Username;
            """;

        var row = await QuerySingleOrDefaultAsync(
            sql,
            new { Username = username.Value },
            cancellationToken);

        return row is null ? null : Map(row);
    }

    public async Task<bool> IsUsernameAvailableAsync(
        Username username,
        Guid? excludingProfileId = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT NOT EXISTS (
                SELECT 1
                FROM social.user_profiles
                WHERE username = @Username
                  AND (@ExcludingProfileId IS NULL OR id <> @ExcludingProfileId)
            );
            """;

        var command = CreateCommand(
            sql,
            new { Username = username.Value, ExcludingProfileId = excludingProfileId },
            cancellationToken);

        var connection = await _unitOfWork.GetOpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<Result> AddAsync(
        UserProfile profile,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO social.user_profiles (
                id, user_id, username, display_name, biography, avatar_url,
                visibility, created_at_utc, updated_at_utc)
            VALUES (
                @Id, @UserId, @Username, @DisplayName, @Biography, @AvatarUrl,
                @Visibility, @CreatedAtUtc, @UpdatedAtUtc);
            """;

        var command = CreateCommand(sql, ToParameters(profile), cancellationToken);

        try
        {
            var connection = await _unitOfWork.GetOpenConnectionAsync(cancellationToken);

            await connection.ExecuteScalarAsync<long>(command);

            return Result.Success();
        }
        catch (PostgresException exception)
            when (exception.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            if (exception.ConstraintName == "uq_user_profiles_username")
                return Result.Failure(UserProfileErrors.UsernameAlreadyTaken);

            if (exception.ConstraintName == "uq_user_profiles_user_id")
                return Result.Failure(UserProfileErrors.AlreadyExists);

            throw;
        }
    }

    public async Task<Result> UpdateAsync(
        UserProfile profile,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE social.user_profiles
            SET username = @Username,
                display_name = @DisplayName,
                biography = @Biography,
                avatar_url = @AvatarUrl,
                visibility = @Visibility,
                updated_at_utc = @UpdatedAtUtc
            WHERE id = @Id;
            """;

        var command = CreateCommand(sql, ToParameters(profile), cancellationToken);

        try
        {
            var connection = await _unitOfWork.GetOpenConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(command);

            return Result.Success();
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return Result.Failure(UserProfileErrors.UsernameAlreadyTaken);
        }
    }

    private async Task<UserProfileRow?> QuerySingleOrDefaultAsync(
        string sql,
        object parameters,
        CancellationToken cancellationToken)
    {
        var command = CreateCommand(sql, parameters, cancellationToken);
        var connection = await _unitOfWork.GetOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<UserProfileRow>(command);
    }

    private CommandDefinition CreateCommand(
        string sql,
        object? parameters,
        CancellationToken cancellationToken) =>
        new(
            sql,
            parameters,
            _unitOfWork.Transaction,
            _options.CommandTimeoutSeconds,
            cancellationToken: cancellationToken
        );

    private static object ToParameters(UserProfile profile) => new
    {
        profile.Id,
        profile.UserId,
        Username = profile.Username.Value,
        DisplayName = profile.DisplayName.Value,
        Biography = profile.Biography.Value,
        AvatarUrl = profile.AvatarUrl.Value,
        Visibility = profile.Visibility.ToString(),
        profile.CreatedAtUtc,
        profile.UpdatedAtUtc,
    };

    private static UserProfile Map(UserProfileRow row)
    {
        var username = Username.Create(row.Username).Value;
        var displayName = DisplayName.Create(row.DisplayName).Value;
        var biography = Biography.Create(row.Biography).Value;
        var avatarUrl = AvatarUrl.Create(row.AvatarUrl).Value;
        var visibility = Enum.Parse<ProfileVisibility>(row.Visibility, ignoreCase: true);

        return UserProfile.Rehydrate(
            row.Id,
            row.UserId,
            username,
            displayName,
            biography,
            avatarUrl,
            visibility,
            row.CreatedAtUtc,
            row.UpdatedAtUtc
        );
    }

}
