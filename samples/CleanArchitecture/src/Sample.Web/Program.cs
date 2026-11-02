using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Sample.Application.Clientes;
using Sample.Domain;
using Sample.Infrastructure;
using XForge.DesignSystem.Components.Feedback.Toast;
using XForge.DesignSystem.Components.Overlays.Dialog;
using XForge.DesignSystem.Components.Overlays.Drawer;
using XForge.DesignSystem.Components.Utilities.BreakpointObserver;
using XForge.DesignSystem.Services;
using XForge.Localization.Json;
using XForge.Mediator;
using XForge.Mediator.Behaviors;
using XForge.Mediator.Generated;
using XForge.Validation;

var builder = WebApplication.CreateBuilder(args);

// ---- Design System: localizacao pt-BR/en/es, registro de icones, HttpClient, opcoes ----
builder.Services.AddXForgeComponents();

// ---- Camadas da Clean Architecture ----
builder.Services.AddSingleton<IClienteRepository, InMemoryClienteRepository>(); // Infrastructure
builder.Services.AddXForgeValidation(typeof(CriarClienteValidator).Assembly);   // Validation (scanning)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); // Behaviors
builder.Services.AddXForgeMediatorGenerated();                                 // Mediator (source generator)

// ---- Servicos de shell do design system ----
builder.Services.AddScoped<NavigationStateService>();
builder.Services.AddSingleton<BrandMetadataService>();
builder.Services.AddScoped<XFDialogService>();
builder.Services.AddScoped<XFDrawerService>();
builder.Services.AddScoped<XFToastService>();
builder.Services.AddSingleton<XFBreakpointService>();

// ---- Marcas do shell (o rail usa a cor/icone da marca ativa) ----
builder.Services.Configure<BrandMetadataOptions>(options =>
{
    options.Brands["clientes"] = new BrandMetadata("Clientes", "#0ea5e9",
        XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("users"));
    options.Brands["arquitetura"] = new BrandMetadata("Arquitetura", "#10b981",
        XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("layer-group"));
});

// ---- Blazor: render interativo no servidor (sem projeto WASM separado) ----
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// AddXForgeComponents carrega os locales pt-BR/en/es, mas o provider nasce com
// CurrentCulture = "en". Sem escolher pt-BR aqui, as chaves caem no locale ingles
// e voltam cruas no markup (ex.: id="appshell.main_id").
app.Services.GetRequiredService<XFJsonLocaleProvider>().CurrentCulture = "pt-BR";

app.UseStaticFiles();
app.UseAntiforgery();

// Dados de demonstracao: o repositorio e em memoria e nasce vazio, entao sem este
// seed o grid nunca mostraria linhas. Marcado como demo — remover em uso real.
using (var seed = app.Services.CreateScope())
{
    var repositorio = seed.ServiceProvider.GetRequiredService<IClienteRepository>();
    if (repositorio.List().Count == 0)
    {
        repositorio.Add(new Cliente { Nome = "Maria Silva", Email = "maria@empresa.com" });
        repositorio.Add(new Cliente { Nome = "Joao Souza", Email = "joao@empresa.com" });
        repositorio.Add(new Cliente { Nome = "Ana Costa", Email = "ana@empresa.com" });
        repositorio.Add(new Cliente { Nome = "Pedro Lima", Email = "pedro@empresa.com" });
    }
}

app.MapRazorComponents<Sample.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
