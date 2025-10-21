using System.Text.RegularExpressions;

namespace postgres_net_minimal_api.Helpers;

/// <summary>
/// Helper for sanitizing HTML content to prevent XSS attacks
/// </summary>
public static partial class HtmlSanitizer
{
    private static readonly HashSet<string> AllowedTags =
    [
        "p", "br", "strong", "em", "u", "h1", "h2", "h3", "h4", "h5", "h6",
        "ul", "ol", "li", "blockquote", "code", "pre", "a", "img"
    ];

    private static readonly HashSet<string> AllowedAttributes =
    [
        "href", "src", "alt", "title", "class"
    ];

    /// <summary>
    /// Sanitizes HTML content by removing potentially dangerous tags and attributes
    /// </summary>
    /// <param name="html">The HTML content to sanitize</param>
    /// <returns>Sanitized HTML</returns>
    public static string Sanitize(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        // Remove script tags and their content
        html = ScriptTagRegex().Replace(html, string.Empty);

        // Remove style tags and their content
        html = StyleTagRegex().Replace(html, string.Empty);

        // Remove iframe tags
        html = IframeTagRegex().Replace(html, string.Empty);

        // Remove event handlers (onclick, onload, etc.)
        html = EventHandlerRegex().Replace(html, string.Empty);

        // Remove javascript: protocol in links
        html = JavascriptProtocolRegex().Replace(html, string.Empty);

        return html;
    }

    /// <summary>
    /// Extracts plain text from HTML
    /// </summary>
    /// <param name="html">The HTML content</param>
    /// <param name="maxLength">Maximum length of the result (0 for unlimited)</param>
    /// <returns>Plain text</returns>
    public static string StripHtml(string html, int maxLength = 0)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        // Remove HTML tags
        var text = HtmlTagRegex().Replace(html, string.Empty);

        // Decode HTML entities
        text = System.Net.WebUtility.HtmlDecode(text);

        // Normalize whitespace
        text = MultipleWhitespaceRegex().Replace(text, " ").Trim();

        // Truncate if needed
        if (maxLength > 0 && text.Length > maxLength)
        {
            text = text[..maxLength].TrimEnd() + "...";
        }

        return text;
    }

    /// <summary>
    /// Generates an excerpt from HTML content
    /// </summary>
    /// <param name="html">The HTML content</param>
    /// <param name="maxLength">Maximum length of the excerpt (default 160)</param>
    /// <returns>Excerpt text</returns>
    public static string GenerateExcerpt(string html, int maxLength = 160)
    {
        return StripHtml(html, maxLength);
    }

    [GeneratedRegex(@"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", RegexOptions.IgnoreCase)]
    private static partial Regex ScriptTagRegex();

    [GeneratedRegex(@"<style\b[^<]*(?:(?!<\/style>)<[^<]*)*<\/style>", RegexOptions.IgnoreCase)]
    private static partial Regex StyleTagRegex();

    [GeneratedRegex(@"<iframe\b[^<]*(?:(?!<\/iframe>)<[^<]*)*<\/iframe>", RegexOptions.IgnoreCase)]
    private static partial Regex IframeTagRegex();

    [GeneratedRegex(@"\s*on\w+\s*=\s*[""'][^""']*[""']", RegexOptions.IgnoreCase)]
    private static partial Regex EventHandlerRegex();

    [GeneratedRegex(@"javascript:", RegexOptions.IgnoreCase)]
    private static partial Regex JavascriptProtocolRegex();

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleWhitespaceRegex();
}
