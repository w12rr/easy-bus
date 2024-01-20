namespace SharedCore.Messaging.Infrastructure.Tools;

public static class TopicNames
{
    public static string NormalizeForType<T>()
    {
        var name = typeof(T).Name;
        return name.EndsWith(Variables.EventSuffix) ? name[^Variables.EventSuffix.Length..] : name; 
    }
}