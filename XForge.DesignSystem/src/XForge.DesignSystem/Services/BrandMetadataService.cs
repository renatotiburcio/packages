using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using XForge.DesignSystem.Components.Icon;

namespace XForge.DesignSystem.Services;

public class BrandMetadata
{
    public string Title { get; init; } = string.Empty;
    public string Color { get; init; } = "#6366f1";
    public string? ContrastColor { get; init; }
    public string IconSvg { get; init; } = string.Empty;

    public BrandMetadata() { }

    public BrandMetadata(string title, string color, string iconSvg, string? contrastColor = null)
    {
        Title = title;
        Color = color;
        IconSvg = iconSvg;
        ContrastColor = contrastColor;
    }
}

public class BrandMetadataOptions
{
    public Dictionary<string, BrandMetadata> Brands { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public BrandMetadata DefaultBrand { get; set; } = new("XForge Docs", "#6366f1", "");
    public bool ReplaceDefaults { get; set; }
}

public class BrandMetadataService
{
    private static readonly Dictionary<string, BrandMetadata> DefaultBrands = new(StringComparer.OrdinalIgnoreCase)
    {
        ["components"] = new BrandMetadata("Components", "#6366f1",
            XFSvgIcons.Get("layer-group")),
        ["display"] = new BrandMetadata("Data Display", "#0ea5e9",
            XFSvgIcons.Get("table-cells")),
        ["feedback"] = new BrandMetadata("Feedback", "#f59e0b",
            XFSvgIcons.Get("message-exclamation")),
        ["layout"] = new BrandMetadata("Layout and Shell", "#10b981",
            XFSvgIcons.Get("window-frame")),
        ["forms"] = new BrandMetadata("Forms", "#ec4899",
            XFSvgIcons.Get("input-text")),
        ["tokens"] = new BrandMetadata("Tokens e Temas", "#8b5cf6",
            XFSvgIcons.Get("palette")),
        ["home"] = new BrandMetadata("XForge Docs", "#6366f1",
            XFSvgIcons.Get("house")),
    };

    private readonly Dictionary<string, BrandMetadata> brands;
    private readonly BrandMetadata defaultMetadata;

    public BrandMetadataService()
        : this(Options.Create(new BrandMetadataOptions()))
    {
    }

    public BrandMetadataService(IOptions<BrandMetadataOptions> options)
    {
        var configured = options.Value;
        brands = configured.ReplaceDefaults
            ? new Dictionary<string, BrandMetadata>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, BrandMetadata>(DefaultBrands, StringComparer.OrdinalIgnoreCase);

        foreach (var (key, metadata) in configured.Brands)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                brands[key] = metadata;
            }
        }

        defaultMetadata = configured.DefaultBrand;
    }

    public BrandMetadata? GetBrand(string brand)
    {
        if (string.IsNullOrEmpty(brand))
            return null;

        return brands.GetValueOrDefault(brand) ?? null;
    }

    public string GetBrandTitle(string brand)
    {
        return GetBrand(brand)?.Title ?? (string.IsNullOrEmpty(brand) || brand.Equals("home", StringComparison.OrdinalIgnoreCase) ? defaultMetadata.Title : char.ToUpper(brand[0]) + brand[1..]);
    }

    public string GetBrandColor(string brand)
    {
        return GetBrand(brand)?.Color ?? defaultMetadata.Color;
    }

    public string GetBrandContrastColor(string brand)
    {
        var metadata = GetBrand(brand) ?? defaultMetadata;
        return !string.IsNullOrWhiteSpace(metadata.ContrastColor)
            ? metadata.ContrastColor!
            : GetReadableContrastColor(metadata.Color);
    }

    public string GetBrandIconSvg(string brand)
    {
        return GetBrand(brand)?.IconSvg ?? defaultMetadata.IconSvg;
    }

    public bool IsBrandKnown(string brand)
    {
        return !string.IsNullOrEmpty(brand) && brands.ContainsKey(brand);
    }

    public IReadOnlyList<string> GetAllBrands()
    {
        return brands.Keys.ToList().AsReadOnly();
    }

    public IReadOnlyDictionary<string, BrandMetadata> GetAllBrandMetadata()
    {
        return new ReadOnlyDictionary<string, BrandMetadata>(brands);
    }

    [Obsolete("Brand CSS should be declared in app.css as static :root[data-brand] blocks. This method will be removed in a future version.")]
    public string BuildBrandCssVariables(string brand, string selector)
    {
        if (string.IsNullOrWhiteSpace(selector))
        {
            throw new ArgumentException("Selector must be provided.", nameof(selector));
        }

        var color = GetBrandColor(brand);
        var contrast = GetBrandContrastColor(brand);
        var scale = BuildColorScale(color);
        var builder = new StringBuilder();

        builder.Append(selector);
        builder.Append(" {");
        foreach (var (step, value) in scale)
        {
            builder.Append(CultureInfo.InvariantCulture, $" --color-primary-{step}: {value};");
        }

        builder.Append(" --color-primary: var(--color-primary-500);");
        builder.Append(" --color-primary-hover: var(--color-primary-600);");
        builder.Append(" --color-primary-active: var(--color-primary-700);");
        builder.Append(CultureInfo.InvariantCulture, $" --color-primary-contrast: {contrast};");
        builder.Append(" }");
        return builder.ToString();
    }

            /// <summary>
        /// Generates complete CSS rules for all brands with both light and dark theme selectors.
        /// Light theme: :root[data-brand="brand-X"] — sets primary color scale (50-950) and derivatives.
        /// Dark theme:  :root[data-theme="dark"][data-brand="brand-X"] — shifts primary toward lighter
        /// steps for visibility on dark backgrounds, and inverts hover/active direction.
        /// </summary>
    [Obsolete("Brand CSS should be declared in app.css as static :root[data-brand] blocks. This method will be removed in a future version.")]
        public string BuildAllBrandThemeCssRules()
        {
            var builder = new StringBuilder();

            foreach (var brandKey in GetAllBrands())
            {
                var brandAttr = $"brand-{EscapeCssAttributeValue(brandKey)}";
                var color = GetBrandColor(brandKey);
                var contrast = GetBrandContrastColor(brandKey);

                if (!TryParseHex(color, out var r, out var g, out var b))
                {
                    r = 99; g = 102; b = 241;
                }

                var lightScale = BuildColorScale(color);

                // --- Light theme rule ---
                builder.Append(CultureInfo.InvariantCulture, $":root[data-brand=\"{brandAttr}\"] {{");
                foreach (var (step, value) in lightScale)
                {
                    builder.Append(CultureInfo.InvariantCulture, $" --color-primary-{step}: {value};");
                }
                builder.Append(" --color-primary: var(--color-primary-500);");
                builder.Append(" --color-primary-hover: var(--color-primary-600);");
                builder.Append(" --color-primary-active: var(--color-primary-700);");
                builder.Append(CultureInfo.InvariantCulture, $" --color-primary-contrast: {contrast};");
                builder.Append(" }");
                builder.AppendLine();

                // --- Dark theme rule ---
                // In dark mode, shift the primary to a lighter step for visibility.
                // Primary = 400 (lighter), Hover = 300 (lighter still), Active = 500 (base).
                // The full scale remains available for components that need specific steps.
                builder.Append(CultureInfo.InvariantCulture, $":root[data-theme=\"dark\"][data-brand=\"{brandAttr}\"] {{");
                foreach (var (step, value) in lightScale)
                {
                    builder.Append(CultureInfo.InvariantCulture, $" --color-primary-{step}: {value};");
                }
                builder.Append(" --color-primary: var(--color-primary-400);");
                builder.Append(" --color-primary-hover: var(--color-primary-300);");
                builder.Append(" --color-primary-active: var(--color-primary-500);");
                // In dark mode, contrast color depends on the brand color luminance.
                // Light brands (like yellow/amber) need dark text; dark brands need white text.
                var darkContrast = GetRelativeLuminance(r, g, b) > 0.179 ? "rgb(15 23 42)" : "rgb(255 255 255)";
                builder.Append(CultureInfo.InvariantCulture, $" --color-primary-contrast: {darkContrast};");
                builder.Append(" }");
                builder.AppendLine();
            }

            return builder.ToString();
        }

    [Obsolete("Brand CSS should be declared in app.css as static :root[data-brand] blocks. This method will be removed in a future version.")]
public string BuildAllBrandCssVariables(Func<string, string>? selectorFactory = null)
    {
        selectorFactory ??= brand => $":root[data-brand=\"{EscapeCssAttributeValue(brand)}\"]";
        var builder = new StringBuilder();

        foreach (var brand in GetAllBrands())
        {
            var selector = selectorFactory(brand);
            if (string.IsNullOrWhiteSpace(selector))
            {
                continue;
            }

            if (builder.Length > 0)
            {
                builder.AppendLine();
            }

            builder.Append(BuildBrandCssVariables(brand, selector));
        }

        return builder.ToString();
    }

    private static string GetReadableContrastColor(string color)
    {
        return TryParseHex(color, out var r, out var g, out var b) && GetRelativeLuminance(r, g, b) > 0.179
            ? "#0f172a"
            : "#ffffff";
    }

    private static IReadOnlyList<(int Step, string Color)> BuildColorScale(string color)
    {
        if (!TryParseHex(color, out var r, out var g, out var b))
        {
            r = 99;
            g = 102;
            b = 241;
        }

        return new[]
        {
            (50, MixWithWhite(r, g, b, 0.94)),
            (100, MixWithWhite(r, g, b, 0.86)),
            (200, MixWithWhite(r, g, b, 0.72)),
            (300, MixWithWhite(r, g, b, 0.52)),
            (400, MixWithWhite(r, g, b, 0.28)),
            (500, ToRgb(r, g, b)),
            (600, MixWithBlack(r, g, b, 0.12)),
            (700, MixWithBlack(r, g, b, 0.24)),
            (800, MixWithBlack(r, g, b, 0.38)),
            (900, MixWithBlack(r, g, b, 0.52)),
            (950, MixWithBlack(r, g, b, 0.68))
        };
    }

    private static string MixWithWhite(int r, int g, int b, double amount) =>
        ToRgb(Mix(r, 255, amount), Mix(g, 255, amount), Mix(b, 255, amount));

    private static string MixWithBlack(int r, int g, int b, double amount) =>
        ToRgb(Mix(r, 0, amount), Mix(g, 0, amount), Mix(b, 0, amount));

    private static int Mix(int source, int target, double amount) =>
        (int)Math.Round(source * (1 - amount) + target * amount);

    private static string ToRgb(int r, int g, int b) =>
        string.Create(CultureInfo.InvariantCulture, $"rgb({r} {g} {b})");

    private static string EscapeCssAttributeValue(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal);

    private static bool TryParseHex(string color, out int r, out int g, out int b)
    {
        r = 0;
        g = 0;
        b = 0;

        if (string.IsNullOrWhiteSpace(color))
        {
            return false;
        }

        var hex = color.Trim().TrimStart('#');
        if (hex.Length == 3)
        {
            hex = string.Concat(hex.Select(character => $"{character}{character}"));
        }

        if (hex.Length != 6)
        {
            return false;
        }

        return int.TryParse(hex[..2], System.Globalization.NumberStyles.HexNumber, null, out r)
            && int.TryParse(hex[2..4], System.Globalization.NumberStyles.HexNumber, null, out g)
            && int.TryParse(hex[4..6], System.Globalization.NumberStyles.HexNumber, null, out b);
    }

    private static double GetRelativeLuminance(int r, int g, int b)
    {
        static double Convert(int channel)
        {
            var value = channel / 255d;
            return value <= 0.03928 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
        }

        return 0.2126 * Convert(r) + 0.7152 * Convert(g) + 0.0722 * Convert(b);
    }
}
