using System.IO;
using System.Text.Json;
using XForge.Localization;
using XForge.Localization.Json;

namespace XForge.DesignSystem.Tests;

/// <summary>
/// Localizer de teste que resolve o locale real (pt-BR.json) em vez de devolver a
/// chave — antes o stub mascarava chaves ausentes, que apareciam cruas no markup.
/// A chave continua sendo o fallback quando nao existe no locale.
/// </summary>
public sealed class TestLocalizer : IXFLocalizer
{
    private static readonly Lazy<Dictionary<string, string>> Strings = new(Load, isThreadSafe: true);

    public string this[string key]
        => Strings.Value.TryGetValue(key, out var value) ? value : key;

    public string Format(string key, params object[] args)
    {
        var value = this[key];
        return args.Length == 0 ? value : string.Format(value, args);
    }

    private static Dictionary<string, string> Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "pt-BR.json");

        if (!File.Exists(path))
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path));

        return parsed is null
            ? new Dictionary<string, string>(StringComparer.Ordinal)
            : new Dictionary<string, string>(parsed, StringComparer.Ordinal);
    }
}
