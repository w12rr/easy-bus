using Confluent.Kafka;
using EasyBus.Core.Publishing;
using EasyBus.Example.Kafka;
using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.Transports.Kafka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

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
        pub.AddKafkaEventPublisher<TestEvent>("kafka_name", "my-topic");
    });

    config.AddReceiver(rec =>
    {
        rec.AddKafkaReceiver<TestEvent>("kafka_name", "topic", "consumer_name");
    });
});

await using var sp = services.BuildServiceProvider();
var publisher = sp.GetRequiredService<IPublisher>();

await publisher.Publish(new TestEvent(Guid.NewGuid(), "this is message"), CancellationToken.None);
