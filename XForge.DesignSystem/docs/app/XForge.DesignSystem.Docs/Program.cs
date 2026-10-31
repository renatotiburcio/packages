using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using XForge.DesignSystem.Components.Feedback.Toast;
using XForge.DesignSystem.Components.Overlays.Dialog;
using XForge.DesignSystem.Components.Overlays.Drawer;
using XForge.DesignSystem.Components.Utilities.BreakpointObserver;
using XForge.DesignSystem.Docs;
using XForge.DesignSystem.Docs.Shared.Contracts;
using XForge.DesignSystem.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<NavigationStateService>();
builder.Services.AddScoped<XForgeDocsLanguageService>();
builder.Services.AddSingleton(new XForgeDocsProductContext(XForgeDocsProduct.Core));
builder.Services.AddSingleton<BrandMetadataService>();
builder.Services.AddScoped<XFDialogService>();
builder.Services.AddScoped<XFDrawerService>();
builder.Services.AddScoped<XFToastService>();
builder.Services.AddScoped<XFNavigationGuardService>();
builder.Services.AddSingleton<XFBreakpointService>();
builder.Services.Configure<BrandMetadataOptions>(options =>
{
    options.Brands["components"] = new BrandMetadata("Components", "#6366f1", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("layer-group"));
    options.Brands["display"] = new BrandMetadata("Data Display", "#0ea5e9", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("table-cells"));
    options.Brands["forms"] = new BrandMetadata("Forms", "#ec4899", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("input-text"));
    options.Brands["feedback"] = new BrandMetadata("Feedback", "#f59e0b", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("message-exclamation"));
    options.Brands["layout"] = new BrandMetadata("Layout and Shell", "#10b981", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("window-frame"));
    options.Brands["navigation"] = new BrandMetadata("Navigation", "#14b8a6", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("route"));
    options.Brands["overlays"] = new BrandMetadata("Overlays", "#f97316", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("window-restore"));
    options.Brands["tokens"] = new BrandMetadata("Design Tokens", "#8b5cf6", XForge.DesignSystem.Components.Icon.XFSvgIcons.Get("palette"));
});

await builder.Build().RunAsync();
