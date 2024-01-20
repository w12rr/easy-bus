using System.Dynamic;

namespace SharedCore.Abstraction.Services.Dynamic;

public static class DynamicExtensions
{
    public static dynamic AsExpandoObject(this object? o)
    {
        switch (o)
        {
            case null:
                return new ExpandoObject();
            case ExpandoObject:
                return o;
        }

        var expando = new ExpandoObject();
        var dictionary = (IDictionary<string, object?>)expando;

        foreach (var property in o.GetType().GetProperties())
            dictionary.Add(property.Name, property.GetValue(o));

        return expando;
    }
    
    public static dynamic AsExpandoObject(this object? o, Func<string, string> namingStrategy)
    {
        switch (o)
        {
            case null:
                return new ExpandoObject();
            case ExpandoObject:
                return o;
        }

        var expando = new ExpandoObject();
        var dictionary = (IDictionary<string, object?>)expando;

        foreach (var property in o.GetType().GetProperties())
            dictionary.Add(namingStrategy(property.Name), property.GetValue(o));

        return expando;
    }
}