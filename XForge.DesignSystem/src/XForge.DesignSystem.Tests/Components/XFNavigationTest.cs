#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Accordion;
using XForge.DesignSystem.Components.Navigation.Breadcrumb;
using XForge.DesignSystem.Components.Navigation.Pagination;
using XForge.DesignSystem.Components.Navigation.Steps;
using XForge.DesignSystem.Components.Navigation.Tabs;
using XForge.DesignSystem.Components.Navigation.TOC;
using XForge.DesignSystem.Components.Navigation.Toolbar;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFNavigationTest : BunitContext
{
    public XFNavigationTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region XFTOC Tests

    [Fact]
    public void XFTOC_Render_WithItems()
    {
        var items = new List<XFTOCItem>
        {
            new() { Id = "section-1", Text = "Section 1" },
            new() { Id = "section-2", Text = "Section 2" }
        };

        var cut = Render<XFTOC>(parameters =>
            parameters.Add(p => p.Items, items));

        Assert.NotNull(cut);
        Assert.Contains("xf-toc", cut.Markup);
    }

    [Fact]
    public void XFTOC_Render_WithActiveId()
    {
        var items = new List<XFTOCItem>
        {
            new() { Id = "section-1", Text = "Section 1" },
            new() { Id = "section-2", Text = "Section 2" }
        };

        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.ActiveId, "section-1");
        });

        Assert.NotNull(cut);
        Assert.Contains("section-1", cut.Markup);
    }

    [Fact]
    public void XFTOC_Render_WithItemClicked()
    {
        var items = new List<XFTOCItem>
        {
            new() { Id = "section-1", Text = "Section 1" }
        };

        XFTOCItem? clickedItem = null;
        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.ItemClicked, EventCallback.Factory.Create<XFTOCItem>(this, item => clickedItem = item));
        });

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFTOC_Render_WithOrientation()
    {
        var items = new List<XFTOCItem>
        {
            new() { Id = "section-1", Text = "Section 1" }
        };

        var cut = Render<XFTOC>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Orientation, XFTOCOrientation.Horizontal);
        });

        Assert.NotNull(cut);
        Assert.Contains("overflow-x-auto", cut.Markup);
    }

    #endregion
    #region XFToolbar Tests

    [Fact]
    public void XFToolbar_Render_WithItems()
    {
        var items = new List<XFToolbarItem>
        {
            new() { Id = "action-1", Text = "Action 1" }
        };

        var cut = Render<XFToolbar>(parameters =>
            parameters.Add(p => p.Items, items));

        Assert.NotNull(cut);
        Assert.Contains("xf-toolbar", cut.Markup);
    }

    [Fact]
    public void XFToolbar_Render_WithVariant()
    {
        var items = new List<XFToolbarItem>
        {
            new() { Id = "action-1", Text = "Action 1" }
        };

        var cut = Render<XFToolbar>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Variant, XFToolbarVariant.Bordered);
        });

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFToolbar_Render_WithSize()
    {
        var items = new List<XFToolbarItem>
        {
            new() { Id = "action-1", Text = "Action 1" }
        };

        var cut = Render<XFToolbar>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Size, XFSize.Sm);
        });

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFToolbar_Render_WithOrientation()
    {
        var items = new List<XFToolbarItem>
        {
            new() { Id = "action-1", Text = "Action 1" }
        };

        var cut = Render<XFToolbar>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Orientation, XFOrientation.Vertical);
        });

        Assert.NotNull(cut);
        Assert.Contains("flex-col", cut.Markup);
    }

    #endregion
    #region XFBreadcrumb Tests

    [Fact]
    public void XFBreadcrumb_Render_WithItems()
    {
        var items = new List<XFBreadcrumbItemModel>
        {
            new() { Text = "Home", Href = "/" },
            new() { Text = "Current", Current = true }
        };

        var cut = Render<XFBreadcrumb>(parameters =>
            parameters.Add(p => p.Items, items));

        Assert.NotNull(cut);
        Assert.Contains("xf-breadcrumb", cut.Markup);
    }

    [Fact]
    public void XFBreadcrumb_Render_WithMaxVisibleItems()
    {
        var items = new List<XFBreadcrumbItemModel>
        {
            new() { Text = "Home", Href = "/" },
            new() { Text = "Page 1", Href = "/page1" },
            new() { Text = "Page 2", Href = "/page2" },
            new() { Text = "Current", Current = true }
        };

        var cut = Render<XFBreadcrumb>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.MaxVisibleItems, 3);
        });

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFBreadcrumb_Render_WithCollapseMiddle()
    {
        var items = new List<XFBreadcrumbItemModel>
        {
            new() { Text = "Home", Href = "/" },
            new() { Text = "Page 1", Href = "/page1" },
            new() { Text = "Page 2", Href = "/page2" },
            new() { Text = "Current", Current = true }
        };

        var cut = Render<XFBreadcrumb>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.CollapseMiddle, true);
        });

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFBreadcrumb_Render_WithAriaLabel()
    {
        var items = new List<XFBreadcrumbItemModel>
        {
            new() { Text = "Home", Href = "/" },
            new() { Text = "Current", Current = true }
        };

        var cut = Render<XFBreadcrumb>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.AriaLabel, "Breadcrumb Navigation");
        });

        Assert.NotNull(cut);
        Assert.Contains("Breadcrumb Navigation", cut.Markup);
    }

    #endregion
    #region XFAccordion Tests

    [Fact]
    public void XFAccordion_Render_WithMultiple()
    {
        var cut = Render<XFAccordion>(parameters =>
            parameters.Add(p => p.Multiple, true));

        Assert.NotNull(cut);
        Assert.Contains("xf-accordion", cut.Markup);
    }

    [Fact]
    public void XFAccordion_Render_WithCollapsible()
    {
        var cut = Render<XFAccordion>(parameters =>
            parameters.Add(p => p.Collapsible, false));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFAccordion_Render_WithVariant()
    {
        var cut = Render<XFAccordion>(parameters =>
            parameters.Add(p => p.Variant, XFAccordionVariant.Outline));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFAccordion_Render_WithAnimation()
    {
        var cut = Render<XFAccordion>(parameters =>
            parameters.Add(p => p.Animation, XFAccordionAnimation.SlideFade));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFAccordion_Render_WithIconMode()
    {
        var cut = Render<XFAccordion>(parameters =>
            parameters.Add(p => p.IconMode, XFAccordionIconMode.PlusMinus));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFAccordion_Render_WithItemChanged()
    {
        XFAccordionItemChangeEventArgs? changeArgs = null;
        var cut = Render<XFAccordion>(parameters =>
            parameters.Add(p => p.ItemChanged, EventCallback.Factory.Create<XFAccordionItemChangeEventArgs>(this, args => changeArgs = args)));

        Assert.NotNull(cut);
    }

    #endregion
    #region XFTabs Tests

    [Fact]
    public void XFTabs_Render_WithActiveId()
    {
        var cut = Render<XFTabs>(parameters =>
            parameters.Add(p => p.ActiveId, "tab-1"));

        Assert.NotNull(cut);
        Assert.Contains("xf-tabs", cut.Markup);
    }

    [Fact]
    public void XFTabs_Render_WithVariant()
    {
        var cut = Render<XFTabs>(parameters =>
            parameters.Add(p => p.Variant, XFTabsVariant.Pills));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFTabs_Render_WithSize()
    {
        var cut = Render<XFTabs>(parameters =>
            parameters.Add(p => p.Size, XFSize.Lg));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFTabs_Render_WithOrientation()
    {
        var cut = Render<XFTabs>(parameters =>
            parameters.Add(p => p.Orientation, XFTabsOrientation.Vertical));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFTabs_Render_WithFullWidth()
    {
        var cut = Render<XFTabs>(parameters =>
            parameters.Add(p => p.FullWidth, true));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFTabs_Render_WithShowIndicator()
    {
        var cut = Render<XFTabs>(parameters =>
            parameters.Add(p => p.ShowIndicator, false));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFTabs_Render_WithAllowClose()
    {
        var cut = Render<XFTabs>(parameters =>
            parameters.Add(p => p.AllowClose, true));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFTabs_Render_WithTabChanged()
    {
        XFTabChangeEventArgs? changeArgs = null;
        var cut = Render<XFTabs>(parameters =>
            parameters.Add(p => p.TabChanged, EventCallback.Factory.Create<XFTabChangeEventArgs>(this, args => changeArgs = args)));

        Assert.NotNull(cut);
    }

    #endregion
    #region XFPagination Tests

    [Fact]
    public void XFPagination_Render_WithPageAndPageSize()
    {
        var cut = Render<XFPagination>(parameters =>
        {
            parameters.Add(p => p.Page, 2);
            parameters.Add(p => p.PageSize, 25);
        });

        Assert.NotNull(cut);
        Assert.Contains("xf-pagination", cut.Markup);
    }

    [Fact]
    public void XFPagination_Render_WithTotalItems()
    {
        var cut = Render<XFPagination>(parameters =>
            parameters.Add(p => p.TotalItems, 100));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFPagination_Render_WithVariant()
    {
        var cut = Render<XFPagination>(parameters =>
            parameters.Add(p => p.Variant, XFContainerVariant.Soft));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFPagination_Render_WithSize()
    {
        var cut = Render<XFPagination>(parameters =>
            parameters.Add(p => p.Size, XFSize.Sm));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFPagination_Render_WithAutoHide()
    {
        var cut = Render<XFPagination>(parameters =>
            parameters.Add(p => p.AutoHide, true));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFPagination_Render_WithAllowReload()
    {
        var cut = Render<XFPagination>(parameters =>
            parameters.Add(p => p.AllowReload, true));

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFPagination_Render_WithShowSummary()
    {
        var cut = Render<XFPagination>(parameters =>
            parameters.Add(p => p.ShowSummary, true));

        Assert.NotNull(cut);
    }

    #endregion
    #region XFSteps Tests

    [Fact]
    public void XFSteps_Render_WithItems()
    {
        var items = new List<XFStepItemModel>
        {
            new() { Label = "Step 1", Status = XFStepStatus.Complete },
            new() { Label = "Step 2", Status = XFStepStatus.Current },
            new() { Label = "Step 3", Status = XFStepStatus.Pending }
        };

        var cut = Render<XFSteps>(parameters =>
            parameters.Add(p => p.Items, items));

        Assert.NotNull(cut);
        Assert.Contains("xf-steps", cut.Markup);
    }

    [Fact]
    public void XFSteps_Render_WithOrientation()
    {
        var items = new List<XFStepItemModel>
        {
            new() { Label = "Step 1" },
            new() { Label = "Step 2" }
        };

        var cut = Render<XFSteps>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Orientation, XFOrientation.Vertical);
        });

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFSteps_Render_WithVariant()
    {
        var items = new List<XFStepItemModel>
        {
            new() { Label = "Step 1" },
            new() { Label = "Step 2" }
        };

        var cut = Render<XFSteps>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Variant, XFContainerVariant.Surface);
        });

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFSteps_Render_WithSize()
    {
        var items = new List<XFStepItemModel>
        {
            new() { Label = "Step 1" },
            new() { Label = "Step 2" }
        };

        var cut = Render<XFSteps>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Size, XFSize.Lg);
        });

        Assert.NotNull(cut);
    }

    [Fact]
    public void XFSteps_Render_WithStepClick()
    {
        var items = new List<XFStepItemModel>
        {
            new() { Label = "Step 1" },
            new() { Label = "Step 2" }
        };

        XFStepClickEventArgs? clickArgs = null;
        var cut = Render<XFSteps>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.StepClick, EventCallback.Factory.Create<XFStepClickEventArgs>(this, args => clickArgs = args));
        });

        Assert.NotNull(cut);
    }

    #endregion
}