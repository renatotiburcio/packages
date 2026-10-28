using System.IO;
using System.Text.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace XForge.DesignSystem.Tests;

/// <summary>
/// Contexto bUnit compartilhado: registra o localizer real carregado com pt-BR.json —
/// 56 componentes fazem <c>[Inject] IXFLocalizer?</c> e os testes assertam strings
/// localizadas em português.
/// </summary>
public class XForgeTestContext : BunitContext
{
    public XForgeTestContext()
    {
        var provider = new XForge.Localization.Json.XFJsonLocaleProvider();
        var path = Path.Combine(AppContext.BaseDirectory, "pt-BR.json");

        if (File.Exists(path))
        {
            var strings = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path));
            if (strings is not null)
            {
                provider.RegisterLocale("pt-BR", strings);
                provider.CurrentCulture = "pt-BR";
            }
        }

        Services.AddSingleton<XForge.Localization.IXFLocalizer>(new XForge.Localization.XFLocalizer(provider));
    }
}
