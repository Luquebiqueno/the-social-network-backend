namespace TheSocialNetwork.UnitTests.Infra.Persistence.Messaging.TestDoubles;

public sealed class FakeServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, object> _services = [];

    public FakeServiceProvider Register<TService>(TService instance) where TService : notnull
    {
        _services[typeof(TService)] = instance;
        return this;
    }

    public object? GetService(Type serviceType) => _services.GetValueOrDefault(serviceType);
}
