using System.Text.RegularExpressions;

namespace XForge.Validation.Internal;

/// <summary>Formata templates de mensagem com placeholders {Nome}.</summary>
internal static class MessageFormatter
{
    private static readonly Regex PlaceholderRegex = new(@"\{([^}]+)\}", RegexOptions.Compiled);

    public static string Format(string template, IReadOnlyDictionary<string, object?> values)
    {
        return PlaceholderRegex.Replace(template, match =>
        {
            var key = match.Groups[1].Value;

            if (values.TryGetValue(key, out var value))
            {
                return value?.ToString() ?? string.Empty;
            }

            return match.Value; // placeholder desconhecido permanece literal
        });
    }
}
