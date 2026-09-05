using Dapper;
using Npgsql;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.Domain.SeedWork;
using TheSocialNetwork.Infra.Persistence.Data;

namespace TheSocialNetwork.Infra.Persistence.Repositories.Users;

public sealed class UserRepository(
    IUnitOfWork unitOfWork,
    DatabaseOptions options) : IUserRepository
{
    private const string SelectColumns = """
        id,
        email,
        external_identity_provider AS ExternalIdentityProvider,
        external_identity_subject AS ExternalIdentitySubject,
        status,
        is_email_verified AS IsEmailVerified,
        created_at_utc AS CreatedAtUtc,
        email_verified_at_utc AS EmailVerifiedAtUtc,
        suspended_at_utc AS SuspendedAtUtc,
        deactivated_at_utc AS DeactivatedAtUtc,
        suspension_reason AS SuspensionReason
        """;

    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly DatabaseOptions _options = options;

    public async Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM identity.users
            WHERE id = @UserId;
            """;

        var command = CreateCommand(sql, new { UserId = userId }, cancellationToken);
        var connection = await _unitOfWork.GetOpenConnectionAsync(cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<UserRow>(command);

        return row is null ? null : Map(row);
    }

    public async Task<Result> AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO identity.users (
                id, email, external_identity_provider, external_identity_subject,
                status, is_email_verified, created_at_utc, email_verified_at_utc,
                suspended_at_utc, deactivated_at_utc, suspension_reason)
            VALUES (
                @Id, @Email, @ExternalIdentityProvider, @ExternalIdentitySubject,
                @Status, @IsEmailVerified, @CreatedAtUtc, @EmailVerifiedAtUtc,
                @SuspendedAtUtc, @DeactivatedAtUtc, @SuspensionReason);
            """;

        var command = CreateCommand(sql, ToParameters(user), cancellationToken);

        try
        {
            var connection = await _unitOfWork.GetOpenConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(command);

            return Result.Success();
        }
        catch (PostgresException exception)
            when (exception.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            if (exception.ConstraintName == "uq_users_email")
                return Result.Failure(UserErrors.EmailAlreadyRegistered);

            if (exception.ConstraintName == "uq_users_external_identity")
                return Result.Failure(UserErrors.ExternalIdentityAlreadyRegistered);

            throw;
        }
    }

    public async Task<Result> UpdateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE identity.users
            SET email = @Email,
                status = @Status,
                is_email_verified = @IsEmailVerified,
                email_verified_at_utc = @EmailVerifiedAtUtc,
                suspended_at_utc = @SuspendedAtUtc,
                deactivated_at_utc = @DeactivatedAtUtc,
                suspension_reason = @SuspensionReason
            WHERE id = @Id;
            """;

        var command = CreateCommand(sql, ToParameters(user), cancellationToken);

        try
        {
            var connection = await _unitOfWork.GetOpenConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(command);

            return Result.Success();
        }
        catch (PostgresException exception)
            when (exception.SqlState == PostgresErrorCodes.UniqueViolation
                && exception.ConstraintName == "uq_users_email")
        {
            return Result.Failure(UserErrors.EmailAlreadyRegistered);
        }
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

    private static object ToParameters(User user) => new
    {
        user.Id,
        Email = user.Email.Value,
        ExternalIdentityProvider = user.ExternalIdentityProvider.Value,
        ExternalIdentitySubject = user.ExternalIdentitySubject.Value,
        Status = user.Status.ToString(),
        user.IsEmailVerified,
        user.CreatedAtUtc,
        user.EmailVerifiedAtUtc,
        user.SuspendedAtUtc,
        user.DeactivatedAtUtc,
        SuspensionReason = user.SuspensionReason?.Value
    };

    private static User Map(UserRow row)
    {
        var email = Email.Create(row.Email).Value;
        var externalIdentityProvider = ExternalIdentityProvider.Create(row.ExternalIdentityProvider).Value;
        var externalIdentitySubject = ExternalIdentitySubject.Create(row.ExternalIdentitySubject).Value;
        var status = Enum.Parse<UserStatus>(row.Status, ignoreCase: true);
        var suspensionReason = row.SuspensionReason is null
            ? null
            : SuspensionReason.Create(row.SuspensionReason).Value;

        return User.Rehydrate(
            row.Id,
            email,
            externalIdentityProvider,
            externalIdentitySubject,
            status,
            row.IsEmailVerified,
            row.CreatedAtUtc,
            row.EmailVerifiedAtUtc,
            row.SuspendedAtUtc,
            row.DeactivatedAtUtc,
            suspensionReason
        );
    }
}
