#region Using directives
#pragma warning disable ASP0006 // Sequence numbers in RenderTreeBuilder are correct when used in loops
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.TreeView;
using XForge.DesignSystem.Components.DataDisplay.Card;
using XForge.DesignSystem.Components.DataDisplay.Badge;
using XForge.DesignSystem.Components.DataDisplay.Chip;
using XForge.DesignSystem.Components.DataDisplay.Timeline;
using XForge.DesignSystem.Components.DataDisplay.Table;
using XForge.Localization;
using XForge.DesignSystem.Components.DataDisplay.Avatar;
using XForge.DesignSystem.Components.DataDisplay.Code;
using XForge.DesignSystem.Components.DataDisplay.Divider;
using XForge.DesignSystem.Components.DataDisplay.Gauge;
using XForge.DesignSystem.Components.DataDisplay.Barcode;
using XForge.DesignSystem.Components.DataDisplay.QRCode;
using XForge.DesignSystem.Components.DataDisplay.Carousel;
using XForge.DesignSystem.Components.DataDisplay.List;
using XForge.DesignSystem.Components.DataDisplay.Image;
using XForge.DesignSystem.Components.DataDisplay.DataGrid;
using XForge.DesignSystem.Components.DataDisplay.Scheduler;
using XForge.DesignSystem.Components.Button;
using XForge.DesignSystem.Components.Feedback.Alert;
using XForge.DesignSystem.Components.Feedback.Progress;
using XForge.DesignSystem.Components.Feedback.Skeleton;
using XForge.DesignSystem.Components.Feedback.SplashScreen;
using XForge.DesignSystem.Components.Feedback.Toast;
using XForge.DesignSystem.Components.Forms.TextInput;
using XForge.DesignSystem.Components.Forms.Textarea;
using XForge.DesignSystem.Components.Forms.Search;
using XForge.DesignSystem.Components.Forms.Checkbox;
using XForge.DesignSystem.Components.Forms.Switch;
using XForge.DesignSystem.Components.Forms.Radio;
using XForge.DesignSystem.Components.Forms.NumberInput;
using XForge.DesignSystem.Components.Forms.Select;
using XForge.DesignSystem.Components.Forms.DatePicker;
using XForge.DesignSystem.Components.Forms.Field;
using XForge.DesignSystem.Components.Forms.EditForm;
using XForge.DesignSystem.Components.Forms.TransferList;
using XForge.DesignSystem.Components.Navigation.Tabs;
using XForge.DesignSystem.Components.Navigation.Pagination;
using XForge.DesignSystem.Components.Navigation.Breadcrumb;
using XForge.DesignSystem.Components.Navigation.Accordion;
using XForge.DesignSystem.Components.Navigation.Steps;
using XForge.DesignSystem.Components.Navigation.Menu;
using XForge.DesignSystem.Components.Navigation.TOC;
using XForge.DesignSystem.Components.Overlays.Dialog;
using XForge.DesignSystem.Components.Overlays.Drawer;
using XForge.DesignSystem.Components.Layout;
using XForge.DesignSystem.Components.Layout.Shell;
using XForge.DesignSystem.Components.Layout.Splitter;
using XForge.DesignSystem.Components.Utilities.Collapse;
using XForge.DesignSystem.Components.Utilities.Animate;
using XForge.DesignSystem.Components.Utilities.FocusTrap;
using XForge.DesignSystem.Components.Utilities.DragDrop;
using XForge.DesignSystem.Components.Utilities.SortableList;
using XForge.DesignSystem.Components.Forms.Upload;
using XForge.DesignSystem.Components.Forms.CurrencyInput;
using XForge.DesignSystem.Components.Forms.DateInput;
using XForge.DesignSystem.Components.Forms.TimeInput;
using XForge.DesignSystem.Components.Forms.InputMask;
using XForge.DesignSystem.Components.Forms.OneTimeInput;
using XForge.DesignSystem.Components.Forms.PasswordStrength;
using XForge.DesignSystem.Components.Forms.RangeSlider;
using XForge.DesignSystem.Components.Forms.SignaturePad;
using XForge.DesignSystem.Components.Overlays.Popover;
using XForge.DesignSystem.Components.Typography;
using XForge.DesignSystem.Components.Forms.FileInput;
using XForge.DesignSystem.Components.DataDisplay.Markdown;
using XForge.DesignSystem.Components.Forms.Rating;
using XForge.DesignSystem.Components.Forms.Slider;
using XForge.DesignSystem.Components.Navigation.Toolbar;
using XForge.DesignSystem.Components.DataDisplay.Video;
using XForge.DesignSystem.Components.Button.FAB;
using XForge.DesignSystem.Components.Button.SplitButton;
using XForge.DesignSystem.Components.Overlays.DropdownMenu;
using XForge.DesignSystem.Components.Overlays.ContextMenu;
using XForge.DesignSystem.Components.Forms.ColorInput;
using XForge.DesignSystem.Components.DataDisplay.Calendar;
using XForge.DesignSystem.Components.Feedback.Chat;
using XForge.DesignSystem.Components.Utilities.KeyCode;
using XForge.DesignSystem.Components.DataDisplay.PdfViewer;
using XForge.DesignSystem.Components.Forms.Validation;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using XForge.DesignSystem.Components.DataDisplay.PivotDataGrid;
using XForge.DesignSystem.Components.DataDisplay.VirtualList;
using XForge.DesignSystem.Components.DataDisplay.Gantt;
using XForge.DesignSystem.Components.DataDisplay.Sankey;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace XForge.DesignSystem.Tests.Components;

/// <summary>
/// Performance smoke tests for heavy components.
/// Verifies that components render within acceptable time bounds and produce reasonable markup size.
/// </summary>
public class XFPerformanceTest : XForgeTestContext
{
    public XFPerformanceTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    /// <summary>
    /// Maximum acceptable render time in milliseconds for a single component.
    /// bUnit renders are much faster than browser renders, so this is a generous bound.
    /// 1000ms allows for CI/slow machine variance while still catching regressions.
    /// </summary>
    private const int MaxRenderTimeMs = 1000;

    /// <summary>
    /// Maximum acceptable render time for complex components (Scheduler, Gantt, DataGrid).
    /// These have multiple views, computed properties, and initialization overhead.
    /// </summary>
    private const int MaxComplexRenderTimeMs = 3000;

    /// <summary>
    /// Maximum acceptable markup size in characters for a single component render.
    /// Helps detect markup bloat.
    /// </summary>
    private const int MaxMarkupSizeBytes = 50_000;

    #region Render Time Tests

    [Fact]
    public void Perf_Button_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFButton>(p => p.AddChildContent("Click"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFButton render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_TextInput_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFTextInput>(p => p.Add(i => i.Value, "test"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTextInput render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Card_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFCard>(p => p.AddChildContent("Card body"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFCard render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Progress_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFProgress>(p => p.Add(pb => pb.Value, 50));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFProgress render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Pagination_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFPagination>(p =>
        {
            p.Add(pg => pg.Page, 5);
            p.Add(pg => pg.TotalItems, 200);
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFPagination render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Tabs_With10Tabs_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFTabs>(p =>
        {
            p.Add(x => x.Tabs, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 10; i++)
                {
                    var idx = i;
                    builder.OpenComponent<XFTab>(idx);
                    builder.AddAttribute(idx + 1, "Label", $"Tab {idx}");
                    builder.CloseComponent();
                }
            }));
            p.Add(x => x.Panels, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 10; i++)
                {
                    var idx = i;
                    builder.OpenComponent<XFTabPanel>(idx);
                    builder.AddAttribute(idx + 1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, $"Content {idx}")));
                    builder.CloseComponent();
                }
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTabs (10 tabs) render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_TreeView_50Nodes_RendersUnderThreshold()
    {
        var nodes = Enumerable.Range(1, 50)
            .Select(i => new XFTreeNode { Id = $"n{i}", Text = $"Node {i}" })
            .ToList();

        var sw = Stopwatch.StartNew();
        var cut = Render<XFTreeView>(p => p.Add(tv => tv.Items, nodes));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTreeView (50 nodes) render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_ChipList_20Chips_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFChipList>(p =>
        {
            p.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 20; i++)
                {
                    var idx = i;
                    builder.OpenComponent<XFChip>(idx);
                    builder.AddAttribute(idx + 1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, $"Chip {idx}")));
                    builder.CloseComponent();
                }
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFChipList (20 chips) render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Typography_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFTypography>(p =>
        {
            p.Add(t => t.Variant, XFTypographyVariant.H1);
            p.AddChildContent("Titulo");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTypography render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_CurrencyInput_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFCurrencyInput>(p =>
        {
            p.Add(c => c.Label, "Valor");
            p.Add(c => c.Placeholder, "0,00");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFCurrencyInput render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_DateInput_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFDateInput>(p =>
        {
            p.Add(d => d.Label, "Data");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFDateInput render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_TimeInput_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFTimeInput>(p =>
        {
            p.Add(t => t.Label, "Horario");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTimeInput render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_InputMask_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFInputMask>(p =>
        {
            p.Add(i => i.Label, "CPF");
            p.Add(i => i.Placeholder, "000.000.000-00");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFInputMask render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_OneTimeInput_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFOneTimeInput>(p =>
        {
            p.Add(o => o.AriaLabel, "Codigo");
            p.Add(o => o.Group, "6");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFOneTimeInput render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_PasswordStrength_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFPasswordStrength>(p =>
        {
            p.Add(pw => pw.Label, "Senha");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFPasswordStrength render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_RangeSlider_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFRangeSlider>(p =>
        {
            p.Add(r => r.Label, "Faixa");
            p.Add(r => r.StartName, "min");
            p.Add(r => r.EndName, "max");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFRangeSlider render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_TreeView_50Nodes_RenderTime()
    {
        var items = Enumerable.Range(1, 50).Select(i => new XFTreeNode
        {
            Id = $"node-{i}",
            Text = $"Node {i}"
        }).ToList();

        var sw = Stopwatch.StartNew();
        var cut = Render<XFTreeView>(p => p.Add(t => t.Items, items));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTreeView (50 nodes) render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Upload_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFUpload>(p =>
        {
            p.Add(u => u.Title, "Upload");
            p.Add(u => u.Description, "Drag files here");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFUpload render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_MultiSplitter_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFMultiSplitter>(p =>
        {
            p.Add(m => m.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFMultiSplitterPane>(0);
                builder.AddAttribute(1, "Size", 50d);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Left")));
                builder.CloseComponent();
                builder.OpenComponent<XFMultiSplitterPane>(3);
                builder.AddAttribute(4, "Size", 50d);
                builder.AddAttribute(5, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Right")));
                builder.CloseComponent();
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFMultiSplitter render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    #endregion

    #region Markup Size Tests

    [Fact]
    public void Perf_Button_MarkupSizeReasonable()
    {
        var cut = Render<XFButton>(p => p.AddChildContent("Click"));
        AssertMarkupSize(cut.Markup, "XFButton");
    }

    [Fact]
    public void Perf_TextInput_MarkupSizeReasonable()
    {
        var cut = Render<XFTextInput>(p => p.Add(i => i.Value, "test"));
        AssertMarkupSize(cut.Markup, "XFTextInput");
    }

    [Fact]
    public void Perf_Card_MarkupSizeReasonable()
    {
        var cut = Render<XFCard>(p => p.AddChildContent("Body content"));
        AssertMarkupSize(cut.Markup, "XFCard");
    }

    [Fact]
    public void Perf_TreeView_50Nodes_MarkupSizeReasonable()
    {
        var nodes = Enumerable.Range(1, 50)
            .Select(i => new XFTreeNode { Id = $"n{i}", Text = $"Node {i}" })
            .ToList();

        var cut = Render<XFTreeView>(p => p.Add(tv => tv.Items, nodes));
        // 50 nodes with ARIA/SVG expand/collapse icons produces ~75K chars — set generous limit
        Assert.True(cut.Markup.Length < 100_000,
            $"XFTreeView (50 nodes) markup is {cut.Markup.Length} chars (limit: 100000)");
    }

    [Fact]
    public void Perf_Tabs_10Tabs_MarkupSizeReasonable()
    {
        var cut = Render<XFTabs>(p =>
        {
            p.Add(x => x.Tabs, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 10; i++)
                {
                    var idx = i;
                    builder.OpenComponent<XFTab>(idx);
                    builder.AddAttribute(idx + 1, "Label", $"Tab {idx}");
                    builder.CloseComponent();
                }
            }));
            p.Add(x => x.Panels, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 10; i++)
                {
                    var idx = i;
                    builder.OpenComponent<XFTabPanel>(idx);
                    builder.AddAttribute(idx + 1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, $"Content {idx}")));
                    builder.CloseComponent();
                }
            }));
        });
        AssertMarkupSize(cut.Markup, "XFTabs (10 tabs)");
    }

    [Fact]
    public void Perf_ChipList_20Chips_MarkupSizeReasonable()
    {
        var cut = Render<XFChipList>(p =>
        {
            p.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 20; i++)
                {
                    var idx = i;
                    builder.OpenComponent<XFChip>(idx);
                    builder.AddAttribute(idx + 1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, $"Chip {idx}")));
                    builder.CloseComponent();
                }
            }));
        });
        AssertMarkupSize(cut.Markup, "XFChipList (20 chips)");
    }

    #endregion

    #region Extended Render Time Tests

    [Fact]
    public void Perf_Textarea_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFTextarea>(p => p.Add(t => t.Value, "test content"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTextarea render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Search_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSearch>(p => p.Add(s => s.Value, "query"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSearch render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Badge_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFBadge>(p => p.AddChildContent("5"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFBadge render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Alert_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFAlert>(p => p.AddChildContent("Warning"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFAlert render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Chip_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFChip>(p => p.AddChildContent("Tag"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFChip render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Breadcrumb_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFBreadcrumb>(p =>
        {
            p.Add(b => b.Items, new List<XFBreadcrumbItemModel>
            {
                new() { Text = "Home", Href = "/" },
                new() { Text = "Products", Href = "/products" },
                new() { Text = "Detail" }
            });
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFBreadcrumb render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Skeleton_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSkeleton>();
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSkeleton render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Collapse_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFCollapse>(p => p.AddChildContent("Hidden content"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFCollapse render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Accordion_WithItems_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFAccordion>(p =>
        {
            p.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 5; i++)
                {
                    var idx = i;
                    builder.OpenComponent<XFAccordionItem>(idx);
                    builder.AddAttribute(idx + 1, "Title", $"Section {idx}");
                    builder.AddAttribute(idx + 2, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, $"Content {idx}")));
                    builder.CloseComponent();
                }
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFAccordion (5 items) render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Checkbox_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFCheckbox>(p => p.Add(c => c.Label, "Check me"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFCheckbox render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Switch_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSwitch>(p => p.Add(s => s.Label, "Toggle"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSwitch render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    #endregion

    #region Extended Markup Size Tests

    [Fact]
    public void Perf_Textarea_MarkupSizeReasonable()
    {
        var cut = Render<XFTextarea>(p => p.Add(t => t.Value, "test content"));
        AssertMarkupSize(cut.Markup, "XFTextarea");
    }

    [Fact]
    public void Perf_Search_MarkupSizeReasonable()
    {
        var cut = Render<XFSearch>(p => p.Add(s => s.Value, "query"));
        AssertMarkupSize(cut.Markup, "XFSearch");
    }

    [Fact]
    public void Perf_Badge_MarkupSizeReasonable()
    {
        var cut = Render<XFBadge>(p => p.AddChildContent("5"));
        AssertMarkupSize(cut.Markup, "XFBadge");
    }

    [Fact]
    public void Perf_Alert_MarkupSizeReasonable()
    {
        var cut = Render<XFAlert>(p => p.AddChildContent("Warning message"));
        AssertMarkupSize(cut.Markup, "XFAlert");
    }

    [Fact]
    public void Perf_Accordion_5Items_MarkupSizeReasonable()
    {
        var cut = Render<XFAccordion>(p =>
        {
            p.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 5; i++)
                {
                    var idx = i;
                    builder.OpenComponent<XFAccordionItem>(idx);
                    builder.AddAttribute(idx + 1, "Title", $"Section {idx}");
                    builder.AddAttribute(idx + 2, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, $"Content {idx}")));
                    builder.CloseComponent();
                }
            }));
        });
        AssertMarkupSize(cut.Markup, "XFAccordion (5 items)");
    }

    [Fact]
    public void Perf_Breadcrumb_MarkupSizeReasonable()
    {
        var cut = Render<XFBreadcrumb>(p =>
        {
            p.Add(b => b.Items, new List<XFBreadcrumbItemModel>
            {
                new() { Text = "Home", Href = "/" },
                new() { Text = "Products", Href = "/products" },
                new() { Text = "Category", Href = "/products/cat" },
                new() { Text = "Detail" }
            });
        });
        AssertMarkupSize(cut.Markup, "XFBreadcrumb");
    }

    #endregion

    #region Additional Render Time Tests

    [Fact]
    public void Perf_Table_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFTable>(p => p.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
        {
            builder.OpenElement(0, "tr");
            builder.OpenElement(1, "td");
            builder.AddContent(2, "Cell");
            builder.CloseElement();
            builder.CloseElement();
        })));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTable render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Avatar_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFAvatar>(p => p.Add(a => a.Initials, "AB"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFAvatar render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Code_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFCode>(p =>
        {
            p.Add(c => c.Text, "Console.WriteLine(\"Hello\");");
            p.Add(c => c.Language, "csharp");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFCode render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Divider_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFDivider>();
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFDivider render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Gauge_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 65);
            p.Add(g => g.Min, 0);
            p.Add(g => g.Max, 100);
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFGauge render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_LinearGauge_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Min, 0);
            p.Add(g => g.Max, 100);
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFLinearGauge render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Radio_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFRadio<string>>(p =>
        {
            p.Add(r => r.Value, "option1");
            p.Add(r => r.Label, "Option 1");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFRadio render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Steps_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSteps>(p =>
        {
            p.Add(s => s.Items, new List<XFStepItemModel>
            {
                new() { Label = "Step 1" },
                new() { Label = "Step 2" },
                new() { Label = "Step 3" },
                new() { Label = "Step 4" },
                new() { Label = "Step 5" }
            });
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSteps render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_NumberInput_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFNumberInput<decimal>>(p => p.Add(n => n.Value, 42m));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFNumberInput render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Barcode_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFBarcode>(p => p.Add(b => b.Value, "1234567890"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFBarcode render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_QRCode_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFQRCode>(p => p.Add(q => q.Value, "https://example.com"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFQRCode render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Splitter_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSplitter>(p =>
        {
            p.Add(s => s.PrimaryContent, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Panel A");
                builder.CloseElement();
            }));
            p.Add(s => s.SecondaryContent, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Panel B");
                builder.CloseElement();
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSplitter render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    #endregion

    #region Additional Markup Size Tests

    [Fact]
    public void Perf_Table_MarkupSizeReasonable()
    {
        var cut = Render<XFTable>(p => p.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
        {
            for (int i = 0; i < 10; i++)
            {
                builder.OpenElement(i * 2, "tr");
                builder.OpenElement(i * 2 + 1, "td");
                builder.AddContent(0, $"Row {i}");
                builder.CloseElement();
                builder.CloseElement();
            }
        })));
        AssertMarkupSize(cut.Markup, "XFTable (10 rows)");
    }

    [Fact]
    public void Perf_Steps_MarkupSizeReasonable()
    {
        var cut = Render<XFSteps>(p =>
        {
            p.Add(s => s.Items, new List<XFStepItemModel>
            {
                new() { Label = "Step 1" },
                new() { Label = "Step 2" },
                new() { Label = "Step 3" },
                new() { Label = "Step 4" },
                new() { Label = "Step 5" }
            });
        });
        AssertMarkupSize(cut.Markup, "XFSteps (5 steps)");
    }

    [Fact]
    public void Perf_Gauge_MarkupSizeReasonable()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 65);
            p.Add(g => g.Min, 0);
            p.Add(g => g.Max, 100);
            p.Add(g => g.ShowTicks, true);
            p.Add(g => g.TickStep, 10);
            p.Add(g => g.ShowLabels, true);
        });
        AssertMarkupSize(cut.Markup, "XFGauge with ticks");
    }

    [Fact]
    public void Perf_Accordion_10Items_MarkupSizeReasonable()
    {
        var cut = Render<XFAccordion>(p =>
        {
            p.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 10; i++)
                {
                    var idx = i;
                    builder.OpenComponent<XFAccordionItem>(idx);
                    builder.AddAttribute(idx + 1, "Title", $"Section {idx}");
                    builder.AddAttribute(idx + 2, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, $"Content {idx}")));
                    builder.CloseComponent();
                }
            }));
        });
        AssertMarkupSize(cut.Markup, "XFAccordion (10 items)");
    }

    [Fact]
    public void Perf_TreeView_100Nodes_MarkupSizeReasonable()
    {
        var nodes = new List<XFTreeNode>();
        for (int i = 0; i < 100; i++)
        {
            nodes.Add(new XFTreeNode { Id = $"n{i}", Text = $"Node {i}" });
        }

        var cut = Render<XFTreeView>(p => p.Add(tv => tv.Items, nodes));
        // 100 nodes with full ARIA/checkbox/icon markup generates ~150K chars — acceptable for enterprise tree
        Assert.True(cut.Markup.Length < 200_000,
            $"XFTreeView (100 nodes) markup is {cut.Markup.Length} chars (limit: 200000)");
    }

    #endregion

    #region Phase 2 Render Time Tests — Overlays

    [Fact]
    public void Perf_Dialog_Open_RendersUnderThreshold()
    {
        JSInterop.SetupVoid("xfDialog.trap", _ => true);
        JSInterop.SetupVoid("xfDialog.release", _ => true);
        var sw = Stopwatch.StartNew();
        var cut = Render<XFDialog>(p =>
        {
            p.Add(d => d.Open, true);
            p.Add(d => d.Title, "Dialog Title");
            p.AddChildContent("Dialog body");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFDialog render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Drawer_Open_RendersUnderThreshold()
    {
        JSInterop.SetupVoid("xfDialog.trap", _ => true);
        JSInterop.SetupVoid("xfDialog.release", _ => true);
        var sw = Stopwatch.StartNew();
        var cut = Render<XFDrawer>(p =>
        {
            p.Add(d => d.Open, true);
            p.Add(d => d.AriaLabel, "Drawer");
            p.AddChildContent("Drawer body");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFDrawer render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    #endregion

    #region Phase 2 Render Time Tests — Forms

    [Fact]
    public void Perf_Select_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(s => s.Label, "Estado");
            p.AddChildContent((RenderFragment)(builder =>
            {
                builder.OpenComponent<XFSelectOption<string>>(0);
                builder.AddAttribute(1, "Value", "SP");
                builder.AddAttribute(2, "Text", "Sao Paulo");
                builder.CloseComponent();
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSelect render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_DatePicker_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFDatePicker>(p =>
        {
            p.Add(d => d.Label, "Data");
            p.Add(d => d.Placeholder, "dd/mm/aaaa");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFDatePicker render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Field_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFField>(p =>
        {
            p.Add(f => f.Label, "Nome");
            p.AddChildContent("input here");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFField render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_EditForm_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFEditForm>(p =>
        {
            p.Add(f => f.Model, new { });
            p.Add(f => f.ChildContent, (RenderFragment<XFEditFormContext>)(ctx => (RenderFragment)(b => b.AddContent(0, "Form content"))));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFEditForm render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_TransferList_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFTransferList>(p =>
        {
            p.Add(t => t.Items, new List<XFTransferListItem>
            {
                new() { Value = "1", Text = "Item 1" },
                new() { Value = "2", Text = "Item 2" },
                new() { Value = "3", Text = "Item 3" }
            });
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTransferList render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    #endregion

    #region Phase 2 Render Time Tests — Layout

    [Fact]
    public void Perf_AppShell_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFAppShell>(p => p.AddChildContent("Main content"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFAppShell render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_AppBar_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFAppBar>(p => p.AddChildContent("Title"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFAppBar render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Sidebar_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSidebar>(p => p.AddChildContent("Nav items"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSidebar render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Footer_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFFooter>(p =>
        {
            p.Add(f => f.AppName, "ERP");
            p.Add(f => f.Version, "v1.0.0");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFFooter render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Grid_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFGrid>(p =>
        {
            p.Add(g => g.Cols, 3);
            p.AddChildContent("Cell");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFGrid render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Stack_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFStack>(p => p.AddChildContent("Stack item"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFStack render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    #endregion

    #region Phase 2 Render Time Tests — Feedback

    [Fact]
    public void Perf_SplashScreen_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSplashScreen>(p =>
        {
            p.Add(s => s.Title, "Loading");
            p.Add(s => s.Message, "Please wait");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSplashScreen render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    #endregion

    #region Phase 2 Render Time Tests — DataDisplay

    [Fact]
    public void Perf_List_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFList>(p =>
        {
            p.AddChildContent((RenderFragment)(builder =>
            {
                for (var i = 0; i < 10; i++)
                {
                    builder.OpenComponent<XFListItem>(i);
                    builder.AddAttribute(i + 1, "Text", $"Item {i}");
                    builder.CloseComponent();
                }
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFList (10 items) render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Image_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFImage>(p =>
        {
            p.Add(i => i.Src, "https://example.com/img.png");
            p.Add(i => i.Alt, "Image");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFImage render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_DataGrid_10Rows_RendersUnderThreshold()
    {
        var data = Enumerable.Range(1, 10)
            .Select(i => new PerfDataGridItem { Id = i, Name = $"Item {i}" })
            .ToList();

        var sw = Stopwatch.StartNew();
        var cut = Render<XFDataGrid<PerfDataGridItem>>(p =>
        {
            p.Add(d => d.Items, data);
            p.Add(d => d.Columns, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFDataGridColumn<PerfDataGridItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                builder.CloseComponent();
                builder.OpenComponent<XFDataGridColumn<PerfDataGridItem>>(3);
                builder.AddAttribute(4, "Title", "Name");
                builder.AddAttribute(5, "Property", "Name");
                builder.CloseComponent();
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFDataGrid (10 rows) render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    #endregion

    #region Phase 2 Render Time Tests — Navigation

    [Fact]
    public void Perf_Menu_With5Items_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFMenu>(p =>
        {
            p.Add(m => m.Items, new List<XFMenuItem>
            {
                new() { Id = "1", Text = "Home" },
                new() { Id = "2", Text = "Reports" },
                new() { Id = "3", Text = "Settings" },
                new() { Id = "4", Text = "Users" },
                new() { Id = "5", Text = "Help" }
            });
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFMenu (5 items) render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_Wizard_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFWizard>(p =>
        {
            p.AddChildContent((RenderFragment)(builder =>
            {
                builder.OpenComponent<XFWizardStep>(0);
                builder.AddAttribute(1, "Label", "Step 1");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Content")));
                builder.CloseComponent();
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFWizard render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_TOC_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFTOC>(p =>
        {
            p.Add(t => t.Items, new List<XFTOCItem>
            {
                new() { Id = "s1", Text = "Section 1" },
                new() { Id = "s2", Text = "Section 2" },
                new() { Id = "s3", Text = "Section 3" }
            });
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTOC render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    #endregion

    #region Phase 2 Render Time Tests — Utilities

    [Fact]
    public void Perf_Animate_RendersUnderThreshold()
    {
        JSInterop.SetupVoid("xfAnimate.observe", _ => true);
        JSInterop.SetupVoid("xfAnimate.release", _ => true);
        var sw = Stopwatch.StartNew();
        var cut = Render<XFAnimate>(p => p.AddChildContent("Animated"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFAnimate render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_FocusTrap_RendersUnderThreshold()
    {
        JSInterop.SetupVoid("xfFocusTrap.activate", _ => true);
        JSInterop.SetupVoid("xfFocusTrap.release", _ => true);
        var sw = Stopwatch.StartNew();
        var cut = Render<XFFocusTrap>(p => p.AddChildContent("Trapped"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFFocusTrap render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_DragDrop_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFDragDrop>(p => p.Add(d => d.Label, "Drop here"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFDragDrop render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_SortableList_10Items_RendersUnderThreshold()
    {
        var items = Enumerable.Range(1, 10).Select(i => $"Item {i}").ToList();
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSortableList<string>>(p =>
        {
            p.Add(s => s.Items, items);
            p.Add(s => s.TextSelector, (string s) => s);
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSortableList (10 items) render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    #endregion

    #region Phase 2 Markup Size Tests

    [Fact]
    public void Perf_Dialog_MarkupSizeReasonable()
    {
        JSInterop.SetupVoid("xfDialog.trap", _ => true);
        JSInterop.SetupVoid("xfDialog.release", _ => true);
        var cut = Render<XFDialog>(p =>
        {
            p.Add(d => d.Open, true);
            p.Add(d => d.Title, "Title");
            p.AddChildContent("Body");
        });
        AssertMarkupSize(cut.Markup, "XFDialog");
    }

    [Fact]
    public void Perf_Drawer_MarkupSizeReasonable()
    {
        JSInterop.SetupVoid("xfDialog.trap", _ => true);
        JSInterop.SetupVoid("xfDialog.release", _ => true);
        var cut = Render<XFDrawer>(p =>
        {
            p.Add(d => d.Open, true);
            p.AddChildContent("Body");
        });
        AssertMarkupSize(cut.Markup, "XFDrawer");
    }

    [Fact]
    public void Perf_Select_MarkupSizeReasonable()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(s => s.Label, "Select");
            p.AddChildContent((RenderFragment)(builder =>
            {
                builder.OpenComponent<XFSelectOption<string>>(0);
                builder.AddAttribute(1, "Value", "a");
                builder.AddAttribute(2, "Text", "Option A");
                builder.CloseComponent();
            }));
        });
        AssertMarkupSize(cut.Markup, "XFSelect");
    }

    [Fact]
    public void Perf_AppShell_MarkupSizeReasonable()
    {
        var cut = Render<XFAppShell>(p => p.AddChildContent("Content"));
        AssertMarkupSize(cut.Markup, "XFAppShell");
    }

    [Fact]
    public void Perf_Menu_5Items_MarkupSizeReasonable()
    {
        var cut = Render<XFMenu>(p =>
        {
            p.Add(m => m.Items, new List<XFMenuItem>
            {
                new() { Id = "1", Text = "Home" },
                new() { Id = "2", Text = "Reports" },
                new() { Id = "3", Text = "Settings" },
                new() { Id = "4", Text = "Users" },
                new() { Id = "5", Text = "Help" }
            });
        });
        AssertMarkupSize(cut.Markup, "XFMenu (5 items)");
    }

    [Fact]
    public void Perf_TransferList_MarkupSizeReasonable()
    {
        var cut = Render<XFTransferList>(p =>
        {
            p.Add(t => t.Items, new List<XFTransferListItem>
            {
                new() { Value = "1", Text = "Item 1" },
                new() { Value = "2", Text = "Item 2" },
                new() { Value = "3", Text = "Item 3" }
            });
        });
        AssertMarkupSize(cut.Markup, "XFTransferList");
    }

    [Fact]
    public void Perf_SortableList_10Items_MarkupSizeReasonable()
    {
        var items = Enumerable.Range(1, 10).Select(i => $"Item {i}").ToList();
        var cut = Render<XFSortableList<string>>(p =>
        {
            p.Add(s => s.Items, items);
            p.Add(s => s.TextSelector, (string s) => s);
        });
        AssertMarkupSize(cut.Markup, "XFSortableList (10 items)");
    }

    [Fact]
    public void Perf_DataGrid_10Rows_MarkupSizeReasonable()
    {
        var data = Enumerable.Range(1, 10)
            .Select(i => new PerfDataGridItem { Id = i, Name = $"Item {i}" })
            .ToList();

        var cut = Render<XFDataGrid<PerfDataGridItem>>(p =>
        {
            p.Add(d => d.Items, data);
            p.Add(d => d.Columns, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFDataGridColumn<PerfDataGridItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                builder.CloseComponent();
                builder.OpenComponent<XFDataGridColumn<PerfDataGridItem>>(3);
                builder.AddAttribute(4, "Title", "Name");
                builder.AddAttribute(5, "Property", "Name");
                builder.CloseComponent();
            }));
        });
        AssertMarkupSize(cut.Markup, "XFDataGrid (10 rows)");
    }

    #endregion

    #region Performance Tests — Snapshot-Only Components (S-G050)

    [Fact]
    public void Perf_Carousel_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFCarousel>(p =>
        {
            p.Add(c => c.Autoplay, false);
            p.Add(c => c.ShowControls, true);
            p.Add(c => c.ChildContent, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenComponent<XFCarouselSlide>(0);
                builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "Slide 1")));
                builder.CloseComponent();
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFCarousel render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_FileInput_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFFileInput>(p =>
        {
            p.Add(f => f.Label, "Upload de arquivo");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFFileInput render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Markdown_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(m => m.Text, "# Titulo\n\nParagrafo com **negrito**.");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFMarkdown render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Rating_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFRating>(p =>
        {
            p.Add(r => r.Value, 3);
            p.Add(r => r.Max, 5);
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFRating render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_SignaturePad_RendersUnderThreshold()
    {
        JSInterop.SetupVoid("xfSignaturePad.register", _ => true);

        var sw = Stopwatch.StartNew();
        var cut = Render<XFSignaturePad>(p =>
        {
            p.Add(sp => sp.Label, "Assinatura");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSignaturePad render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Slider_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSlider>(p =>
        {
            p.Add(s => s.Value, 50);
            p.Add(s => s.Min, 0);
            p.Add(s => s.Max, 100);
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSlider render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Timeline_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFTimeline>(p =>
        {
            p.Add(t => t.ChildContent, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 1; i <= 5; i++)
                {
                    var idx = i;
                    builder.OpenComponent<XFTimelineItem>(idx);
                    builder.AddAttribute(idx + 1, "Title", $"Evento {idx}");
                    builder.AddAttribute(idx + 2, "Text", $"Descricao {idx}");
                    builder.CloseComponent();
                }
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTimeline render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Toast_RendersUnderThreshold()
    {
        var options = new XFToastOptions { Title = "Teste", Message = "Toast message" };
        var item = new XFToastItem(Guid.NewGuid(), options);

        var sw = Stopwatch.StartNew();
        var cut = Render<XFToast>(p =>
        {
            p.Add(t => t.Item, item);
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFToast render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Toolbar_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFToolbar>(p =>
        {
            p.Add(t => t.ChildContent, (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "Toolbar content")));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFToolbar render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    #endregion

    #region Performance Tests — Uncovered Components (S-G050)

    [Fact]
    public void Perf_Video_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFVideo>(p =>
        {
            p.Add(v => v.Src, "https://example.com/video.mp4");
            p.Add(v => v.Type, "video/mp4");
            p.Add(v => v.AriaLabel, "Video de teste");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFVideo render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_FAB_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFFAB>(p =>
        {
            p.Add(f => f.AriaLabel, "Acao principal");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFFAB render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_SplitButton_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSplitButton>(p =>
        {
            p.Add(sb => sb.Text, "Acao");
            p.Add(sb => sb.Items, new List<XFDropdownMenuItem>
            {
                new() { Text = "Item 1" },
                new() { Text = "Item 2" }
            });
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSplitButton render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_DropdownMenu_RendersUnderThreshold()
    {
        JSInterop.SetupVoid("xfDropdownMenu.init", _ => true);
        JSInterop.SetupVoid("xfDropdownMenu.show", _ => true);
        JSInterop.SetupVoid("xfDropdownMenu.hide", _ => true);

        var sw = Stopwatch.StartNew();
        var cut = Render<XFDropdownMenu>(p =>
        {
            p.Add(d => d.ButtonText, "Menu");
            p.Add(d => d.Items, new List<XFDropdownMenuItem>
            {
                new() { Text = "Item 1" },
                new() { Text = "Item 2" }
            });
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFDropdownMenu render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_ColorInput_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFColorInput>(p =>
        {
            p.Add(c => c.Label, "Cor");
            p.Add(c => c.Value, "#ff0000");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFColorInput render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Calendar_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFCalendar>(p =>
        {
            p.Add(c => c.Value, new DateOnly(2026, 5, 19));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFCalendar render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Chat_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFChat>(p =>
        {
            p.Add(c => c.Title, "Chat de teste");
            p.Add(c => c.Placeholder, "Digite...");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFChat render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Video_MarkupSizeReasonable()
    {
        var cut = Render<XFVideo>(p =>
        {
            p.Add(v => v.Src, "https://example.com/video.mp4");
            p.Add(v => v.Type, "video/mp4");
            p.Add(v => v.AriaLabel, "Video de teste");
        });
        AssertMarkupSize(cut.Markup, "XFVideo");
    }

    [Fact]
    public void Perf_Calendar_MarkupSizeReasonable()
    {
        var cut = Render<XFCalendar>(p =>
        {
            p.Add(c => c.Value, new DateOnly(2026, 5, 19));
        });
        AssertMarkupSize(cut.Markup, "XFCalendar");
    }

    [Fact]
    public void Perf_Chat_MarkupSizeReasonable()
    {
        var cut = Render<XFChat>(p =>
        {
            p.Add(c => c.Title, "Chat de teste");
        });
        AssertMarkupSize(cut.Markup, "XFChat");
    }

    #endregion

    #region S-G052: Additional Performance Tests

    [Fact]
    public void Perf_ColorPicker_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFColorPicker>(p =>
        {
            p.Add(x => x.Value, "#3B82F6");
            p.Add(x => x.ShowHsvPicker, true);
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFColorPicker render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_RadioGroup_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFRadioGroup<int>>(p =>
        {
            p.Add(x => x.Label, "Escolha");
            p.Add(x => x.ChildContent, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 5; i++)
                {
                    var idx = i;
                    builder.OpenComponent<XFRadio<int>>(idx);
                    builder.AddAttribute(1, "Value", idx);
                    builder.AddAttribute(2, "Label", $"Opção {idx}");
                    builder.CloseComponent();
                }
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFRadioGroup render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_AvatarGroup_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFAvatarGroup>(p =>
        {
            p.Add(x => x.ChildContent, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 10; i++)
                {
                    builder.OpenComponent<XFAvatar>(i);
                    builder.AddAttribute(1, "Name", $"User {i}");
                    builder.CloseComponent();
                }
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFAvatarGroup render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_SkeletonGroup_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSkeletonGroup>(p =>
        {
            p.Add(x => x.ChildContent, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                for (var i = 0; i < 5; i++)
                {
                    builder.OpenComponent<XFSkeleton>(i);
                    builder.AddAttribute(1, "Width", "100%");
                    builder.AddAttribute(2, "Height", "20px");
                    builder.CloseComponent();
                }
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSkeletonGroup render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_ColorPicker_MarkupSizeReasonable()
    {
        var cut = Render<XFColorPicker>(p =>
        {
            p.Add(x => x.Value, "#3B82F6");
            p.Add(x => x.ShowHsvPicker, true);
        });
        AssertMarkupSize(cut.Markup, "XFColorPicker");
    }

    #endregion

    #region S-G053b: Missing Component Performance Tests

    [Fact]
    public void Perf_SkeletonTable_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFSkeletonTable>(p =>
        {
            p.Add(x => x.Rows, 5);
            p.Add(x => x.Columns, 4);
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSkeletonTable render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Popover_RendersUnderThreshold()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var sw = Stopwatch.StartNew();
        var cut = Render<XFPopover>(p =>
        {
            p.Add(x => x.ButtonText, "Abrir");
            p.AddChildContent("Popover content");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFPopover render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_KeyCode_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFKeyCode>(p =>
        {
            p.Add(x => x.Shortcuts, new List<XFKeyCodeShortcut>
            {
                new() { Key = "s", Ctrl = true, Label = "Salvar" },
                new() { Key = "z", Ctrl = true, Label = "Desfazer" }
            });
            p.AddChildContent("Content");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFKeyCode render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Rail_RendersUnderThreshold()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<XForge.DesignSystem.Services.NavigationStateService>();
        var sw = Stopwatch.StartNew();
        var cut = Render<XFRail>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Nav item")));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFRail render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Figure_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFFigure>(p =>
        {
            p.Add(x => x.Caption, "Test caption");
            p.AddChildContent((RenderFragment)(b =>
            {
                b.OpenElement(0, "img");
                b.AddAttribute(1, "src", "test.png");
                b.CloseElement();
            }));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFFigure render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Highlighter_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFHighlighter>(p =>
        {
            p.Add(x => x.Text, "O rato roeu a roupa do rei de Roma");
            p.Add(x => x.HighlightedText, "rato");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFHighlighter render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Fieldset_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFFieldset>(p =>
        {
            p.Add(x => x.Legend, "Dados pessoais");
            p.AddChildContent("Form fields");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFFieldset render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_PdfViewer_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFPdfViewer>(p =>
        {
            p.Add(x => x.Url, "https://example.com/doc.pdf");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFPdfViewer render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_PdfViewer_CanvasMode_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFPdfViewer>(p =>
        {
            p.Add(x => x.Url, "https://example.com/doc.pdf");
            p.Add(x => x.RenderMode, XFPdfRenderMode.Canvas);
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFPdfViewer Canvas render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_ContextMenu_RendersUnderThreshold()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var items = new List<XFDropdownMenuItem>
        {
            new() { Text = "Copy", Kind = XFDropdownMenuItemKind.Action },
            new() { Text = "Paste", Kind = XFDropdownMenuItemKind.Action }
        };
        var sw = Stopwatch.StartNew();
        var cut = Render<XFContextMenu>(p =>
        {
            p.Add(x => x.Items, items);
            p.AddChildContent("Right-click");
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFContextMenu render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_DrawerProvider_RendersUnderThreshold()
    {
        Services.AddSingleton<XForge.DesignSystem.Components.Overlays.Drawer.XFDrawerService>();
        var sw = Stopwatch.StartNew();
        var cut = Render<XFDrawerProvider>();
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFDrawerProvider render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_DialogProvider_RendersUnderThreshold()
    {
        Services.AddSingleton<XForge.DesignSystem.Components.Overlays.Dialog.XFDialogService>();
        var sw = Stopwatch.StartNew();
        var cut = Render<XFDialogProvider>();
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFDialogProvider render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    [Fact]
    public void Perf_ToastProvider_RendersUnderThreshold()
    {
        Services.AddSingleton<XForge.DesignSystem.Components.Feedback.Toast.XFToastService>();
        var sw = Stopwatch.StartNew();
        var cut = Render<XFToastProvider>();
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFToastProvider render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_PageProgressProvider_RendersUnderThreshold()
    {
        Services.AddSingleton<XForge.DesignSystem.Components.Feedback.Progress.XFPageProgressService>();
        var sw = Stopwatch.StartNew();
        var cut = Render<XFPageProgressProvider>();
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFPageProgressProvider render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
    }

    #endregion

    #region S-G054 Final Coverage Expansion

    [Fact]
    public void Perf_ProgressRing_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFProgressRing>(p => p
            .Add(x => x.Value, 75)
            .Add(x => x.ShowValue, true)
        );
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFProgressRing render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_TableFooter_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFTableFooter>(p => p.AddChildContent("Total: R$ 1.000"));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTableFooter render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Tooltip_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XForge.DesignSystem.Components.Shared.XFTooltip>(p =>
        {
            p.Add(t => t.Text, "Dica");
            p.AddChildContent((RenderFragment)(b => b.AddContent(0, "Hover")));
        });
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTooltip render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_ValidationStatus_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XForge.DesignSystem.Components.Forms.Validation.XFValidationStatus>();
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFValidationStatus render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_ValidationSummary_RendersUnderThreshold()
    {
        var messages = new List<string> { "Erro 1", "Erro 2", "Erro 3" };
        var sw = Stopwatch.StartNew();
        var cut = Render<XForge.DesignSystem.Components.Forms.Validation.XFValidationSummary>(p =>
            p.Add(x => x.Items, messages));
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFValidationSummary render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Chart_RendersUnderThreshold()
    {
        var data = new[] { new ChartTestItem("Jan", 100), new ChartTestItem("Fev", 150), new ChartTestItem("Mar", 200) };
        var sw = Stopwatch.StartNew();
        var cut = Render<XFChart<ChartTestItem>>(p => p
            .Add(x => x.Title, "Receita")
            .Add(x => x.Data, data)
        );
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFChart render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_PivotDataGrid_RendersUnderThreshold()
    {
        var items = Enumerable.Range(1, 20).Select(i =>
            new PivotTestItem($"R{i % 5}", $"C{i % 4}", i * 10)).ToArray();
        var sw = Stopwatch.StartNew();
        var cut = Render<XFPivotDataGrid<PivotTestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.RowSelector, new Func<PivotTestItem, string>(i => i.Region))
            .Add(x => x.ColumnSelector, new Func<PivotTestItem, string>(i => i.Category))
            .Add(x => x.ValueSelector, new Func<PivotTestItem, double>(i => i.Amount))
        );
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFPivotDataGrid render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Scheduler_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XFScheduler<ChartTestItem>>();
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxComplexRenderTimeMs,
            $"XFScheduler render took {sw.ElapsedMilliseconds}ms (limit: {MaxComplexRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_VirtualList_RendersUnderThreshold()
    {
        var items = Enumerable.Range(1, 100).Select(i => $"Item {i}").ToList();
        var sw = Stopwatch.StartNew();
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, items)
        );
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFVirtualList render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_TimeSpan_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XForge.DesignSystem.Components.Forms.TimeSpan.XFTimeSpan>(p => p
            .Add(x => x.Label, "Duração")
        );
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFTimeSpan render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Sparkline_RendersUnderThreshold()
    {
        var items = Enumerable.Range(1, 50).Select(i => new ChartTestItem(i.ToString(), i * 1.5)).ToArray();
        var sw = Stopwatch.StartNew();
        var cut = Render<XForge.DesignSystem.Components.DataDisplay.Sparkline.XFSparkline<ChartTestItem>>(p => p
            .Add(x => x.Data, items)
            .Add(x => x.ValueSelector, new Func<ChartTestItem, double>(i => i.Value))
        );
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSparkline render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Lottie_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XForge.DesignSystem.Components.Feedback.Lottie.XFLottie>(p => p
            .Add(x => x.Path, "https://example.com/animation.json")
        );
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFLottie render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Captcha_RendersUnderThreshold()
    {
        var sw = Stopwatch.StartNew();
        var cut = Render<XForge.DesignSystem.Components.Forms.Captcha.XFCaptcha>(p => p
            .Add(x => x.SiteKey, "6Le-test-key")
        );
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFCaptcha render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    #endregion

    #region XFGantt Performance

    [Fact]
    public void Perf_Gantt_RendersUnderThreshold()
    {
        var tasks = new[]
        {
            new GanttPerfItem("1", null, "Projeto", DateTime.Today, DateTime.Today.AddDays(30), 35),
            new GanttPerfItem("2", "1", "Fase 1", DateTime.Today, DateTime.Today.AddDays(10), 100),
            new GanttPerfItem("3", "1", "Fase 2", DateTime.Today.AddDays(11), DateTime.Today.AddDays(25), 25),
            new GanttPerfItem("4", "3", "Sub 1", DateTime.Today.AddDays(11), DateTime.Today.AddDays(18), 60),
            new GanttPerfItem("5", "3", "Sub 2", DateTime.Today.AddDays(18), DateTime.Today.AddDays(25), 10),
        };
        var sw = Stopwatch.StartNew();
        var cut = Render<XFGantt<GanttPerfItem>>(p => p
            .Add(x => x.Data, tasks)
            .Add(x => x.IdFunc, new Func<GanttPerfItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttPerfItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttPerfItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttPerfItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttPerfItem, DateTime>(t => t.End))
            .Add(x => x.ProgressFunc, new Func<GanttPerfItem, double>(t => t.Progress))
        );
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFGantt render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    #endregion

    #region XFSankey Performance

    [Fact]
    public void Perf_Sankey_RendersUnderThreshold()
    {
        // Warm up bUnit rendering pipeline
        Render<XFSankey<SankeyPerfItem>>(p => p
            .Add(x => x.Data, new List<SankeyPerfItem> { new("W", "X", 1) })
            .Add(x => x.SourceSelector, new Func<SankeyPerfItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<SankeyPerfItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<SankeyPerfItem, double>(f => f.Value))
        );

        var sw = Stopwatch.StartNew();
        var cut = Render<XFSankey<SankeyPerfItem>>(p => p
            .Add(x => x.Data, new List<SankeyPerfItem>
            {
                new("A", "B", 10), new("A", "C", 5), new("B", "D", 7), new("C", "D", 3),
                new("D", "E", 8), new("D", "F", 2),
            })
            .Add(x => x.SourceSelector, new Func<SankeyPerfItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<SankeyPerfItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<SankeyPerfItem, double>(f => f.Value))
        );
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFSankey render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    [Fact]
    public void Perf_Map_RendersUnderThreshold()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var cut = Render<XForge.DesignSystem.Components.DataDisplay.Maps.XFMap>();
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < MaxRenderTimeMs,
            $"XFMap render took {sw.ElapsedMilliseconds}ms (limit: {MaxRenderTimeMs}ms)");
        Assert.NotEmpty(cut.Markup);
    }

    #endregion

    #region Helpers

    private static void AssertMarkupSize(string markup, string componentName)
    {
        Assert.True(markup.Length < MaxMarkupSizeBytes,
            $"{componentName} markup is {markup.Length} chars (limit: {MaxMarkupSizeBytes})");
    }

    private sealed record PerfDataGridItem
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }

    private sealed record ChartTestItem(string Label, double Value);

    private sealed record PivotTestItem(string Region, string Category, double Amount);

    private sealed record GanttPerfItem(string Id, string? ParentId, string Title, DateTime Start, DateTime End, double Progress);

    private sealed record SankeyPerfItem(string Source, string Target, double Value);

    #endregion
}
