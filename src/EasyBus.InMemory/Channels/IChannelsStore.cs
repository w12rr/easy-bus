using System.Threading.Channels;

namespace EasyBus.InMemory.Channels;

public interface IChannelsStore
{
    Channel<T>? GetChannel<T>();
    Channel<T> CreateChannel<T>();
}