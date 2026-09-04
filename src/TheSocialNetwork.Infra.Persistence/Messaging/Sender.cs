using Microsoft.Extensions.DependencyInjection;
using TheSocialNetwork.Application.Abstractions.Messaging;

namespace TheSocialNetwork.Infra.Persistence.Messaging;

internal sealed class Sender(IServiceProvider serviceProvider) : ISender
{
    public Task SendAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);

        var handler = serviceProvider
            .GetRequiredService<ICommandHandler<TCommand>>();

        return handler.HandleAsync(command, cancellationToken);
    }

    public Task<TResult> SendAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        ArgumentNullException.ThrowIfNull(command);

        var handler = serviceProvider
            .GetRequiredService<ICommandHandler<TCommand, TResult>>();

        return handler.HandleAsync(command, cancellationToken);
    }

    public Task<TResult> QueryAsync<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        ArgumentNullException.ThrowIfNull(query);

        var handler = serviceProvider
            .GetRequiredService<IQueryHandler<TQuery, TResult>>();

        return handler.HandleAsync(query, cancellationToken);
    }
}
