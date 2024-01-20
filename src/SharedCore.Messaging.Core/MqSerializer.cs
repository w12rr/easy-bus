using System.Text.Json;
using System.Text.Json.Serialization;
using SharedCore.Abstraction.Services;

namespace SharedCore.Messaging.Core;

public static class MqSerializer
{
    public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, GetSettings());
    public static T Deserialize<T>(string serialized) => JsonSerializer.Deserialize<T>(serialized, GetSettings()).AssertNull();

    private static JsonSerializerOptions GetSettings() => new()
    {
        Converters =
        {
            new JsonStringEnumConverter()
        },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}