using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Overlays.Dialog;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Overlays;

public sealed class XFDialogInteractionTest : BunitContext
{
    public XFDialogInteractionTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void Renders_Default() => Render<XFDialog>();
    [Fact] public void Renders_WithTitle() { var cut = Render<XFDialog>(p => { p.Add(x => x.Open, true); p.Add(x => x.Title, "Confirm"); }); Assert.Contains("Confirm", cut.Markup); }
    [Fact] public void Renders_WithDescription() { var cut = Render<XFDialog>(p => { p.Add(x => x.Open, true); p.Add(x => x.Description, "Are you sure?"); }); Assert.NotNull(cut); }
    [Fact] public void Renders_WithChildContent() => Render<XFDialog>(p => p.Add(x => x.ChildContent, builder => builder.AddMarkupContent(0, "<div>Content</div>")));
    [Fact] public void Renders_WithHeader() => Render<XFDialog>(p => p.Add(x => x.Header, builder => builder.AddMarkupContent(0, "<div>Header</div>")));
    [Fact] public void Renders_WithFooter() => Render<XFDialog>(p => p.Add(x => x.Footer, builder => builder.AddMarkupContent(0, "<div>Footer</div>")));
    [Fact] public void Renders_OpenTrue() => Render<XFDialog>(p => p.Add(x => x.Open, true));
    [Fact] public void Renders_SizeSm() => Render<XFDialog>(p => p.Add(x => x.Size, XFOverlaySize.Sm));
    [Fact] public void Renders_SizeLg() => Render<XFDialog>(p => p.Add(x => x.Size, XFOverlaySize.Lg));
    [Fact] public void Renders_SizeXl() => Render<XFDialog>(p => p.Add(x => x.Size, XFOverlaySize.Xl));
    [Fact] public void Renders_TonePrimary() => Render<XFDialog>(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
    [Fact] public void Renders_ToneDanger() => Render<XFDialog>(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
    [Fact] public void Renders_PlacementCenter() => Render<XFDialog>(p => p.Add(x => x.Placement, XFDialogPlacement.Center));
    [Fact] public void Renders_PlacementTop() => Render<XFDialog>(p => p.Add(x => x.Placement, XFDialogPlacement.Top));
    [Fact] public void Renders_Alert() => Render<XFDialog>(p => p.Add(x => x.Alert, true));
    [Fact] public void Renders_CloseOnEscapeFalse() => Render<XFDialog>(p => p.Add(x => x.CloseOnEscape, false));
    [Fact] public void Renders_CloseOnOverlayClickFalse() => Render<XFDialog>(p => p.Add(x => x.CloseOnOverlayClick, false));
    [Fact] public void Renders_ShowCloseButtonFalse() => Render<XFDialog>(p => p.Add(x => x.ShowCloseButton, false));
    [Fact] public void OpenChanged_CallbackSet() { bool? open = null; var cut = Render<XFDialog>(p => p.Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => open = v))); Assert.NotNull(cut); }
    [Fact] public void OnClose_CallbackSet() { var cut = Render<XFDialog>(p => p.Add(x => x.OnClose, EventCallback.Factory.Create(this, () => { }))); Assert.NotNull(cut); }
    [Fact] public void Enums_OverlaySize() => Assert.Equal(5, Enum.GetValues<XFOverlaySize>().Length);
    [Fact] public void Enums_DialogPlacement() => Assert.Equal(5, Enum.GetValues<XFDialogPlacement>().Length);
}
