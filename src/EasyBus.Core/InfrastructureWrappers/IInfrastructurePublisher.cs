namespace EasyBus.Core.InfrastructureWrappers;

public interface IInfrastructurePublisher
{
}

public interface IInfrastructurePublisher<in T> : IInfrastructurePublisher
{
    Task Publish(T @event, CancellationToken cancellationToken);
}