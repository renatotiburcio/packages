using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Services.AI;
using XForge.DesignSystem.Components.Utilities.Timing;
using XForge.DesignSystem.Components.Utilities.BreakpointObserver;
using XForge.DesignSystem.Components.Icon;
using XForge.Localization;
using XForge.Localization.Json;
namespace XForge.DesignSystem.Services;
public static class XForgeServiceCollectionExtensions
{
    public static IServiceCollection AddXForgeComponents(this IServiceCollection services)
    {
        // Debouncer e Throttler exigem (delayMs, Action) no construtor. Registrar o tipo
        // puro faz o provider explodir na validacao (ambiente de desenvolvimento) com
        // "Unable to resolve service for type 'System.Int32'". A factory entrega os dois
        // argumentos; a acao vazia e o padrao — quem precisa de comportamento cria a sua.
        services.AddSingleton(sp => new Debouncer(250, () => { }));
        services.AddSingleton(sp => new Throttler(250, () => { }));

        // XFBreakpointService consome IJSRuntime, que e scoped no Blazor Server e
        // singleton no WASM. Scoped resolve nos dois casos (scoped pode consumir
        // singleton); singleton quebraria no Blazor Server em validacao.
        services.AddScoped<XFBreakpointService>();

        // AIChatService depende de HttpClient — sem este registro o servico nao resolve.
        if (!services.Any(d => d.ServiceType == typeof(HttpClient)))
        {
            services.AddSingleton<HttpClient>();
        }

        services.AddSingleton<IAIChatService, AIChatService>();

        // AIChatService consome IOptions<AIChatServiceOptions>.
        services.AddOptions<AIChatServiceOptions>();

        // Opcoes sempre disponiveis, independente do overload usado.
        if (!services.Any(d => d.ServiceType == typeof(XForgeComponentsOptions)))
        {
            services.AddSingleton(new XForgeComponentsOptions());
        }

        // Localization
        services.AddSingleton<XFJsonLocaleProvider>(sp =>
        {
            var provider = new XFJsonLocaleProvider();
            LoadLocale(provider, "pt-BR");
            LoadLocale(provider, "en");
            LoadLocale(provider, "es");
            return provider;
        });
        services.AddSingleton<IXFLocalizer, XFLocalizer>();

        // Icon registry
        services.AddSingleton<IXFIconRegistry, XFIconRegistry>();

        // Time provider (migrated from AddHxServices)
        services.AddSingleton(TimeProvider.System);

        return services;
    }

    private static void LoadLocale(XFJsonLocaleProvider provider, string culture)
    {
        var assembly = typeof(XForgeServiceCollectionExtensions).Assembly;
        var resourceName = $"XForge.DesignSystem.Localization.Json.Locales.{culture}.json";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null) return;
        using var reader = new System.IO.StreamReader(stream);
        var json = reader.ReadToEnd();
        provider.RegisterLocaleFromJson(culture, json);
    }

    public static IServiceCollection AddXForgeComponents(this IServiceCollection services, Action<XForgeComponentsOptions> configure)
    {
        var opts = new XForgeComponentsOptions(); configure(opts);
        services.AddSingleton(opts);
        return services.AddXForgeComponents();
    }
}
public class XForgeComponentsOptions { public bool EnableAiChat { get; set; } = true; public int MaxSessions { get; set; } = 1000; public int DebounceMs { get; set; } = 250; }