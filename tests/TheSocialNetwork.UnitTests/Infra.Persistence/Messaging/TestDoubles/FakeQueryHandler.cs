using TheSocialNetwork.Application.Abstractions.Messaging;

namespace TheSocialNetwork.UnitTests.Infra.Persistence.Messaging.TestDoubles;

public sealed record FakeQuery(string Value) : IQuery<string>;

public sealed class FakeQueryHandler(string resultToReturn) : IQueryHandler<FakeQuery, string>
{
    public int HandleCallCount { get; private set; }
    public FakeQuery? LastQuery { get; private set; }

    public Task<string> HandleAsync(FakeQuery query, CancellationToken cancellationToken = default)
    {
        HandleCallCount++;
        LastQuery = query;
        return Task.FromResult(resultToReturn);
    }
}
