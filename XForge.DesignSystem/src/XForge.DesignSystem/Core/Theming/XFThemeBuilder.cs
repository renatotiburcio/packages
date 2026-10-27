namespace XForge.DesignSystem.Core.Theming
{
    /// <summary>
    /// Fluent builder for creating custom themes with CSS variable overrides.
    /// Usage: XFThemeBuilder.Create().WithPrimary("#6366f1").WithRadius("0.75rem").Build();
    /// </summary>
    public class XFThemeBuilder
    {
        private readonly Dictionary<string, string> _variables = new();
        private string _themeName = "custom";

        private XFThemeBuilder() { }

        /// <summary>Creates a new theme builder.</summary>
        public static XFThemeBuilder Create(string? name = null)
        {
            var builder = new XFThemeBuilder();
            if (name != null) builder._themeName = name;
            return builder;
        }

        #region Primary Palette

        /// <summary>Sets the primary color (auto-generates 50-950 scale).</summary>
        public XFThemeBuilder WithPrimary(string hex)
        {
            var (h, s, l) = HexToHsl(hex);
            SetScale("color-primary", h, s, l);
            _variables["--color-primary"] = hex;
            return this;
        }

        /// <summary>Sets a specific primary shade.</summary>
        public XFThemeBuilder WithPrimaryShade(int shade, string hex)
        {
            _variables[$"--color-primary-{shade}"] = hex;
            return this;
        }

        #endregion

        #region Semantic Colors

        public XFThemeBuilder WithSuccess(string hex)
        {
            _variables["--color-success"] = hex;
            return this;
        }

        public XFThemeBuilder WithWarning(string hex)
        {
            _variables["--color-warning"] = hex;
            return this;
        }

        public XFThemeBuilder WithDanger(string hex)
        {
            _variables["--color-danger"] = hex;
            return this;
        }

        public XFThemeBuilder WithInfo(string hex)
        {
            _variables["--color-info"] = hex;
            return this;
        }

        #endregion

        #region Surfaces & Text

        public XFThemeBuilder WithBackground(string hex)
        {
            _variables["--color-bg"] = hex;
            return this;
        }

        public XFThemeBuilder WithSurface(string hex)
        {
            _variables["--color-bg-surface"] = hex;
            return this;
        }

        public XFThemeBuilder WithTextPrimary(string hex)
        {
            _variables["--color-text-primary"] = hex;
            return this;
        }

        public XFThemeBuilder WithTextSecondary(string hex)
        {
            _variables["--color-text-secondary"] = hex;
            return this;
        }

        public XFThemeBuilder WithBorder(string hex)
        {
            _variables["--color-border"] = hex;
            return this;
        }

        #endregion

        #region Typography

        public XFThemeBuilder WithFontFamily(string family)
        {
            _variables["--font-family"] = family;
            return this;
        }

        public XFThemeBuilder WithFontSize(string size)
        {
            _variables["--font-size"] = size;
            return this;
        }

        #endregion

        #region Spacing & Radius

        public XFThemeBuilder WithRadius(string radius)
        {
            _variables["--radius"] = radius;
            return this;
        }

        public XFThemeBuilder WithSpacing(string spacing)
        {
            _variables["--spacing"] = spacing;
            return this;
        }

        #endregion

        #region Component Overrides

        /// <summary>Overrides a specific CSS variable for a component.</summary>
        public XFThemeBuilder WithComponentVariable(string componentName, string variable, string value)
        {
            _variables[$"--{componentName}-{variable}"] = value;
            return this;
        }

        /// <summary>Overrides a raw CSS variable.</summary>
        public XFThemeBuilder WithVariable(string name, string value)
        {
            _variables[name] = value;
            return this;
        }

        #endregion

        #region Build

        /// <summary>Builds the theme as a CSS string.</summary>
        public string BuildCss()
        {
            var lines = _variables.Select(kv => $"  {kv.Key}: {kv.Value};").OrderBy(l => l);
            return $":root[data-theme=\"{_themeName}\"] {{\n{string.Join("\n", lines)}\n}}";
        }

        /// <summary>Builds a brand-only CSS string (data-brand selector, primary scale only).</summary>
        public string BuildBrandCss()
        {
            var primaryVars = _variables
                .Where(kv => kv.Key.StartsWith("--color-primary"))
                .OrderBy(kv => kv.Key);
            var lines = primaryVars.Select(kv => $"  {kv.Key}: {kv.Value};");
            return $":root[data-brand=\"{_themeName}\"] {{\n{string.Join("\n", lines)}\n}}";
        }

        /// <summary>Builds dark+brand combined CSS string.</summary>
        public string BuildDarkBrandCss()
        {
            var primaryVars = _variables
                .Where(kv => kv.Key.StartsWith("--color-primary"))
                .OrderBy(kv => kv.Key);
            var lines = primaryVars.Select(kv => $"  {kv.Key}: {kv.Value};");
            return $":root[data-theme=\"dark\"][data-brand=\"{_themeName}\"] {{\n{string.Join("\n", lines)}\n}}";
        }

        /// <summary>Builds the theme as a dictionary of CSS variables.</summary>
        public IReadOnlyDictionary<string, string> BuildVariables()
        {
            return _variables.AsReadOnly();
        }

        /// <summary>Builds a dark variant of this theme.</summary>
        public XFThemeBuilder BuildDarkVariant()
        {
            var dark = Create($"{_themeName}-dark");
            foreach (var kv in _variables)
            {
                if (kv.Key.Contains("primary") || kv.Key.Contains("success") ||
                    kv.Key.Contains("warning") || kv.Key.Contains("danger") ||
                    kv.Key.Contains("info"))
                {
                    dark._variables[kv.Key] = kv.Value;
                }
            }
            return dark;
        }

        #endregion

        #region Helpers

        private void SetScale(string prefix, double h, double s, double l)
        {
            _variables[$"--{prefix}-50"] = HslToHex(h, s, 97);
            _variables[$"--{prefix}-100"] = HslToHex(h, s, 93);
            _variables[$"--{prefix}-200"] = HslToHex(h, s, 86);
            _variables[$"--{prefix}-300"] = HslToHex(h, s, 76);
            _variables[$"--{prefix}-400"] = HslToHex(h, s, 64);
            _variables[$"--{prefix}-500"] = HslToHex(h, s, 53);
            _variables[$"--{prefix}-600"] = HslToHex(h, s, 45);
            _variables[$"--{prefix}-700"] = HslToHex(h, s, 38);
            _variables[$"--{prefix}-800"] = HslToHex(h, s, 31);
            _variables[$"--{prefix}-900"] = HslToHex(h, s, 24);
            _variables[$"--{prefix}-950"] = HslToHex(h, s, 16);
        }

        private static (double H, double S, double L) HexToHsl(string hex)
        {
            hex = hex.TrimStart('#');
            if (hex.Length == 3) hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";

            var r = Convert.ToInt32(hex[..2], 16) / 255.0;
            var g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255.0;
            var b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255.0;

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));
            var l = (max + min) / 2;

            if (max == min) return (0, 0, l * 100);

            var d = max - min;
            var s = l > 0.5 ? d / (2 - max - min) : d / (max + min);

            double h = max switch
            {
                _ when max == r => (g - b) / d + (g < b ? 6 : 0),
                _ when max == g => (b - r) / d + 2,
                _ => (r - g) / d + 4
            };
            h *= 60;

            return (h, s * 100, l * 100);
        }

        private static string HslToHex(double h, double s, double l)
        {
            s /= 100;
            l /= 100;

            var c = (1 - Math.Abs(2 * l - 1)) * s;
            var x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            var m = l - c / 2;

            double r, g, b;
            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return $"#{((int)((r + m) * 255)):X2}{((int)((g + m) * 255)):X2}{((int)((b + m) * 255)):X2}";
        }

        #endregion
    }
}
