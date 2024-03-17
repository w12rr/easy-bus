namespace EasyBus.Inbox.Core;

public interface IMessageIdProvider<in T>
{
    string GetId(T val);
}