using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.AI.Avatars;

/// <summary>
/// Visual AI agent avatar with customizable appearance, presence status,
/// and accessibility support. SVG-based for WASM compatibility (D-V2-002).
/// </summary>
public partial class XFAiAvatar : XForgeComponentBase, IDisposable
{
    // -- Parameters ------------------------------------------------------

    /// <summary>Agent display name.</summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Avatar size variant.</summary>
    [Parameter] public XFExtendedSize Size { get; set; } = XFExtendedSize.Md;

    /// <summary>Presence/online status.</summary>
    [Parameter] public XFAiAvatarStatus Status { get; set; } = XFAiAvatarStatus.Hidden;

    /// <summary>Shape variant.</summary>
    [Parameter] public XFShape Shape { get; set; } = XFShape.Circle;

    /// <summary>Avatar background color (Tailwind class, e.g., "bg-violet-500").</summary>
    [Parameter] public string? AvatarColor { get; set; }

    /// <summary>Avatar text color (Tailwind class, e.g., "text-white").</summary>
    [Parameter] public string? TextColor { get; set; }

    /// <summary>Icon name from icon catalog (shown instead of initials).</summary>
    [Parameter] public string? IconName { get; set; }

    /// <summary>Whether to show the agent name next to the avatar.</summary>
    [Parameter] public bool ShowName { get; set; }

    /// <summary>Whether the avatar is interactive (clickable).</summary>
    [Parameter] public bool Interactive { get; set; }

    /// <summary>ARIA label for the avatar.</summary>
    [Parameter] public string AriaLabel { get; set; } = "Agente IA";

    /// <summary>Status label for screen readers.</summary>
    [Parameter] public string? StatusLabel { get; set; }

    /// <summary>Custom content rendered inside the avatar circle.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Fired when the avatar is clicked (if Interactive).</summary>
    [Parameter] public EventCallback<XFAiAvatarClickEventArgs> OnClick { get; set; }

    // -- Computed --------------------------------------------------------

    private string RootClass => Join(
        "xf-ai-avatar inline-flex items-center gap-2 select-none",
        Interactive && !Disabled ? "cursor-pointer" : "",
        Disabled ? "opacity-50 pointer-events-none" : "",
        Class);

    private string AvatarContainerClass => Size switch
    {
        XFExtendedSize.Sm => Join("relative inline-flex shrink-0 items-center justify-center", ShapeClass, "h-8 w-8 text-xs", AvatarColorClass),
        XFExtendedSize.Md => Join("relative inline-flex shrink-0 items-center justify-center", ShapeClass, "h-10 w-10 text-sm", AvatarColorClass),
        XFExtendedSize.Lg => Join("relative inline-flex shrink-0 items-center justify-center", ShapeClass, "h-14 w-14 text-base", AvatarColorClass),
        XFExtendedSize.Xl => Join("relative inline-flex shrink-0 items-center justify-center", ShapeClass, "h-20 w-20 text-lg", AvatarColorClass),
        _ => ""
    };

    private string ShapeClass => Shape switch
    {
        XFShape.Circle => "rounded-full",
        XFShape.Rounded => "rounded-lg",
        XFShape.Square => "rounded-none",
        _ => "rounded-full"
    };

    private string AvatarColorClass => !string.IsNullOrWhiteSpace(AvatarColor)
        ? AvatarColor
        : "bg-violet-100 dark:bg-violet-900/30";

    private string AvatarBgStyle => !string.IsNullOrWhiteSpace(AvatarColor) && AvatarColor.StartsWith("#", StringComparison.Ordinal)
        ? $"background-color:{AvatarColor}"
        : "";

    private string TextColorClass => !string.IsNullOrWhiteSpace(TextColor)
        ? TextColor
        : "text-violet-700 dark:text-violet-300";

    private string IconClass => Join("font-semibold", TextColorClass);

    private string InitialsClass => Join("font-semibold leading-none", TextColorClass);

    private string NameClass => Size switch
    {
        XFExtendedSize.Sm => "text-xs font-medium text-text-primary truncate max-w-[8rem]",
        XFExtendedSize.Md => "text-sm font-medium text-text-primary truncate max-w-[10rem]",
        XFExtendedSize.Lg => "text-base font-medium text-text-primary truncate max-w-[12rem]",
        XFExtendedSize.Xl => "text-lg font-medium text-text-primary truncate max-w-[16rem]",
        _ => ""
    };

    private string StatusClass => Size switch
    {
        XFExtendedSize.Sm => Join("absolute -bottom-0.5 -right-0.5 h-2.5 w-2.5 rounded-full ring-2 ring-bg-surface", StatusColorClass),
        XFExtendedSize.Md => Join("absolute -bottom-0.5 -right-0.5 h-3 w-3 rounded-full ring-2 ring-bg-surface", StatusColorClass),
        XFExtendedSize.Lg => Join("absolute bottom-0 right-0 h-3.5 w-3.5 rounded-full ring-2 ring-bg-surface", StatusColorClass),
        XFExtendedSize.Xl => Join("absolute bottom-0.5 right-0.5 h-4 w-4 rounded-full ring-2 ring-bg-surface", StatusColorClass),
        _ => ""
    };

    private string StatusColorClass => Status switch
    {
        XFAiAvatarStatus.Online => "bg-green-500",
        XFAiAvatarStatus.Busy => "bg-red-500",
        XFAiAvatarStatus.Away => "bg-yellow-500",
        XFAiAvatarStatus.Offline => "bg-gray-400",
        _ => ""
    };

    // -- Interactions ----------------------------------------------------

    private async Task HandleClickAsync()
    {
        if (!Interactive || Disabled || !OnClick.HasDelegate)
            return;

        await OnClick.InvokeAsync(new XFAiAvatarClickEventArgs
        {
            AgentName = Name ?? "IA"
        });
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (args.Key is "Enter" or " " && Interactive && !Disabled)
        {
            await HandleClickAsync();
        }
    }

    // -- Helpers ---------------------------------------------------------

    private string GetInitials()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return "IA";

        var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant()
            : parts[0][..Math.Min(2, parts[0].Length)].ToUpperInvariant();
    }

    private static string Join(params string?[] values)
    {
        return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
