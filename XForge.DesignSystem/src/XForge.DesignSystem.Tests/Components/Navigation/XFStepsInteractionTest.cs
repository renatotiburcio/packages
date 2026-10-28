using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Steps;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public sealed class XFStepsInteractionTest : XForgeTestContext
{
    public XFStepsInteractionTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void Renders_Default() => Render<XFSteps>();
    [Fact] public void Renders_WithItems() { var items = new List<XFStepItemModel> { new() { Label = "Step 1" }, new() { Label = "Step 2" } }; Render<XFSteps>(p => p.Add(x => x.Items, items)); }
    [Fact] public void Renders_OrientationHorizontal() => Render<XFSteps>(p => p.Add(x => x.Orientation, XFOrientation.Horizontal));
    [Fact] public void Renders_OrientationVertical() => Render<XFSteps>(p => p.Add(x => x.Orientation, XFOrientation.Vertical));
    [Fact] public void Renders_VariantPlain() => Render<XFSteps>(p => p.Add(x => x.Variant, XFContainerVariant.Plain));
    [Fact] public void Renders_VariantSurface() => Render<XFSteps>(p => p.Add(x => x.Variant, XFContainerVariant.Surface));
    [Fact] public void Renders_SizeSm() => Render<XFSteps>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Renders_SizeLg() => Render<XFSteps>(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void Renders_AriaLabel() => Render<XFSteps>(p => p.Add(x => x.AriaLabel, "Steps"));
    [Fact] public void StepClick_CallbackSet() { var cut = Render<XFSteps>(p => p.Add(x => x.StepClick, EventCallback.Factory.Create<XFStepClickEventArgs>(this, v => { }))); Assert.NotNull(cut); }
    [Fact] public void StepClicking_CallbackSet() { var cut = Render<XFSteps>(p => p.Add(x => x.StepClicking, EventCallback.Factory.Create<XFStepClickEventArgs>(this, v => { }))); Assert.NotNull(cut); }
    [Fact] public void Enums_Orientation() => Assert.Equal(2, Enum.GetValues<XFOrientation>().Length);
    [Fact] public void Enums_ContainerVariant() => Assert.Equal(3, Enum.GetValues<XFContainerVariant>().Length);
    [Fact] public void StepItemModel_Created() { var item = new XFStepItemModel { Label = "Step" }; Assert.Equal("Step", item.Label); }
    [Fact] public void StepItemModel_DefaultDisabled() => Assert.False(new XFStepItemModel().Disabled);
    [Fact] public void StepItemModel_DefaultStatus() => Assert.Equal(XFStepStatus.Pending, new XFStepItemModel().Status);
}
