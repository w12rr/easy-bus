namespace EasyBus.InMemory.Receivers;

public interface IInMemoryMessageHandler<in T>
{
    Task MessageHandler(T @event);
}