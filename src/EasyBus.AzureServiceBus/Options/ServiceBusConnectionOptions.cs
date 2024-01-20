namespace EasyBus.AzureServiceBus.Options;

public sealed class ServiceBusConnectionOptions
{
    public required string ConnectionString { get; init; }
    public required string Prefix { get; init; }
    public required string? SubscriptionSuffix { get; init; } 
}