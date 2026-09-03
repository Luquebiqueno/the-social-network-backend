using System.Data;
using System.Data.Common;
using Npgsql;
using TheSocialNetwork.Application.Abstractions.Data;

namespace TheSocialNetwork.Infra.Persistence.Data;

internal sealed class DapperUnitOfWork(NpgsqlDataSource dataSource) : IUnitOfWork
{
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;

    public DbConnection Connection => _connection
        ?? throw new InvalidOperationException("The unit of work has not started.");

    public DbTransaction? Transaction => _transaction;

    public async Task<DbConnection> GetOpenConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        _connection ??= await dataSource.OpenConnectionAsync(cancellationToken);
        return _connection;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
            throw new InvalidOperationException("A transaction is already active.");

        _connection = (NpgsqlConnection)await GetOpenConnectionAsync(cancellationToken);
        _transaction = await _connection.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken
        );
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        var transaction = _transaction
            ?? throw new InvalidOperationException("There is no active transaction.");

        await transaction.CommitAsync(cancellationToken);
        await transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}
