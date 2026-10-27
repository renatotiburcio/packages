using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Tooling;

/// <summary>
/// Component scaffolding generator that creates Razor, code-behind, tests, and
/// docs from a configuration form. Pure Blazor, no JS interop.
/// </summary>
public partial class XFComponentGenerator : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Component Generator";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Gerador de componentes";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Initial generator configuration.</summary>
    [Parameter]
    public XFGeneratorConfig? InitialConfig { get; set; }

    /// <summary>Fired when generation completes.</summary>
    [Parameter]
    public EventCallback<IReadOnlyList<XFGeneratorOutput>> OnGenerated { get; set; }

    /// <summary>Fired when output is copied.</summary>
    [Parameter]
    public EventCallback<XFGeneratorOutput> OnCopied { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private XFGeneratorConfig _config = new();
    private readonly List<XFGeneratorOutput> _outputs = new();

    // ── Lifecycle ───────────────────────────────────────────────────────

    protected override void OnInitialized()
    {
        if (InitialConfig is not null)
            _config = InitialConfig;
    }

    // ── Public API ──────────────────────────────────────────────────────

    /// <summary>Gets the current configuration.</summary>
    public XFGeneratorConfig GetConfig() => _config;

    /// <summary>Gets generated outputs.</summary>
    public IReadOnlyList<XFGeneratorOutput> GetOutputs() => _outputs.AsReadOnly();

    // ── Razor Helpers ───────────────────────────────────────────────────

    private void UpdateComponentName(string? value) => _config = _config with { ComponentName = value ?? "" };
    private void UpdateNamespace(string? value) => _config = _config with { Namespace = value ?? "" };
    private void UpdateBaseClass(string? value) => _config = _config with { BaseClass = value };

    private void UpdateOutputType(string? value)
    {
        if (Enum.TryParse<XFGeneratorOutputType>(value, out var type))
            _config = _config with { OutputType = type };
    }

    private void ToggleBool(string property, object? value)
    {
        var boolVal = value is bool b ? b : false;
        _config = property switch
        {
            "IncludeTests" => _config with { IncludeTests = boolVal },
            "IncludeDocs" => _config with { IncludeDocs = boolVal },
            "TailwindFirst" => _config with { TailwindFirst = boolVal },
            "SupportDarkLight" => _config with { SupportDarkLight = boolVal },
            "ImplementDisposable" => _config with { ImplementDisposable = boolVal },
            "ImplementAsyncDisposable" => _config with { ImplementAsyncDisposable = boolVal },
            _ => _config
        };
    }

    // ── Generation ──────────────────────────────────────────────────────

    private void Generate()
    {
        _outputs.Clear();
        var name = _config.ComponentName;

        _outputs.Add(new XFGeneratorOutput
        {
            FileName = $"{name}.razor",
            Content = GenerateRazor(name),
            OutputType = _config.OutputType
        });

        _outputs.Add(new XFGeneratorOutput
        {
            FileName = $"{name}.razor.cs",
            Content = GenerateCodeBehind(name),
            OutputType = _config.OutputType
        });

        if (_config.IncludeTests)
        {
            _outputs.Add(new XFGeneratorOutput
            {
                FileName = $"{name}Test.cs",
                Content = GenerateTest(name),
                OutputType = XFGeneratorOutputType.Service
            });
        }

        if (_config.IncludeDocs)
        {
            _outputs.Add(new XFGeneratorOutput
            {
                FileName = $"{name}Docs.razor",
                Content = GenerateDocs(name),
                OutputType = XFGeneratorOutputType.Page
            });
        }

        OnGenerated.InvokeAsync(_outputs.AsReadOnly());
    }

    private void ParseParameters(string raw)
    {
        var lines = raw.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        _config = _config with { Parameters = lines };
    }

    private async Task CopyOutput(XFGeneratorOutput output)
    {
        await OnCopied.InvokeAsync(output);
    }

    // ── Code Generators ─────────────────────────────────────────────────

    private string GenerateRazor(string name)
    {
        var implements = _config.ImplementAsyncDisposable ? "\n@implements IAsyncDisposable" :
                         _config.ImplementDisposable ? "\n@implements IDisposable" : "";
        var baseClass = !string.IsNullOrEmpty(_config.BaseClass) ? $"@inherits {_config.BaseClass}" : "";

        return $@"{baseClass}{implements}

<div class={{@""RootClass""}}
     role=""region""
     aria-label={{@""AriaLabel""}}
     @attributes=""AdditionalAttributes"">

    @if (ShowHeader)
    {{
        <div class=""flex items-center justify-between border-b border-border-light px-4 py-2.5"">
            <span class=""text-sm font-semibold text-text-primary"">@Title</span>
        </div>
    }}

    <div class=""p-4"">
        <p class=""text-sm text-text-secondary"">{name} content</p>
    </div>
</div>";
    }

    private string GenerateCodeBehind(string name)
    {
        var ns = _config.Namespace;
        var baseClass = !string.IsNullOrEmpty(_config.BaseClass) ? $" : {_config.BaseClass}" : " : XForgeComponentBase";
        var interfaces = _config.ImplementAsyncDisposable ? ", IAsyncDisposable" :
                         _config.ImplementDisposable ? ", IDisposable" : "";

        var parameters = new List<string>();
        foreach (var p in _config.Parameters)
        {
            var parts = p.Split(' ', 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2)
            {
                parameters.Add($$"""
        /// <summary>TODO: document.</summary>
        [Parameter]
        public {{parts[0]}} {{parts[1]}} { get; set; }
""");
            }
        }

        var paramBlock = parameters.Any() ? "\n    // ── Parameters ──────────────────────────────────────────────────────\n" + string.Join("\n", parameters) : "";

        var disposeImpl = _config.ImplementAsyncDisposable ? """

    // ── IAsyncDisposable ──────────────────────────────────────────────

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    private ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;
""" : _config.ImplementDisposable ? """

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
""" : "";

        return $$"""
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace {{ns}};

/// <summary>
/// TODO: document {{name}}.
/// </summary>
public partial class {{name}}{{baseClass}}{{interfaces}}
{{{paramBlock}}

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "{{name}}";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "{{name}}";

    /// <summary>Whether to show the header.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;
{{disposeImpl}}
    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-{{name.ToLower()}} rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();
}
""";
    }

    private string GenerateTest(string name)
    {
        return $$"""
using Bunit;
using Xunit;
using {{_config.Namespace}};

namespace XForge.DesignSystem.Tests.Components;

public class {{name}}Test : BunitContext
{
    [Fact]
    public void {{name}}_RendersDefault()
    {
        var cut = Render<{{name}}>();

        Assert.Contains("xf-{{name.ToLower()}}", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void {{name}}_ShowsTitle()
    {
        var cut = Render<{{name}}>(p => p.Add(x => x.Title, "Test Title"));

        Assert.Contains("Test Title", cut.Markup);
    }

    [Fact]
    public void {{name}}_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<{{name}}>(p => p.Add(x => x.ShowHeader, false));

        Assert.DoesNotContain("Test Title", cut.Markup);
    }
}
""";
    }

    private string GenerateDocs(string name)
    {
        var route = name.Replace("XF", "").ToLower();
        return $$"""
@page "/ai/{{route}}"
@using {{_config.Namespace}}

<PageTitle>{{name}} — XForge Docs</PageTitle>

<main id=""main-content"" class=""mx-auto grid max-w-6xl gap-6 px-6 py-10"">
    <header class=""grid gap-2"">
        <XFTypography Variant=""XFTypographyVariant.H2"" Weight=""XFTypographyWeight.Bold"">{{name}}</XFTypography>
        <XFTypography Variant=""XFTypographyVariant.Body1"" Color=""XFTypographyColor.Secondary"">
            TODO: document {{name}}.
        </XFTypography>
    </header>

    <section class=""grid gap-5"">
        <XFCard Variant=""XFCardVariant.Outlined"" Padding=""XFPadding.Xl"" AreaRestricted=""false"">
            <ChildContent>
                <div class=""grid gap-6"">
                    <div class=""flex flex-wrap items-center gap-2"">
                        <XFBadge Variant=""XFBadgeVariant.Solid"" Color=""XFBadgeColor.Primary"" Text=""{{name}}"" />
                    </div>
                    <div class=""grid gap-2"">
                        <XFTypography Variant=""XFTypographyVariant.H3"" Weight=""XFTypographyWeight.Semibold"">Descricao do componente.</XFTypography>
                    </div>
                </div>
            </ChildContent>
        </XFCard>
    </section>

    <section class=""grid gap-5"">
        <XFCard Variant=""XFCardVariant.Outlined"" Padding=""XFPadding.Lg"">
            <Header><XFTypography Variant=""XFTypographyVariant.H4"" Weight=""XFTypographyWeight.Semibold"">Exemplo</XFTypography></Header>
            <ChildContent>
                <div class=""grid gap-4"">
                    <{{name}} Title=""Exemplo"" />
                </div>
            </ChildContent>
        </XFCard>
    </section>

    <section class=""grid gap-5"">
        <XFCard Variant=""XFCardVariant.Outlined"" Padding=""XFPadding.Lg"">
            <Header><XFTypography Variant=""XFTypographyVariant.H4"" Weight=""XFTypographyWeight.Semibold"">API — Parametros</XFTypography></Header>
            <ChildContent>
                <div class=""overflow-x-auto"">
                    <table class=""w-full text-sm"">
                        <thead><tr class=""border-b border-border-light""><th class=""py-2 text-left font-semibold"">Parametro</th><th class=""py-2 text-left font-semibold"">Tipo</th><th class=""py-2 text-left font-semibold"">Default</th><th class=""py-2 text-left font-semibold"">Descricao</th></tr></thead>
                        <tbody>
                            <tr class=""border-b border-border-light/50""><td class=""py-2 font-mono text-xs"">Title</td><td class=""py-2"">string</td><td class=""py-2"">""{{name}}""</td><td class=""py-2"">Titulo do componente</td></tr>
                            <tr class=""border-b border-border-light/50""><td class=""py-2 font-mono text-xs"">ShowHeader</td><td class=""py-2"">bool</td><td class=""py-2"">true</td><td class=""py-2"">Mostrar cabecalho</td></tr>
                        </tbody>
                    </table>
                </div>
            </ChildContent>
        </XFCard>
    </section>
</main>
""";
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string GetOutputTypeBadgeClass(XFGeneratorOutputType type) => type switch
    {
        XFGeneratorOutputType.Component => "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-300",
        XFGeneratorOutputType.Service => "bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-300",
        XFGeneratorOutputType.Page => "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300",
        XFGeneratorOutputType.Layout => "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-300",
        _ => "bg-slate-100 text-slate-700"
    };

    private static string GetOutputTypeLabel(XFGeneratorOutputType type) => type switch
    {
        XFGeneratorOutputType.Component => "Component",
        XFGeneratorOutputType.Service => "Service",
        XFGeneratorOutputType.Page => "Page",
        XFGeneratorOutputType.Layout => "Layout",
        _ => "File"
    };

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-component-generator overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
