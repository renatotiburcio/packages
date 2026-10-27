using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Accordion;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public sealed class XFAccordionInteractionTest : BunitContext
{
    public XFAccordionInteractionTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void Renders_Default() => Render<XFAccordion>();
    [Fact] public void Renders_WithChildContent() => Render<XFAccordion>(p => p.Add(x => x.ChildContent, builder => builder.AddMarkupContent(0, "<div>Item</div>")));
    [Fact] public void Renders_Multiple() => Render<XFAccordion>(p => p.Add(x => x.Multiple, true));
    [Fact] public void Renders_CollapsibleFalse() => Render<XFAccordion>(p => p.Add(x => x.Collapsible, false));
    [Fact] public void Renders_VariantSurface() => Render<XFAccordion>(p => p.Add(x => x.Variant, XFAccordionVariant.Surface));
    [Fact] public void Renders_VariantOutlined() => Render<XFAccordion>(p => p.Add(x => x.Variant, XFAccordionVariant.Soft));
    [Fact] public void Renders_SizeSm() => Render<XFAccordion>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Renders_SizeLg() => Render<XFAccordion>(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void Renders_AnimationHeightFade() => Render<XFAccordion>(p => p.Add(x => x.Animation, XFAccordionAnimation.HeightFade));
    [Fact] public void Renders_AnimationSlide() => Render<XFAccordion>(p => p.Add(x => x.Animation, XFAccordionAnimation.SlideFade));
    [Fact] public void Renders_IconModeChevron() => Render<XFAccordion>(p => p.Add(x => x.IconMode, XFAccordionIconMode.Chevron));
    [Fact] public void Renders_IconModePlus() => Render<XFAccordion>(p => p.Add(x => x.IconMode, XFAccordionIconMode.PlusMinus));
    [Fact] public void Renders_StaggerContent() => Render<XFAccordion>(p => p.Add(x => x.StaggerContent, true));
    [Fact] public void ItemChanged_CallbackSet() { var cut = Render<XFAccordion>(p => p.Add(x => x.ItemChanged, EventCallback.Factory.Create<XFAccordionItemChangeEventArgs>(this, v => { }))); Assert.NotNull(cut); }
    [Fact] public void Enums_AccordionVariant() => Assert.Equal(4, Enum.GetValues<XFAccordionVariant>().Length);
    [Fact] public void Enums_AccordionAnimation() => Assert.Equal(5, Enum.GetValues<XFAccordionAnimation>().Length);
    [Fact] public void Enums_AccordionIconMode() => Assert.Equal(3, Enum.GetValues<XFAccordionIconMode>().Length);
}
