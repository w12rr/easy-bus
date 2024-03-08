using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Transports.InMemory.Channels;

namespace EasyBus.Transports.InMemory.Receivers;

public class ChannelReceiver<T> : IInfrastructureReceiver
{
    private readonly IChannelsStore _channelsStore;
    private readonly IInMemoryMessageHandler<T> _messageHandler;

    public ChannelReceiver(IChannelsStore channelsStore, IInMemoryMessageHandler<T> messageHandler)
    {
        _channelsStore = channelsStore;
        _messageHandler = messageHandler;
    }

    public async Task StartReceiving(CancellationToken cancellationToken)
    {
        var channel = _channelsStore.GetChannel<T>();
        if (channel is null)
        {
            channel = _channelsStore.CreateChannel<T>();
        }

        await foreach (var @event in channel.Reader.ReadAllAsync(cancellationToken))
        {
            await _messageHandler.MessageHandler(@event);
        }
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}