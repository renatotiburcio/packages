using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Layout.Splitter;
using XForge.DesignSystem.Core.Primitives;

using XForge.DesignSystem.Components.Layout;
namespace XForge.DesignSystem.Tests.Components;

public class XFSplitterComponentTest : BunitContext
{
    public XFSplitterComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_HorizontalSplitterWithTwoPanes()
    {
        var cut = Render<XFSplitter>(parameters =>
        {
            parameters.Add(p => p.PrimarySize, 40);
            parameters.Add(p => p.PrimaryContent, "Left");
            parameters.Add(p => p.SecondaryContent, "Right");
            parameters.Add(p => p.AriaLabel, "Resize panels");
        });

        Assert.Contains("grid-template-columns:40fr auto 60fr", cut.Markup);
        Assert.Contains("Left", cut.Markup);
        Assert.Contains("Right", cut.Markup);
        Assert.Contains("aria-label=\"Resize panels\"", cut.Markup);
        Assert.Contains("aria-orientation=\"horizontal\"", cut.Markup);
    }

    [Fact]
    public void Render_VerticalSplitterUsesRows()
    {
        var cut = Render<XFSplitter>(parameters =>
        {
            parameters.Add(p => p.Orientation, XFOrientation.Vertical);
            parameters.Add(p => p.PrimarySize, 60);
        });

        Assert.Contains("grid-template-rows:60fr auto 40fr", cut.Markup);
        Assert.Contains("aria-orientation=\"vertical\"", cut.Markup);
    }

    [Fact]
    public void Render_UsesPortugueseDefaultAriaLabel()
    {
        var cut = Render<XFSplitter>();

        Assert.Contains("aria-label=\"Redimensionar paineis\"", cut.Markup);
        Assert.DoesNotContain("aria-label=\"Resize panes\"", cut.Markup);
    }

    [Fact]
    public void Render_ClampsInitialSize()
    {
        var cut = Render<XFSplitter>(parameters =>
        {
            parameters.Add(p => p.PrimarySize, 5);
            parameters.Add(p => p.Min, 20);
        });

        Assert.Contains("grid-template-columns:20fr auto 80fr", cut.Markup);
    }

    [Fact]
    public async Task Keyboard_UpdatesPrimarySizeChanged()
    {
        double size = 0;
        var cut = Render<XFSplitter>(parameters =>
        {
            parameters.Add(p => p.PrimarySize, 50);
            parameters.Add(p => p.PrimarySizeChanged, value => size = value);
        });

        await cut.Find("[role=separator]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

        Assert.Equal(51, size);
        Assert.Contains("grid-template-columns:51fr auto 49fr", cut.Markup);
    }

    [Fact]
    public async Task Keyboard_RaisesChangedEvent()
    {
        XFSplitterChangedEventArgs? changed = null;
        var cut = Render<XFSplitter>(parameters =>
        {
            parameters.Add(p => p.PrimarySize, 50);
            parameters.Add(p => p.Change, args => changed = args);
        });

        await cut.Find("[role=separator]").KeyDownAsync(new KeyboardEventArgs { Key = "End" });

        Assert.NotNull(changed);
        Assert.Equal(85, changed!.PrimarySize);
        Assert.Equal(15, changed.SecondarySize);
    }

    [Fact]
    public async Task JsInterop_UpdatesSizeContinuouslyAndCommits()
    {
        double size = 0;
        XFSplitterChangedEventArgs? changed = null;
        var cut = Render<XFSplitter>(parameters =>
        {
            parameters.Add(p => p.PrimarySizeChanged, value => size = value);
            parameters.Add(p => p.Change, args => changed = args);
        });

        await cut.Instance.SetSizeFromJsAsync(62, notifyChanged: false);
        await cut.Instance.SetSizeFromJsAsync(64, notifyChanged: true);

        Assert.Equal(64, size);
        Assert.NotNull(changed);
        Assert.Equal(64, changed!.PrimarySize);
    }

    [Fact]
    public void Render_CollapsedPrimaryHidesPrimaryPaneAndSeparator()
    {
        var cut = Render<XFSplitter>(parameters =>
        {
            parameters.Add(p => p.Collapsed, true);
            parameters.Add(p => p.CollapsePrimary, true);
            parameters.Add(p => p.PrimaryContent, "Filters");
            parameters.Add(p => p.SecondaryContent, "Content");
        });

        Assert.DoesNotContain("role=\"separator\"", cut.Markup);
        Assert.Contains("display:none", cut.Markup);
        Assert.Contains("Content", cut.Markup);
    }

    [Fact]
    public void MultiSplitter_RendersMultiplePanesAndHandles()
    {
        var cut = Render<XFMultiSplitter>(parameters =>
        {
            parameters.Add(p => p.ChildContent, MultiSplitterContent());
        });

        Assert.Contains("grid-template-columns:20fr auto 50fr auto 30fr", cut.Markup);
        Assert.Contains("Navigation", cut.Markup);
        Assert.Contains("Workspace", cut.Markup);
        Assert.Contains("Inspector", cut.Markup);
        Assert.Equal(2, cut.FindAll("[data-xf-multisplitter-handle]").Count);
    }

    [Fact]
    public async Task MultiSplitter_KeyboardResizesAdjacentPanes()
    {
        IReadOnlyList<double>? sizes = null;
        XFMultiSplitterChangedEventArgs? changed = null;
        var cut = Render<XFMultiSplitter>(parameters =>
        {
            parameters.Add(p => p.Step, 5);
            parameters.Add(p => p.SizesChanged, value => sizes = value);
            parameters.Add(p => p.Change, args => changed = args);
            parameters.Add(p => p.ChildContent, MultiSplitterContent());
        });

        await cut.Find("[data-xf-multisplitter-handle='0']").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

        Assert.NotNull(sizes);
        Assert.Equal(25, sizes![0]);
        Assert.Equal(45, sizes[1]);
        Assert.Equal(30, sizes[2]);
        Assert.NotNull(changed);
        Assert.Equal(0, changed!.HandleIndex);
    }

    [Fact]
    public void MultiSplitter_VerticalUsesRows()
    {
        var cut = Render<XFMultiSplitter>(parameters =>
        {
            parameters.Add(p => p.Orientation, XFOrientation.Vertical);
            parameters.Add(p => p.ChildContent, MultiSplitterContent());
        });

        Assert.Contains("grid-template-rows:20fr auto 50fr auto 30fr", cut.Markup);
        Assert.Contains("aria-orientation=\"vertical\"", cut.Markup);
    }

    [Fact]
    public void MultiSplitter_WithCustomHandleLabelFormat_UsesLocalizedLabels()
    {
        var cut = Render<XFMultiSplitter>(parameters =>
        {
            parameters.Add(p => p.HandleLabel, "Pane divider");
            parameters.Add(p => p.HandleLabelFormat, "{0} #{1}");
            parameters.Add(p => p.ChildContent, MultiSplitterContent());
        });

        Assert.Contains("aria-label=\"Pane divider #1\"", cut.Markup);
        Assert.Contains("aria-label=\"Pane divider #2\"", cut.Markup);
        Assert.DoesNotContain("aria-label=\"Pane divider 1\"", cut.Markup);
    }

    [Fact]
    public void MultiSplitter_UsesPortugueseDefaultHandleLabels()
    {
        var cut = Render<XFMultiSplitter>(parameters =>
        {
            parameters.Add(p => p.ChildContent, MultiSplitterContent());
        });

        Assert.Contains("aria-label=\"Redimensionar paineis 1\"", cut.Markup);
        Assert.Contains("aria-label=\"Redimensionar paineis 2\"", cut.Markup);
        Assert.DoesNotContain("aria-label=\"Resize panes", cut.Markup);
    }

    private static RenderFragment MultiSplitterContent() => builder =>
    {
        builder.OpenComponent<XFMultiSplitterPane>(0);
        builder.AddAttribute(1, nameof(XFMultiSplitterPane.Size), 20d);
        builder.AddAttribute(2, nameof(XFMultiSplitterPane.Label), "Navigation");
        builder.AddAttribute(3, nameof(XFMultiSplitterPane.ChildContent), Text("Navigation"));
        builder.CloseComponent();

        builder.OpenComponent<XFMultiSplitterPane>(4);
        builder.AddAttribute(5, nameof(XFMultiSplitterPane.Size), 50d);
        builder.AddAttribute(6, nameof(XFMultiSplitterPane.Label), "Workspace");
        builder.AddAttribute(7, nameof(XFMultiSplitterPane.ChildContent), Text("Workspace"));
        builder.CloseComponent();

        builder.OpenComponent<XFMultiSplitterPane>(8);
        builder.AddAttribute(9, nameof(XFMultiSplitterPane.Size), 30d);
        builder.AddAttribute(10, nameof(XFMultiSplitterPane.Label), "Inspector");
        builder.AddAttribute(11, nameof(XFMultiSplitterPane.ChildContent), Text("Inspector"));
        builder.CloseComponent();
    };

    private static RenderFragment Text(string value) => builder => builder.AddContent(0, value);
}
