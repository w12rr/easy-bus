namespace EasyBus.Inbox.Core;

public interface IMissingOutboxTableCreator
{
    Task Create(CancellationToken cancellationToken);
}