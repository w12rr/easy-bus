using System.Threading.Channels;

namespace EasyBus.Transports.InMemory.Channels;

public class ChannelsStore : IChannelsStore
{
    private readonly Dictionary<Type, object> _channels = new();

    public Channel<T>? GetChannel<T>() => _channels.GetValueOrDefault(typeof(T)) as Channel<T>;

    public Channel<T> CreateChannel<T>()
    {
        if (GetChannel<T>() is not null)
        {
            throw new Exception("Channel already exists");
        }

        var channel = Channel.CreateUnbounded<T>();
        _channels.Add(typeof(T), channel);
        return channel;
    }
}