using XForge.DesignSystem.Components.Base;
using XForge.DesignSystem.Shared.Setup;
using Xunit;
namespace XForge.DesignSystem.Tests.Base;
public class HxInputBaseTests
{
    [Fact] public void XFEditContext_Default_Placeholder_Is_Null() => Assert.Null(new XFEditContext().Placeholder);
    [Fact] public void XFEditContext_Default_InputSize_Is_Null() => Assert.Null(new XFEditContext().InputSize);
    [Fact] public void XFEditContext_Default_LabelType_Is_Null() => Assert.Null(new XFEditContext().LabelType);
    [Fact] public void XFEditContext_Can_Be_Initialized_With_Values() { var c = new XFEditContext { Placeholder = "x", InputSize = InputSize.Large, LabelType = LabelType.Floating }; Assert.Equal("x", c.Placeholder); Assert.Equal(InputSize.Large, c.InputSize); Assert.Equal(LabelType.Floating, c.LabelType); }
}