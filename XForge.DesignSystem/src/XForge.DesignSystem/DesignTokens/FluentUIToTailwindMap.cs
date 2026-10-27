using System.Collections.Generic;

namespace XForge.DesignSystem.DesignTokens;

/// <summary>
/// Maps XFIconColor and XFIconSize to Tailwind v4 utility classes.
/// Allows design-system components to co-exist with Tailwind apps.
/// </summary>
public static class FluentUIToTailwindMap
{
    public static readonly IReadOnlyDictionary<string, string> ColorMap = new Dictionary<string, string>
    {
        ["Accent"] = "text-blue-600 dark:text-blue-400",
        ["Lightweight"] = "text-slate-500 dark:text-slate-400",
        ["Success"] = "text-green-600 dark:text-green-400",
        ["Warning"] = "text-yellow-600 dark:text-yellow-400",
        ["Error"] = "text-red-600 dark:text-red-400",
        ["Important"] = "text-purple-600 dark:text-purple-400",
    };

    public static readonly IReadOnlyDictionary<int, string> SizeMap = new Dictionary<int, string>
    {
        [16] = "w-4 h-4",
        [20] = "w-5 h-5",
        [24] = "w-6 h-6",
        [32] = "w-8 h-8",
        [48] = "w-12 h-12",
    };

    public static string ToTailwindColor(string colorName) =>
        ColorMap.TryGetValue(colorName, out var c) ? c : string.Empty;

    public static string ToTailwindSize(int pixels) =>
        SizeMap.TryGetValue(pixels, out var s) ? s : $"w-[{pixels}px] h-[{pixels}px]";
}