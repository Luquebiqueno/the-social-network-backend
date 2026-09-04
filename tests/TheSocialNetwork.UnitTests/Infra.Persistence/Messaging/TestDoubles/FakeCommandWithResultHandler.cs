using TheSocialNetwork.Application.Abstractions.Messaging;

namespace TheSocialNetwork.UnitTests.Infra.Persistence.Messaging.TestDoubles;

public sealed record FakeCommandWithResult(string Value) : ICommand<string>;

public sealed class FakeCommandWithResultHandler(string resultToReturn)
    : ICommandHandler<FakeCommandWithResult, string>
{
    public int HandleCallCount { get; private set; }
    public FakeCommandWithResult? LastCommand { get; private set; }

    public Task<string> HandleAsync(FakeCommandWithResult command, CancellationToken cancellationToken = default)
    {
        HandleCallCount++;
        LastCommand = command;
        return Task.FromResult(resultToReturn);
    }
}
