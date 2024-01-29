using System.Text.RegularExpressions;

namespace Aymeeeric.Website.Framework.Extensions;

public static class StringExtensions
{
    public static bool IsNotNullOrEmpty(this string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    /// <summary>
    /// Passe string.IsNullOrEmpty en methode d'extension
    /// </summary>    
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }
    
    public static string RemoveFromStart(this string source, string stringToRemove)
    {
        if (source.StartsWith(stringToRemove)) return source.Substring(stringToRemove.Length);

        return source;
    }

    public static bool IsValidEmail(this string email)
    {
        var regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                              + "@"
                              + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");

        var match = regex.Match(email);
        return match.Success;
    }

    public static bool ContainsAny(this string source, params string[] values)
    {
        return values.Any(source.Contains);
    }

    public static bool NotContainsAny(this string source, params string[] values)
    {
        return !values.Any(source.Contains);
    }

    public static string ToPascalCaseInvariant(this string source)
    {
        if (source.IsNullOrEmpty())
            return source;

        if (source.Length == 1)
            return source.ToUpperInvariant();

        return $"{char.ToUpperInvariant(source[0])}{source[1..].ToLowerInvariant()}";
    }
}