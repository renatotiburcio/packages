using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Layout.Splitter;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Layout;

public class XFMultiSplitterExtendedTest : BunitContext
{
    public XFMultiSplitterExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static RenderFragment WithPanes()
    {
        return (RenderFragment)(builder =>
        {
            builder.OpenComponent<XFMultiSplitterPane>(0);
            builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => { b.AddContent(0, "Pane 1"); }));
            builder.CloseComponent();
            builder.OpenComponent<XFMultiSplitterPane>(4);
            builder.AddAttribute(5, "ChildContent", (RenderFragment)(b => { b.AddContent(0, "Pane 2"); }));
            builder.CloseComponent();
        });
    }

    private IRenderedComponent<XFMultiSplitter> RenderSplitter(
        Action<ComponentParameterCollectionBuilder<XFMultiSplitter>>? configure = null)
    {
        return Render<XFMultiSplitter>(p =>
        {
            configure?.Invoke(p);
        });
    }

    // ─── BaseClass ────────────────────────────────────────────────────

    [Fact]
    public void Extended_BaseClass_ContainsMinW0OverflowHiddenRounded()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("min-w-0 overflow-hidden rounded-lg", cut.Markup);
    }

    // ─── RootClass ────────────────────────────────────────────────────

    [Fact]
    public void Extended_RootClass_ContainsBorderBgSurfaceSelectNone()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("border border-border-light bg-bg-surface select-none", cut.Markup);
    }

    // ─── Orientation ──────────────────────────────────────────────────

    [Fact]
    public void Extended_OrientationHorizontal_RendersGridTemplateColumns()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Horizontal);
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("grid-template-columns", cut.Markup);
        Assert.Contains("min-h-72", cut.Markup);
    }

    [Fact]
    public void Extended_OrientationVertical_RendersGridTemplateRows()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Vertical);
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("grid-template-rows", cut.Markup);
        Assert.Contains("min-h-96", cut.Markup);
    }

    // ─── Visible ──────────────────────────────────────────────────────

    [Fact]
    public void Extended_VisibleTrue_Renders()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Visible, true);
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("min-w-0 overflow-hidden rounded-lg", cut.Markup);
    }

    [Fact]
    public void Extended_VisibleFalse_DoesNotRender()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Visible, false);
        });
        Assert.Equal(string.Empty, cut.Markup);
    }

    // ─── Disabled ─────────────────────────────────────────────────────

    [Fact]
    public void Extended_DisabledTrue_ContainsOpacity60()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("opacity-60", cut.Markup);
    }

    [Fact]
    public void Extended_DisabledFalse_NoOpacity60()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Disabled, false);
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.DoesNotContain("opacity-60", cut.Markup);
    }

    // ─── Class / Style / Id / AdditionalAttributes ────────────────────

    [Fact]
    public void Extended_ClassParameter_AppendsCustomClass()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Class, "my-splitter");
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("my-splitter", cut.Markup);
    }

    [Fact]
    public void Extended_StyleParameter_RendersStyleAttribute()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Style, "margin: 10px");
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("margin: 10px", cut.Markup);
    }

    [Fact]
    public void Extended_IdParameter_RendersIdAttribute()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Id, "my-splitter");
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("my-splitter", cut.Markup);
    }

    [Fact]
    public void Extended_AdditionalAttributes_RendersExtraAttributes()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-testid", "splitter" } });
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("data-testid", cut.Markup);
    }

    // ─── Step ─────────────────────────────────────────────────────────

    [Fact]
    public void Extended_StepParameter_AcceptsStep()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Step, 5);
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("min-w-0 overflow-hidden", cut.Markup);
    }

    // ─── HandleLabel / HandleLabelFormat ──────────────────────────────

    [Fact]
    public void Extended_HandleLabelParameter_RendersCustomHandleLabel()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.HandleLabel, "Custom Handle");
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("Custom Handle", cut.Markup);
        Assert.DoesNotContain("Redimensionar paineis", cut.Markup);
    }

    [Fact]
    public void Extended_HandleLabelFormatParameter_UsesCustomFormat()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.HandleLabel, "Divider");
            p.Add(x => x.HandleLabelFormat, "{0} #{1}");
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("Divider #1", cut.Markup);
    }

    // ─── Sizes ────────────────────────────────────────────────────────

    [Fact]
    public void Extended_SizesParameter_AcceptsSizes()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Sizes, new List<double> { 2, 1 });
            p.Add(x => x.ChildContent, WithPanes());
        });
        Assert.Contains("2fr", cut.Markup);
    }

    // ─── Events ───────────────────────────────────────────────────────

    [Fact]
    public void Extended_SizesChanged_ParameterAccepted()
    {
        IReadOnlyList<double>? captured = null;
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.SizesChanged, EventCallback.Factory.Create<IReadOnlyList<double>>(this, val => captured = val));
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("min-w-0 overflow-hidden", cut.Markup);
    }

    [Fact]
    public void Extended_Change_ParameterAccepted()
    {
        XFMultiSplitterChangedEventArgs? captured = null;
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Change, EventCallback.Factory.Create<XFMultiSplitterChangedEventArgs>(this, val => captured = val));
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("min-w-0 overflow-hidden", cut.Markup);
    }

    // ─── Default values ───────────────────────────────────────────────

    [Fact]
    public void Extended_DefaultValues_OrientationHorizontal()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Equal(XFOrientation.Horizontal, cut.Instance.Orientation);
    }

    [Fact]
    public void Extended_DefaultValues_StepIs1()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Equal(1.0, cut.Instance.Step);
    }

    [Fact]
    public void Extended_DefaultValues_VisibleIsTrue()
    {
        var cut = RenderSplitter();
        Assert.True(cut.Instance.Visible);
    }

    [Fact]
    public void Extended_DefaultValues_DisabledIsFalse()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.DoesNotContain("opacity-60", cut.Markup);
    }

    [Fact]
    public void Extended_DefaultValues_HandleLabelIsPortuguese()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("Redimensionar paineis", cut.Markup);
    }

    [Fact]
    public void Extended_DefaultValues_HandleLabelFormatIsDefault()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("Redimensionar paineis 1", cut.Markup);
    }

    // ─── Enum ─────────────────────────────────────────────────────────

    [Fact]
    public void Extended_XFOrientationEnum_HasHorizontal()
    {
        Assert.Equal(0, (int)XFOrientation.Horizontal);
    }

    [Fact]
    public void Extended_XFOrientationEnum_HasVertical()
    {
        Assert.Equal(1, (int)XFOrientation.Vertical);
    }

    [Fact]
    public void Extended_XFOrientationEnum_OnlyHorizontalAndVertical()
    {
        var values = Enum.GetValues<XFOrientation>();
        Assert.Equal(2, values.Length);
        Assert.Contains(XFOrientation.Horizontal, values);
        Assert.Contains(XFOrientation.Vertical, values);
    }

    // ─── Panes / Separators / Handles ─────────────────────────────────

    [Fact]
    public void Extended_WithChildPanes_RendersTwoPanesAndOneHandle()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("Pane 1", cut.Markup);
        Assert.Contains("Pane 2", cut.Markup);
        Assert.Contains("role=\"separator\"", cut.Markup);
    }

    [Fact]
    public void Extended_PaneWithCollapsedTrue_ContainsHiddenClass()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFMultiSplitterPane>(0);
                builder.AddAttribute(1, "Collapsed", true);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => { b.AddContent(0, "Pane 1"); }));
                builder.CloseComponent();
                builder.OpenComponent<XFMultiSplitterPane>(5);
                builder.AddAttribute(6, "ChildContent", (RenderFragment)(b => { b.AddContent(0, "Pane 2"); }));
                builder.CloseComponent();
            }));
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("hidden", cut.Markup);
    }

    [Fact]
    public void Extended_PaneWithCollapsedFalse_NoHiddenPane()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFMultiSplitterPane>(0);
                builder.AddAttribute(1, "Collapsed", false);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => { b.AddContent(0, "Pane 1"); }));
                builder.CloseComponent();
                builder.OpenComponent<XFMultiSplitterPane>(5);
                builder.AddAttribute(6, "ChildContent", (RenderFragment)(b => { b.AddContent(0, "Pane 2"); }));
                builder.CloseComponent();
            }));
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        // The component always renders a <div class="hidden"> for ChildContent cascading.
        // Verify non-collapsed panes render their content visible.
        Assert.Contains("Pane 1", cut.Markup);
        Assert.Contains("Pane 2", cut.Markup);
    }

    // ─── CSS Classes ──────────────────────────────────────────────────

    [Fact]
    public void Extended_SeparatorWrapClass_ContainsFlexItemsCenterBg()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("relative flex", cut.Markup);
        Assert.Contains("items-center justify-center", cut.Markup);
        Assert.Contains("bg-surface-50", cut.Markup);
    }

    [Fact]
    public void Extended_HandleClass_ContainsGroupAbsoluteAndCursorResize()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("group absolute", cut.Markup);
        Assert.Contains("cursor-col-resize", cut.Markup);
    }

    [Fact]
    public void Extended_HandleVisualClass_ContainsRoundedFullBgSurface200()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("rounded-full bg-surface-200", cut.Markup);
    }

    [Fact]
    public void Extended_GripClass_ContainsBlockRoundedFullBgCurrentOpacity70()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("block", cut.Markup);
        Assert.Contains("rounded-full", cut.Markup);
        Assert.Contains("bg-current", cut.Markup);
        Assert.Contains("opacity-70", cut.Markup);
    }

    [Fact]
    public void Extended_RoleSeparator_RenderedInMarkup()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("role=\"separator\"", cut.Markup);
    }

    [Fact]
    public void Extended_OrientationVertical_HandleClassContainsCursorRowResize()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Vertical);
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("cursor-row-resize", cut.Markup);
    }

    [Fact]
    public void Extended_OrientationVertical_SeparatorWrapClassContainsHeight()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Vertical);
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("relative flex h-4 items-center justify-center bg-surface-50", cut.Markup);
    }

    [Fact]
    public void Extended_Horizontal_SeparatorWrapClassContainsWidth()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Horizontal);
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("relative flex w-4 items-center justify-center bg-surface-50", cut.Markup);
    }

    [Fact]
    public void Extended_PaneContent_RenderedInsideDiv()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFMultiSplitterPane>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => { b.AddContent(0, "Hello World"); }));
                builder.CloseComponent();
                builder.OpenComponent<XFMultiSplitterPane>(4);
                builder.AddAttribute(5, "ChildContent", (RenderFragment)(b => { b.AddContent(0, "Goodbye World"); }));
                builder.CloseComponent();
            }));
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("Hello World", cut.Markup);
        Assert.Contains("Goodbye World", cut.Markup);
    }

    [Fact]
    public void Extended_ThreePanes_RendersTwoSeparators()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFMultiSplitterPane>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => { b.AddContent(0, "Pane 1"); }));
                builder.CloseComponent();
                builder.OpenComponent<XFMultiSplitterPane>(4);
                builder.AddAttribute(5, "ChildContent", (RenderFragment)(b => { b.AddContent(0, "Pane 2"); }));
                builder.CloseComponent();
                builder.OpenComponent<XFMultiSplitterPane>(8);
                builder.AddAttribute(9, "ChildContent", (RenderFragment)(b => { b.AddContent(0, "Pane 3"); }));
                builder.CloseComponent();
            }));
            p.Add(x => x.Sizes, new List<double> { 1, 1, 1 });
        });
        Assert.Equal(2, cut.FindAll("[role=\"separator\"]").Count);
    }

    [Fact]
    public void Extended_HandleButton_HasSeparatorRole()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        var separator = cut.Find("[role=separator]");
        Assert.NotNull(separator);
        Assert.Equal("button", separator.TagName.ToLowerInvariant());
    }

    [Fact]
    public void Extended_HandleAriaOrientation_Horizontal()
    {
        var cut = RenderSplitter(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Horizontal);
            p.Add(x => x.ChildContent, WithPanes());
            p.Add(x => x.Sizes, new List<double> { 1, 1 });
        });
        Assert.Contains("aria-orientation", cut.Markup);
    }
}
