using System.Text.Json;
using Confluent.Kafka;
using EasyBus.Inbox.Core;
using EasyBus.Inbox.Databases.SqlServer;
using EasyBus.Inbox.Infrastructure;
using EasyBus.Inbox.Transports.Kafka;
using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.Outbox.Databases.SqlServer;
using EasyBus.Transports.Kafka.DependencyInjection;
using EasyBux.Outbox.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Example.Kafka;

public static class DependencyInjectionExtensions
{
    public static void AddTestMessageQueue(this IServiceCollection services)
    {
        services.AddMessageQueue(config =>
        {
            config.AddKafka("kafka_name", options =>
            {
                options.BootstrapServers = new[] { "127.0.0.1:9092" };
                options.SaslMechanism = SaslMechanism.Plain;
                options.SecurityProtocol = SecurityProtocol.Plaintext;
                options.ProducerConfigInterceptor = x => { x.SaslMechanism = SaslMechanism.Plain; };
            });

            config.AddPublisher(pub =>
            {
                pub.AddKafkaEventPublisher<TestEvent>(opt =>
                {
                    opt.SetPublisher("kafka_name", "my-topic");
                    opt.SetMessageSerializer(x => JsonSerializer.Serialize(x, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));
                });

                pub.AddOutboxServices(opt =>
                {
                    opt.UseSqlServer();
                    opt.UseNamedConnectionString("DefaultConnection");
                });
            });

            config.AddReceiver(rec =>
            {
                rec.AddKafkaReceiver<TestEvent>(kafkaRecConfig =>
                {
                    kafkaRecConfig.SetConsumer("kafka_name", "my-topic", "consumer_name");
                    kafkaRecConfig.UseInbox(conf =>
                    {
                        conf.SetInboxFuncHandler((sp, e, ct) =>
                        {
                            Console.WriteLine("Received " + e.SomeData);
                            return Task.FromResult(InboxMessageState.Received);
                        });
                        conf.SetMessageIdProvider(x => x.Id.ToString());
                    });
                });
                rec.AddInboxMessageConsumer(opt =>
                {
                    opt.UseSqlServer();
                    opt.UseNamedConnectionString("DefaultConnection");
                });
            });
        });
    }
}