using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Navigation.Steps;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public class XFStepsExtendedTest : XForgeTestContext
{
    public XFStepsExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private static List<XFStepItemModel> CreateItems(int count)
    {
        var items = new List<XFStepItemModel>();
        for (int i = 0; i < count; i++)
            items.Add(new XFStepItemModel { Label = $"Step {i + 1}" });
        return items;
    }

    private IRenderedComponent<XFSteps> RenderSteps(
        List<XFStepItemModel>? items = null,
        Action<ComponentParameterCollectionBuilder<XFSteps>>? configure = null)
    {
        return Render<XFSteps>(p =>
        {
            p.Add(x => x.Items, items ?? CreateItems(3));
            configure?.Invoke(p);
        });
    }

    // VariantClass Plain (line 31)
    [Fact]
    public void Extended_VariantPlain_NoExtraClass()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Variant, XFContainerVariant.Plain));
        Assert.Contains("xf-steps", cut.Markup);
    }

    // VariantClass Soft (line 29)
    [Fact]
    public void Extended_VariantSoft_RendersSoft()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Variant, XFContainerVariant.Soft));
        Assert.Contains("rounded-lg", cut.Markup);
        Assert.Contains("p-3", cut.Markup);
    }

    // VariantClass Surface (line 30)
    [Fact]
    public void Extended_VariantSurface_RendersSurface()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Variant, XFContainerVariant.Surface));
        Assert.Contains("border", cut.Markup);
        Assert.Contains("border-border-light", cut.Markup);
        Assert.Contains("bg-bg-surface", cut.Markup);
    }

    // ListClass Vertical (line 39)
    [Fact]
    public void Extended_OrientationVertical_RendersGridGap()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Orientation, XFOrientation.Vertical));
        Assert.Contains("grid", cut.Markup);
        Assert.Contains("gap-3", cut.Markup);
    }

    // ListClass Horizontal (line 39)
    [Fact]
    public void Extended_OrientationHorizontal_RendersGridFlowCol()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Orientation, XFOrientation.Horizontal));
        Assert.Contains("sm:grid-flow-col", cut.Markup);
    }

    // MarkerClass Sm (line 84)
    [Fact]
    public void Extended_MarkerSizeSm()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("h-7", cut.Markup);
        Assert.Contains("w-7", cut.Markup);
        Assert.Contains("text-xs", cut.Markup);
    }

    // MarkerClass Lg (line 85)
    [Fact]
    public void Extended_MarkerSizeLg()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("h-10", cut.Markup);
        Assert.Contains("w-10", cut.Markup);
        Assert.Contains("text-base", cut.Markup);
    }

    // MarkerClass default Md (line 86)
    [Fact]
    public void Extended_MarkerSizeDefault()
    {
        var cut = RenderSteps();
        Assert.Contains("h-8", cut.Markup);
        Assert.Contains("w-8", cut.Markup);
        Assert.Contains("text-sm", cut.Markup);
    }

    // MarkerClass Complete (line 91)
    [Fact]
    public void Extended_MarkerStatusComplete()
    {
        var items = CreateItems(3);
        items[0].Status = XFStepStatus.Complete;
        var cut = RenderSteps(items);
        Assert.Contains("bg-primary", cut.Markup);
        Assert.Contains("text-primary-contrast", cut.Markup);
    }

    // MarkerClass Current (line 92)
    [Fact]
    public void Extended_MarkerStatusCurrent()
    {
        var items = CreateItems(3);
        items[0].Status = XFStepStatus.Current;
        var cut = RenderSteps(items);
        Assert.Contains("ring-4", cut.Markup);
        Assert.Contains("text-primary", cut.Markup);
    }

    // MarkerClass Error (line 93)
    [Fact]
    public void Extended_MarkerStatusError()
    {
        var items = CreateItems(3);
        items[0].Status = XFStepStatus.Error;
        var cut = RenderSteps(items);
        Assert.Contains("bg-danger", cut.Markup);
        Assert.Contains("text-white", cut.Markup);
    }

    // MarkerClass Disabled (line 94)
    [Fact]
    public void Extended_MarkerStatusDisabled()
    {
        var items = CreateItems(3);
        items[0].Status = XFStepStatus.Disabled;
        var cut = RenderSteps(items);
        Assert.Contains("bg-surface-100", cut.Markup);
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // MarkerClass Pending (line 95)
    [Fact]
    public void Extended_MarkerStatusPending()
    {
        var items = CreateItems(3);
        items[0].Status = XFStepStatus.Pending;
        var cut = RenderSteps(items);
        Assert.Contains("bg-bg-surface", cut.Markup);
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    // LabelClass Current (line 111)
    [Fact]
    public void Extended_LabelStatusCurrent()
    {
        var items = CreateItems(3);
        items[0].Status = XFStepStatus.Current;
        var cut = RenderSteps(items);
        Assert.Contains("text-primary", cut.Markup);
    }

    // LabelClass Complete (line 112)
    [Fact]
    public void Extended_LabelStatusComplete()
    {
        var items = CreateItems(3);
        items[0].Status = XFStepStatus.Complete;
        var cut = RenderSteps(items);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    // LabelClass Error (line 113)
    [Fact]
    public void Extended_LabelStatusError()
    {
        var items = CreateItems(3);
        items[0].Status = XFStepStatus.Error;
        var cut = RenderSteps(items);
        Assert.Contains("text-danger", cut.Markup);
    }

    // LabelClass Pending (line 114)
    [Fact]
    public void Extended_LabelStatusPending()
    {
        var items = CreateItems(3);
        items[0].Status = XFStepStatus.Pending;
        var cut = RenderSteps(items);
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    // LabelClass Size Sm (line 119)
    [Fact]
    public void Extended_LabelSizeSm()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("text-xs", cut.Markup);
    }

    // LabelClass Size Lg (line 120)
    [Fact]
    public void Extended_LabelSizeLg()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("text-base", cut.Markup);
    }

    // DescriptionClass Sm (line 103)
    [Fact]
    public void Extended_DescriptionSizeSm()
    {
        var items = CreateItems(3);
        items[0].Description = "Desc";
        var cut = RenderSteps(items, p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("mt-0.5", cut.Markup);
    }

    // DescriptionClass default (line 104)
    [Fact]
    public void Extended_DescriptionSizeDefault()
    {
        var items = CreateItems(3);
        items[0].Description = "Desc";
        var cut = RenderSteps(items);
        Assert.Contains("mt-1", cut.Markup);
    }

    // ConnectorClass Complete (line 74)
    [Fact]
    public void Extended_ConnectorStatusComplete()
    {
        var items = CreateItems(3);
        items[0].Status = XFStepStatus.Complete;
        var cut = RenderSteps(items);
        Assert.Contains("bg-[var(--color-primary)]", cut.Markup);
    }

    // ConnectorClass Pending (line 74)
    [Fact]
    public void Extended_ConnectorStatusPending()
    {
        var items = CreateItems(3);
        var cut = RenderSteps(items);
        Assert.Contains("bg-border-light", cut.Markup);
    }

    // ConnectorClass last item hidden (line 70-71)
    [Fact]
    public void Extended_ConnectorLastItem_Hidden()
    {
        var items = CreateItems(2);
        var cut = RenderSteps(items);
        Assert.Contains("hidden", cut.Markup);
    }

    // ItemClass Vertical (line 45)
    [Fact]
    public void Extended_ItemClassVertical()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Orientation, XFOrientation.Vertical));
        Assert.Contains("grid-cols-[auto_1fr]", cut.Markup);
    }

    // ItemClass Horizontal (line 46)
    [Fact]
    public void Extended_ItemClassHorizontal()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Orientation, XFOrientation.Horizontal));
        Assert.Contains("sm:grid-cols-1", cut.Markup);
        Assert.Contains("sm:justify-items-center", cut.Markup);
    }

    // ItemClass Disabled opacity (line 43)
    [Fact]
    public void Extended_ItemDisabled_RendersOpacity()
    {
        var items = CreateItems(3);
        items[1].Disabled = true;
        var cut = RenderSteps(items);
        Assert.Contains("opacity-60", cut.Markup);
    }

    // MarkerShellClass Vertical (line 65)
    [Fact]
    public void Extended_MarkerShellVertical()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Orientation, XFOrientation.Vertical));
        Assert.Contains("justify-center", cut.Markup);
    }

    // TextClass Vertical (line 101)
    [Fact]
    public void Extended_TextClassVertical()
    {
        var cut = RenderSteps(configure: p => p.Add(x => x.Orientation, XFOrientation.Vertical));
        Assert.Contains("pb-2", cut.Markup);
    }

    // BaseClass included (line 34)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderSteps();
        Assert.Contains("xf-steps", cut.Markup);
    }
}
