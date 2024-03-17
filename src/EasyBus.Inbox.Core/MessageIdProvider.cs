namespace EasyBus.Inbox.Core;

public sealed class MessageIdProvider<T> : IMessageIdProvider<T>
{
    private readonly Func<T, string> _provider;

    public MessageIdProvider(Func<T,string> provider)
    {
        _provider = provider;
    }
    
    public string GetId(T val) => _provider(val);
}