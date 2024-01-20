namespace SharedCore.Abstraction;

public static class StringExtensions
{
    public static string MakePascalCase(this string s)
    {
        var splitted = s.Split('.');
        var all = splitted.Select(x => x.MakePascalCaseInternal()).ToArray();
        return string.Join(string.Empty, all);
    }
    
    private static string MakePascalCaseInternal(this string s)
    {
        return s.Split("_").Aggregate(string.Empty, (seed, acc) => $"{seed}{acc[..1].ToUpper()}{acc[1..]}");
    }
}