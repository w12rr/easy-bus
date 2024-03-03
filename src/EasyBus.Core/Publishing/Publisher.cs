using EasyBus.Core.InfrastructureWrappers;

namespace EasyBus.Core.Publishing;

public sealed class Publisher : IPublisher
{
    private readonly IReadOnlyCollection<IInfrastructurePublisher> _eventPublishingDefinitions;

    public Publisher(IReadOnlyCollection<IInfrastructurePublisher> eventPublishingDefinitions)
    {
        _eventPublishingDefinitions = eventPublishingDefinitions;
    }
    
    public async Task Publish<T>(T @event, CancellationToken cancellationToken)
    {
        var definition = _eventPublishingDefinitions.OfType<IInfrastructurePublisher<T>>().Single();
        await definition.Publish(@event, cancellationToken);
    }
}