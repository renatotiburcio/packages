using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using XForge.DesignSystem.Components.Feedback.Toast;
using XForge.DesignSystem.Components.Overlays.Dialog;
using XForge.DesignSystem.Components.Overlays.Drawer;
using XForge.DesignSystem.Components.Utilities.BreakpointObserver;
using XForge.DesignSystem.Docs.AI;
using XForge.DesignSystem.Docs.Shared.Contracts;
using XForge.DesignSystem.Extensions;
using XForge.DesignSystem.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<NavigationStateService>();
builder.Services.AddScoped<XForgeDocsLanguageService>();
builder.Services.AddSingleton(new XForgeDocsProductContext(XForgeDocsProduct.AI));
builder.Services.AddSingleton<BrandMetadataService>();
builder.Services.AddScoped<XFDialogService>();
builder.Services.AddScoped<XFDrawerService>();
builder.Services.AddScoped<XFToastService>();
builder.Services.AddScoped<XFNavigationGuardService>();
builder.Services.AddSingleton<XFBreakpointService>();
builder.Services.Configure<BrandMetadataOptions>(options =>
{
    options.Brands["ai-core"] = new BrandMetadata("AI Core", "#7c3aed", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("sparkles"));
    options.Brands["ai-agents"] = new BrandMetadata("Agents", "#059669", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("diagram-project"));
    options.Brands["ai-business"] = new BrandMetadata("Business AI", "#0284c7", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("briefcase"));
    options.Brands["ai-multimodal"] = new BrandMetadata("Multimodal", "#db2777", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("eye"));
    options.Brands["ai-governance"] = new BrandMetadata("Governance", "#d97706", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("shield-check"));
});
builder.Services.AddXFAi();
builder.Services.AddXFAiMultiModal();

await builder.Build().RunAsync();
