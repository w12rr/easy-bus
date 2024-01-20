using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Features.DistributedLock.AzureBlob;
using SharedCore.Messaging.AzureServiceBus.DependencyInjection;
using SharedCore.Messaging.Inbox;
using SharedCore.Messaging.Inbox.Persistence;
using SharedCore.Messaging.Inbox.Persistence.Entities;
using SharedCore.Messaging.Infrastructure.DependencyInjection;
using SharedCore.Messaging.Outbox;

namespace SharedCore.Messaging.Example;

public static class Program
{
    public static async Task Main()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

        services.AddAzureBlobDistributedLock(configuration.GetSection("DistributedLock:AzureBlob"));
        services.AddMessageQueue(config =>
        {
            config.AddAzureServiceBus("Asd", configuration.GetSection("Mq:AzureServiceBus:Asd"));

            config.AddPublisher(pub =>
            {
                pub.AddAzureServiceBusEventPublisher<SomeEvent>("Asd");
                pub.AddOutboxStore<AppDbContext>();
                pub.AddOutboxMessageProducer<AppDbContext>();
            });

            config.AddReceiver(rec =>
            {
                rec.AddAzureServiceBusEventReceiver<SomeEvent>("Asd");
                rec.AddInboxStore<AppDbContext>();
                rec.AddInboxMessageConsumer<AppDbContext>();
            });
        });

        await using var sp = services.BuildServiceProvider();
        var publisher = sp.GetRequiredService<IPublisher>();

        await publisher.Publish(new SomeEvent(), CancellationToken.None);
    }
}

public sealed class AppDbContext : DbContext, IInboxDbContext
{
    public DbSet<InboxMessage> InboxMessages { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureInboxEntity();
        base.OnModelCreating(modelBuilder);
    }
}

public sealed record SomeEvent
{
}