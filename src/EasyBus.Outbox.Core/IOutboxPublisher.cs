using Microsoft.Data.SqlClient;

namespace EasyBus.Outbox.Core;

public interface IOutboxPublisher
{
    Task Publish<T>(T @event, CancellationToken cancellationToken)
        where T : notnull;

    Task Publish<T>(SqlTransaction transaction, T @event, CancellationToken cancellationToken)
        where T : notnull;
}