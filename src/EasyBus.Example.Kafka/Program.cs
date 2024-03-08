using Confluent.Kafka;
using EasyBus.Example.Kafka;
using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.Transports.Kafka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

        config.AddPublisher(pub => { pub.AddKafkaEventPublisher<TestEvent>("kafka_name", "my-topic"); });

        config.AddReceiver(rec => { rec.AddKafkaReceiver<TestEvent>("kafka_name", "my-topic", "consumer_name"); });
    });

    services.AddHostedService<LocalRunner>();
});
var app = builder.Build();

await app.RunAsync();

Console.WriteLine("End");
Console.ReadKey();