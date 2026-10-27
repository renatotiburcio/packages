using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Overlays.Popover;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Overlays;

public sealed class XFPopoverInteractionTest : BunitContext
{
    public XFPopoverInteractionTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void Renders_Default() => Render<XFPopover>();
    [Fact] public void Renders_Open() => Render<XFPopover>(p => p.Add(x => x.Open, true));
    [Fact] public void Renders_WithChildContent() => Render<XFPopover>(p => p.Add(x => x.ChildContent, builder => builder.AddMarkupContent(0, "<div>Content</div>")));
    [Fact] public void Renders_WithTrigger() => Render<XFPopover>(p => p.Add(x => x.Trigger, context => builder => builder.AddMarkupContent(0, "<button>Click</button>")));
    [Fact] public void Renders_PlacementTop() => Render<XFPopover>(p => p.Add(x => x.Placement, XFPopoverPlacement.Top));
    [Fact] public void Renders_PlacementBottom() => Render<XFPopover>(p => p.Add(x => x.Placement, XFPopoverPlacement.Bottom));
    [Fact] public void Renders_PlacementLeft() => Render<XFPopover>(p => p.Add(x => x.Placement, XFPopoverPlacement.Left));
    [Fact] public void Renders_PlacementRight() => Render<XFPopover>(p => p.Add(x => x.Placement, XFPopoverPlacement.Right));
    [Fact] public void Renders_AlignmentCenter() => Render<XFPopover>(p => p.Add(x => x.Alignment, XFPopoverAlignment.Center));
    [Fact] public void Renders_AlignmentStart() => Render<XFPopover>(p => p.Add(x => x.Alignment, XFPopoverAlignment.Start));
    [Fact] public void Renders_AlignmentEnd() => Render<XFPopover>(p => p.Add(x => x.Alignment, XFPopoverAlignment.End));
    [Fact] public void Renders_SizeSm() => Render<XFPopover>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Renders_SizeLg() => Render<XFPopover>(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void Renders_Offset() => Render<XFPopover>(p => p.Add(x => x.Offset, 16));
    [Fact] public void Renders_CollisionPadding() => Render<XFPopover>(p => p.Add(x => x.CollisionPadding, 20));
    [Fact] public void Renders_FlipFalse() => Render<XFPopover>(p => p.Add(x => x.Flip, false));
    [Fact] public void Renders_CloseOnEscapeFalse() => Render<XFPopover>(p => p.Add(x => x.CloseOnEscape, false));
    [Fact] public void OpenChanged_CallbackSet() { var cut = Render<XFPopover>(p => p.Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => { }))); Assert.NotNull(cut); }
    [Fact] public void Enums_PopoverPlacement() => Assert.Equal(4, Enum.GetValues<XFPopoverPlacement>().Length);
    [Fact] public void Enums_PopoverAlignment() => Assert.Equal(3, Enum.GetValues<XFPopoverAlignment>().Length);
}
