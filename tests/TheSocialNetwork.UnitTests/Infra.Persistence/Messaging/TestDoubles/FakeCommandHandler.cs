using TheSocialNetwork.Application.Abstractions.Messaging;

namespace TheSocialNetwork.UnitTests.Infra.Persistence.Messaging.TestDoubles;

public sealed record FakeCommand(string Value) : ICommand;

public sealed class FakeCommandHandler : ICommandHandler<FakeCommand>
{
    public int HandleCallCount { get; private set; }
    public FakeCommand? LastCommand { get; private set; }

    public Task HandleAsync(FakeCommand command, CancellationToken cancellationToken = default)
    {
        HandleCallCount++;
        LastCommand = command;
        return Task.CompletedTask;
    }
}
