using EasyBus.Core.InfrastructureWrappers;

namespace EasyBus.Core.Publishing;

public sealed class Publisher : IPublisher
{
    private readonly IReadOnlyCollection<IInfrastructurePublisher> _eventPublishingDefinitions;

    public Publisher(IEnumerable<IInfrastructurePublisher> eventPublishingDefinitions)
    {
        _eventPublishingDefinitions = eventPublishingDefinitions.ToList().AsReadOnly();
    }
    
    public async Task Publish<T>(T @event, CancellationToken cancellationToken)
    {
        var definition = _eventPublishingDefinitions.OfType<IInfrastructurePublisher<T>>().Single();
        await definition.Publish(@event, cancellationToken);
    }
    
    public async Task Publish(object @event, CancellationToken cancellationToken)
    {
        var targetPublisherType = typeof(IInfrastructurePublisher<>).MakeGenericType(@event.GetType());
        var definition = _eventPublishingDefinitions.Single(x => targetPublisherType.IsInstanceOfType(x));
        await definition.Publish(@event, cancellationToken);
    }
}