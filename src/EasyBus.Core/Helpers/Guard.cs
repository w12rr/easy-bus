using System.Net;
using System.Text.RegularExpressions;

namespace EasyBus.Core.Helpers;

public static partial class Guard
{
    public static T AssertNull<T>(this T? t)
    {
        return t ?? throw new ArgumentNullException(nameof(t), $"{typeof(T).Name} cannot be null");
    }
    
    public static Uri AssertAbsoluteUri(this string? s)
    {
        return Uri.TryCreate(s.AssertNull(), UriKind.Absolute, out var uri) 
            ? uri 
            : throw new ArgumentException($"Argument value {s} is not absolute uri");
    }

    
    public static string AssertDomainName(this string? s)
    {
        Dns.GetHostEntry(s.AssertNullOrWhiteSpace());
        return s!;
    }

    public static string AssertNullOrWhiteSpace(this string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            throw new ArgumentNullException(
                nameof(s),
                $"string cannot be null or white space, is null: {s is null}, s length: {(s ?? string.Empty).Length}");
        }

        return s;
    }

    public static string AssertAnyWhiteSpaces(this string s)
    {
        if (Regex.IsMatch(s, @"\s"))
        {
            throw new Exception($"String \"{s}\" cannot contain any whitespaces");
        }

        return s;
    }

    public static string AssertMail(this string? s)
    {
        var mailRegex = GetMailRegex();
        var whiteSpaceRegex = GetWhiteSpaceRegex();

        if (!mailRegex.IsMatch(s.AssertNull()) || whiteSpaceRegex.IsMatch(s!))
        {
            throw new ArgumentException($"Given string is not an email address: {s}");
        }

        return s!;
    }

    public static int AssertGreaterThan(this int i, int greaterThan)
    {
        if (i > greaterThan) return i;
        
        throw new ArgumentException($"Given integer ({i}) must be greater than {greaterThan}");
    }

    [GeneratedRegex("\\s")]
    private static partial Regex GetWhiteSpaceRegex();

    [GeneratedRegex("^.+\\@.+\\..+$")]
    private static partial Regex GetMailRegex();
}