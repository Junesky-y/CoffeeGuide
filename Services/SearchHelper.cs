using System.Globalization;

namespace CoffeeGuide.Services;

public static class SearchHelper
{
    /// <summary>
    /// Повертає true, якщо КОЖНЕ слово запиту зустрічається хоча б в одному з полів (без урахування регістру).
    /// Порожній запит збігається з усім.
    /// </summary>
    public static bool Matches(string? query, params string?[] fields)
    {
        if (string.IsNullOrWhiteSpace(query)) return true;

        var haystack = string.Join(" ", fields.Where(f => !string.IsNullOrWhiteSpace(f)));
        var compare = CultureInfo.CurrentCulture.CompareInfo;

        return query
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .All(token => compare.IndexOf(haystack, token, CompareOptions.IgnoreCase) >= 0);
    }
}
