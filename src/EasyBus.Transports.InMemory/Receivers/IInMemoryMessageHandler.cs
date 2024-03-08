namespace EasyBus.Transports.InMemory.Receivers;

public interface IInMemoryMessageHandler<in T>
{
    Task MessageHandler(T @event);
}