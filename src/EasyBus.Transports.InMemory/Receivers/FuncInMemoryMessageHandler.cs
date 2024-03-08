namespace EasyBus.Transports.InMemory.Receivers;

public class FuncInMemoryMessageHandler<T> : IInMemoryMessageHandler<T>
{
    private readonly Func<T, Task> _onSuccess;

    public FuncInMemoryMessageHandler(Func<T, Task> onSuccess)
    {
        _onSuccess = onSuccess;
    }

    public async Task MessageHandler(T @event) => await _onSuccess(@event);
}