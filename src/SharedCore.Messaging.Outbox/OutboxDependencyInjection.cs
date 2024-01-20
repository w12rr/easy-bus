using Microsoft.EntityFrameworkCore;
using SharedCore.Messaging.Infrastructure.DependencyInjection;

namespace SharedCore.Messaging.Outbox;

public static class OutboxDependencyInjection
{
    public static void AddOutboxStore<TDbContext>(this PublisherConfiguration configuration)
        where TDbContext: DbContext
    {
        throw new NotImplementedException();
    }

    public static void AddOutboxMessageProducer<TDbContext>(this PublisherConfiguration configuration)
        where TDbContext: DbContext
    {
        throw new NotImplementedException();
    }
}