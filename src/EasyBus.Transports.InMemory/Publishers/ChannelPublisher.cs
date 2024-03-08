using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Transports.InMemory.Channels;

namespace EasyBus.Transports.InMemory.Publishers;

public class ChannelPublisher<T> : IInfrastructurePublisher<T>
{
    private readonly IChannelsStore _channelsStore;

    public ChannelPublisher(IChannelsStore channelsStore)
    {
        _channelsStore = channelsStore;
    }

    public async Task Publish(T @event, CancellationToken cancellationToken)
    {
        var channel = _channelsStore.GetChannel<T>();
        if (channel is null)
        {
            channel = _channelsStore.CreateChannel<T>();
        }

        await channel.Writer.WriteAsync(@event, cancellationToken);
    }

    public async Task Publish(object @event, CancellationToken cancellationToken)
    {
        await Publish((T)@event, cancellationToken);
    }
}