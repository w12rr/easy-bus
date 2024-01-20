namespace SharedCore.Messaging.Core.Receiving;

public interface IReceiver
{
    Task StartReceiving(CancellationToken cancellationToken);
    Task StopReceiving(CancellationToken cancellationToken);
}