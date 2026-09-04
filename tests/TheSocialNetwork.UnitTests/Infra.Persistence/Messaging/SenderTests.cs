using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Infra.Persistence.Messaging;
using TheSocialNetwork.UnitTests.Infra.Persistence.Messaging.TestDoubles;

namespace TheSocialNetwork.UnitTests.Infra.Persistence.Messaging;

public class SenderTests
{
    [Fact]
    public async Task SendAsync_ShouldInvokeResolvedHandler_WhenCommandHasNoResult()
    {
        var handler = new FakeCommandHandler();
        var serviceProvider = new FakeServiceProvider()
            .Register<ICommandHandler<FakeCommand>>(handler);
        var sender = new Sender(serviceProvider);
        var command = new FakeCommand("value");

        await sender.SendAsync(command);

        Assert.Equal(1, handler.HandleCallCount);
        Assert.Same(command, handler.LastCommand);
    }

    [Fact]
    public async Task SendAsync_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        var sender = new Sender(new FakeServiceProvider());

        await Assert.ThrowsAsync<ArgumentNullException>(() => sender.SendAsync<FakeCommand>(null!));
    }

    [Fact]
    public async Task SendAsync_ShouldThrowInvalidOperationException_WhenNoHandlerIsRegistered()
    {
        var sender = new Sender(new FakeServiceProvider());
        var command = new FakeCommand("value");

        await Assert.ThrowsAsync<InvalidOperationException>(() => sender.SendAsync(command));
    }

    [Fact]
    public async Task SendAsync_ShouldReturnHandlerResult_WhenCommandHasResult()
    {
        var handler = new FakeCommandWithResultHandler("handled-value");
        var serviceProvider = new FakeServiceProvider()
            .Register<ICommandHandler<FakeCommandWithResult, string>>(handler);
        var sender = new Sender(serviceProvider);
        var command = new FakeCommandWithResult("value");

        var result = await sender.SendAsync<FakeCommandWithResult, string>(command);

        Assert.Equal("handled-value", result);
        Assert.Equal(1, handler.HandleCallCount);
        Assert.Same(command, handler.LastCommand);
    }

    [Fact]
    public async Task SendAsync_WithResult_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        var sender = new Sender(new FakeServiceProvider());

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sender.SendAsync<FakeCommandWithResult, string>(null!));
    }

    [Fact]
    public async Task SendAsync_WithResult_ShouldThrowInvalidOperationException_WhenNoHandlerIsRegistered()
    {
        var sender = new Sender(new FakeServiceProvider());
        var command = new FakeCommandWithResult("value");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sender.SendAsync<FakeCommandWithResult, string>(command));
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnHandlerResult()
    {
        var handler = new FakeQueryHandler("query-result");
        var serviceProvider = new FakeServiceProvider()
            .Register<IQueryHandler<FakeQuery, string>>(handler);
        var sender = new Sender(serviceProvider);
        var query = new FakeQuery("value");

        var result = await sender.QueryAsync<FakeQuery, string>(query);

        Assert.Equal("query-result", result);
        Assert.Equal(1, handler.HandleCallCount);
        Assert.Same(query, handler.LastQuery);
    }

    [Fact]
    public async Task QueryAsync_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        var sender = new Sender(new FakeServiceProvider());

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sender.QueryAsync<FakeQuery, string>(null!));
    }

    [Fact]
    public async Task QueryAsync_ShouldThrowInvalidOperationException_WhenNoHandlerIsRegistered()
    {
        var sender = new Sender(new FakeServiceProvider());
        var query = new FakeQuery("value");

        await Assert.ThrowsAsync<InvalidOperationException>(() => sender.QueryAsync<FakeQuery, string>(query));
    }
}
