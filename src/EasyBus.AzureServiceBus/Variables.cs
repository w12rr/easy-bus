namespace EasyBus.AzureServiceBus;

public static class Variables
{
    public static string TopicTemplate(string prefix, string topicName) => $"{prefix}-{topicName}";

    public static string SubscriptionTemplate(string prefix, string topicName, string suffix) =>
        $"{prefix}-{topicName}-{suffix}";

    public static string ConfigurationPath(string name) => $"Mq:AzureServiceBus:{name}";
}