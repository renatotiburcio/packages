// T044: Default Tailwind class provider.
namespace XForge.DesignSystem.Providers;

/// <summary>Default Tailwind v4 CSS class provider.</summary>
public sealed class XFTailwindClassProvider : IXFClassProvider
{
    public string ProviderName => "Tailwind";
    public string Button(bool outline, string? size = null, string? color = null)
    {
        var sz = size switch { "sm" => "px-2 py-1 text-sm", "lg" => "px-4 py-2 text-lg", _ => "px-3 py-1.5 text-base" };
        var cl = color ?? "blue";
        return outline
            ? $"border border-{cl}-600 text-{cl}-600 hover:bg-{cl}-50 rounded {sz}"
            : $"bg-{cl}-600 text-white hover:bg-{cl}-700 rounded {sz}";
    }
    public string Input(string? size = null, string? color = null, bool disabled = false)
    {
        var sz = size switch { "sm" => "px-2 py-1 text-sm", "lg" => "px-3 py-2 text-lg", _ => "px-2.5 py-1.5 text-base" };
        var cl = color ?? "blue";
        var d = disabled ? "opacity-50 cursor-not-allowed " : "";
        return $"{d}border border-gray-300 rounded focus:ring-2 focus:ring-{cl}-500 {sz}";
    }
    public string Card() => "bg-white rounded-lg shadow p-4";
    public string Modal() => "fixed inset-0 z-50 flex items-center justify-center bg-black/50";
    public string Table() => "min-w-full divide-y divide-gray-200";
    public string Alert(string? color = null) => $"border-l-4 p-3 bg-{color ?? "blue"}-50 text-{color ?? "blue"}-900";
    public string Badge(string? color = null) => $"inline-block px-2 py-0.5 text-xs rounded-full bg-{color ?? "gray"}-100 text-{color ?? "gray"}-800";
}