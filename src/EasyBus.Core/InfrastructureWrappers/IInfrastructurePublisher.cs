namespace EasyBus.Core.InfrastructureWrappers;

public interface IInfrastructurePublisher
{
    Task Publish(object @event, CancellationToken cancellationToken);
}

public interface IInfrastructurePublisher<in T> : IInfrastructurePublisher
{
    Task Publish(T @event, CancellationToken cancellationToken);
}