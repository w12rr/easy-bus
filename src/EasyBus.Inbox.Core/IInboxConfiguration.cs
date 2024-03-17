namespace EasyBus.Inbox.Core;

public interface IInboxConfiguration<out T>
{
    void SetInboxFuncHandler(
        Func<IServiceProvider, T, CancellationToken, Task<InboxMessageState>> handler);
    void SetInboxHandler<THandler>()
        where THandler : class, IInboxMessageReceiver<T>;

    void SetMessageIdProvider(Func<T, string> idProvider);
}