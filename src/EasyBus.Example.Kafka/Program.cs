using System.Text.Json;
using Confluent.Kafka;
using EasyBus.Example.Kafka;
using EasyBus.Inbox.Core;
using EasyBus.Inbox.Infrastructure;
using EasyBus.Inbox.Transports.Kafka;
using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.Transports.Kafka.DependencyInjection;
using EasyBux.Outbox.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Outbox.Databases.SqlServer;

var builder = new HostBuilder();
builder.ConfigureServices(services =>
{
    services.AddLogging(x => x.AddConsole());
    services.AddMessageQueue(config =>
    {
        config.AddKafka("kafka_name", options =>
        {
            options.BootstrapServers = new[] { "127.0.0.1:9092" };
            options.SaslMechanism = SaslMechanism.Plain;
            options.SecurityProtocol = SecurityProtocol.Plaintext;
        });

        config.AddPublisher(pub =>
        {
            pub.AddKafkaEventPublisher<TestEvent>("kafka_name", "my-stopic");

            pub.AddOutboxServices(options =>
            {
                options.UseSqlServer();
                options.UseConnectionString(
                    "Server=localhost;Database=easy-bus;User Id=sa;Password=StrongPASSWORD123!@#;TrustServerCertificate=true");
            });
        });

        config.AddReceiver(rec =>
        {
            rec.AddKafkaReceiver<TestEvent>("kafka_name", "my-topic", "consumer_name")
                .SetInbox(x => x.Id.ToString())
                .SetInboxFuncHandler((sp, e, ct) =>
                {
                    Console.WriteLine("Received " + e.SomeData);
                    return Task.FromResult(InboxMessageState.Received);
                });
            rec.AddInboxMessageConsumer(
                "Server=localhost;Database=easy-bus;User Id=sa;Password=StrongPASSWORD123!@#;TrustServerCertificate=true");
        });
    });

    services.AddHostedService<LocalRunner>();
});
var app = builder.Build();

await app.RunAsync();

Console.WriteLine("End");
Console.ReadKey();