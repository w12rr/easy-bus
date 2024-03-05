namespace EasyBus.Inbox.Core;

public interface IConsumingRunner
{
    Task Run(CancellationToken cancellationToken);
}