# EasyBus for .NET 
[![My Skills](https://skillicons.dev/icons?i=dotnet,cs,kafka,rabbitmq,docker)](https://skillicons.dev)

This project is under development and **IS NOT PRODUCTION READY**

## The purpose
1. The purpose of this project is to provide an abstraction code for ASB, 
Kafka, RabbitMQ, etc and provide easy switching between them
2. The second purpose is to provide the most easiest API, this can be described by: **You see the example code, you know how to use it**, let's try to achieve it 

## What have to be done
Investigate the final api that we will use, define the final project structure (POC have to be created).
The Api should look like that:

```csharp
var services = new ServiceCollection();

services.AddMessageQueue(config =>
{
    config.AddAzureServiceBus("Asb", asbOpt =>
    {
        asbOpt.SomeProp = "some-values";
        asbOpt.SomeProp2 = "some-values";
    });
    config.AddAzureServiceBus("Asb2", asbOpt =>
    {
        asbOpt.SomeProp = "some-values";
        asbOpt.SomeProp2 = "some-values";
    });
    config.AddKafka("kafka", kafkaOpt =>
    {
        kafkaOpt.SomeProp = "some-values";
        kafkaOpt.SomeProp2 = "some-values";
    });

    config.AddPublisher(pub =>
    {
        pub.AddAzureServiceBusEventPublisher<SomeEvent>("Asb");
        pub.AddKafkaEventPublisher<SomeEvent2>("kafka");
        pub.AddAzureServiceBusEventPublisher<SomeEvent3>("Asb2");
    });

    config.AddReceiver(rec =>
    {
        rec.AddAzureServiceBusEventReceiver<SomeEvent>("Asb");
    });
});

await using var sp = services.BuildServiceProvider();
var publisher = sp.GetRequiredService<IPublisher>();

await publisher.Publish(new SomeEvent(), CancellationToken.None);

public sealed record SomeEvent;
public sealed record SomeEvent2;
public sealed record SomeEvent3;
```



## Vision of the API
During startup (DI configuration), we define how particular events are received/published
by defining them one by one in configuration 

Then in the app logic, the event is sent by using publisher interface. The EasyBus know where to send it
by looking for configuration that was made during startup.

Similar behaviour is used by receiver. Here we define how event should be handled,
e.g. by sending it to the mediator (MediatR)

### Let's summarize.
During startup we tell how particular event is served (particular event I mean 
particular class/record that represent events)

Then we just send events (instance of classes/records) or telling how to receive event (class or record)

## Documentation/examples
Here will go examples, when library will be production ready :)

## Contributing
Wanna contributing? Contact me: dam.sibinski@gmail.com

Before any pull request please contact me, to talk about your solution