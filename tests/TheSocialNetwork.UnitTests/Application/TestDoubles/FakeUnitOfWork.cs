using System.Data.Common;
using TheSocialNetwork.Application.Abstractions.Data;

namespace TheSocialNetwork.UnitTests.Application.TestDoubles;

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public int BeginTransactionCallCount { get; private set; }
    public int CommitCallCount { get; private set; }
    public int RollbackCallCount { get; private set; }

    public DbConnection Connection => throw new NotSupportedException();
    public DbTransaction? Transaction => null;

    public Task<DbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        BeginTransactionCallCount++;
        return Task.CompletedTask;
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        CommitCallCount++;
        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        RollbackCallCount++;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
