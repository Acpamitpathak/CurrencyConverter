using System;
using System.Text.RegularExpressions;

public static class RegexReplacer
{
    // Default regex pattern to use if none is provided
    private static readonly string DefaultPattern = @"Email:\s+\S+";

    /// <summary>
    /// Replaces all matches of the regex pattern (default or passed) in the content with the replacement string.
    /// </summary>
    /// <param name="content">The original content string</param>
    /// <param name="replacer">Replacement string</param>
    /// <param name="pattern">Optional: Regex pattern to use. If null or empty, default is used.</param>
    /// <returns>Updated string with replacements applied</returns>
    public static string ReplaceMatchingContent(string content, string replacer, string pattern = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be null or empty.", nameof(content));

        string regexPattern = string.IsNullOrWhiteSpace(pattern) ? DefaultPattern : pattern;

        try
        {
            var regex = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.Multiline);
            return regex.Replace(content, replacer);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException("Invalid regex pattern.", ex);
        }
    }
}