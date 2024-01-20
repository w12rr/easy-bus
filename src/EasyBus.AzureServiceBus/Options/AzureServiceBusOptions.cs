﻿namespace EasyBus.AzureServiceBus.Options;

public sealed class AzureServiceBusOptions
{
    public required IDictionary<string, ServiceBusConnectionOptions> Connections { get; init; }
}