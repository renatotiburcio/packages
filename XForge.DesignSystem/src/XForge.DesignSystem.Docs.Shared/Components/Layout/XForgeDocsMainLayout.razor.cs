using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using XForge.DesignSystem.Docs.Shared.Contracts;
using XForge.DesignSystem.Docs.Shared.Navigation;
using XForge.DesignSystem.Services;

namespace XForge.DesignSystem.Docs.Shared.Components.Layout;

public partial class XForgeDocsMainLayout : LayoutComponentBase, IDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private NavigationStateService NavigationState { get; set; } = default!;
    [Inject] private BrandMetadataService BrandMetadata { get; set; } = default!;
    [Inject] private XForgeDocsProductContext ProductContext { get; set; } = default!;
    [Inject] private XForgeDocsLanguageService LanguageService { get; set; } = default!;

    private bool _isDark;
    private int _languageRenderKey;

    private string Theme => _isDark ? "dark" : "light";
    private XForgeDocsProduct Product => ProductContext.Product;
    private string CurrentBrand => string.IsNullOrWhiteSpace(NavigationState.CurrentBrand)
        ? (XForgeDocsNavigation.IsAiProduct(Product) ? "ai" : "home")
        : NavigationState.CurrentBrand;
    private string SkipToContentLabel => LanguageService.Text("Pular para o conteudo principal", "Skip to main content");
    private string VisitorLabel => LanguageService.Text("Visitante", "Visitor");

    protected override void OnInitialized()
    {
        NavigationState.BrandChanged += OnBrandChanged;
        LanguageService.LanguageChanged += OnLanguageChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await LanguageService.InitializeAsync();
        await NavigationState.InitializeAsync(XForgeDocsNavigation.GetRailItems(Product));
        await InitializeThemeAsync();
        await ApplyBrandAsync(CurrentBrand);
        StateHasChanged();
    }

    private async Task InitializeThemeAsync()
    {
        try
        {
            var theme = await JSRuntime.InvokeAsync<string>("xfTheme.getTheme");
            _isDark = string.Equals(theme, "dark", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            _isDark = false;
        }
    }

    private async Task ToggleThemeAsync()
    {
        try
        {
            var theme = await JSRuntime.InvokeAsync<string>("xfTheme.toggleTheme");
            _isDark = string.Equals(theme, "dark", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            _isDark = !_isDark;
        }
    }

    private void OnBrandChanged(object? sender, string brand)
    {
        _ = InvokeAsync(async () =>
        {
            await ApplyBrandAsync(brand);
            StateHasChanged();
        });
    }

    private void OnLanguageChanged(object? sender, EventArgs args)
    {
        _ = InvokeAsync(() =>
        {
            _languageRenderKey++;
            StateHasChanged();
        });
    }

    private async Task ApplyBrandAsync(string brand)
    {
        try
        {
            var brandValue = string.IsNullOrWhiteSpace(brand) || brand == "home"
                ? string.Empty
                : $"brand-{brand}";
            await JSRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-brand", brandValue);
        }
        catch
        {
        }
    }

    public void Dispose()
    {
        NavigationState.BrandChanged -= OnBrandChanged;
        LanguageService.LanguageChanged -= OnLanguageChanged;
        GC.SuppressFinalize(this);
    }
}
