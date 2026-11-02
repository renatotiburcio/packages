using System.IO;
using System.Text.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.Icon;

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
        // O padrao de 1s do WaitForAssertion curte demais: sob a execucao paralela do
        // xUnit (~17 colecoes simultaneas), um render legitimo pode nao completar dentro
        // desse intervalo e o teste falha de forma intermitente sem nenhuma regressao.
        DefaultWaitTimeout = TimeSpan.FromSeconds(10);

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

        // XFIconPicker e os campos de icone consomem IXFIconRegistry por injecao.
        Services.AddXFIconRegistry();
    }
}
