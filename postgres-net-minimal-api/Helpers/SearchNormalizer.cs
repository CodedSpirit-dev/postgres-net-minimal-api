using System.Globalization;
using System.Text;

namespace postgres_net_minimal_api.Helpers;

/// <summary>
/// Helper for normalizing search queries
/// </summary>
public static class SearchNormalizer
{
    /// <summary>
    /// Normalizes a search query for better matching
    /// </summary>
    /// <param name="query">The search query</param>
    /// <returns>Normalized query</returns>
    public static string Normalize(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return string.Empty;
        }

        var normalized = string.Create(query.Length, query, static (buffer, str) =>
        {
            str.AsSpan().ToLowerInvariant(buffer);
        });

        normalized = RemoveAccents(normalized);

        normalized = string.Join(" ", normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        normalized = normalized.Trim();

        return normalized;
    }

    /// <summary>
    /// Splits a search query into individual terms
    /// </summary>
    /// <param name="query">The search query</param>
    /// <returns>Array of search terms</returns>
    public static string[] GetSearchTerms(string query)
    {
        var normalized = Normalize(query);

        if (string.IsNullOrEmpty(normalized))
        {
            return [];
        }

        return normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Generates a PostgreSQL full-text search query
    /// </summary>
    /// <param name="query">The search query</param>
    /// <returns>PostgreSQL tsquery format</returns>
    public static string ToPostgresFullTextQuery(string query)
    {
        var terms = GetSearchTerms(query);

        if (terms.Length == 0)
        {
            return string.Empty;
        }

        // Join terms with AND operator for PostgreSQL full-text search
        return string.Join(" & ", terms.Select(t => $"{t}:*"));
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
}
