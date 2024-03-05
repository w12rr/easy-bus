namespace EasyBus.Inbox.Core;

public interface IInboxConfiguration<out TTransportConfiguration, out T>
{
    TTransportConfiguration SetInboxFuncHandler(
        Func<IServiceProvider, T, CancellationToken, Task<InboxMessageAction>> handler);
    TTransportConfiguration SetInboxHandler<THandler>()
        where THandler : class, IInboxMessageReceiver<T>;
}