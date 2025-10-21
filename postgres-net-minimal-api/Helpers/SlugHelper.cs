using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace postgres_net_minimal_api.Helpers;

/// <summary>
/// Helper for generating SEO-friendly URL slugs
/// </summary>
public static partial class SlugHelper
{
    /// <summary>
    /// Generates a URL-friendly slug from a string
    /// </summary>
    /// <param name="text">The text to slugify</param>
    /// <param name="maxLength">Maximum length of the slug (default 100)</param>
    /// <returns>URL-friendly slug</returns>
    public static string GenerateSlug(string text, int maxLength = 100)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        // Convert to lowercase
        var slug = text.ToLowerInvariant();

        // Remove accents
        slug = RemoveAccents(slug);

        // Replace spaces with hyphens
        slug = slug.Replace(" ", "-");

        // Remove invalid characters (keep only letters, numbers, and hyphens)
        slug = InvalidCharsRegex().Replace(slug, string.Empty);

        // Replace multiple hyphens with single hyphen
        slug = MultipleHyphensRegex().Replace(slug, "-");

        // Trim hyphens from start and end
        slug = slug.Trim('-');

        // Truncate to max length
        if (slug.Length > maxLength)
        {
            slug = slug[..maxLength].TrimEnd('-');
        }

        return slug;
    }

    /// <summary>
    /// Generates a unique slug by appending a suffix if needed
    /// </summary>
    /// <param name="baseSlug">The base slug</param>
    /// <param name="checkExists">Function to check if slug already exists</param>
    /// <param name="excludeId">Optional ID to exclude from uniqueness check</param>
    /// <returns>Unique slug</returns>
    public static async Task<string> GenerateUniqueSlugAsync(
        string baseSlug,
        Func<string, Guid?, Task<bool>> checkExists,
        Guid? excludeId = null)
    {
        var slug = baseSlug;
        var counter = 1;

        while (await checkExists(slug, excludeId))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    private static string RemoveAccents(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex(@"[^a-z0-9\-]")]
    private static partial Regex InvalidCharsRegex();

    [GeneratedRegex(@"-{2,}")]
    private static partial Regex MultipleHyphensRegex();
}
