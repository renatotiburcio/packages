using XForge.DesignSystem.Shared.Setup;
using Xunit;
namespace XForge.DesignSystem.Tests.Setup;
public class XForgeSetupTests
{
    [Fact] public void Defaults_Placeholder_Is_Empty_By_Default() => Assert.Equal(string.Empty, XForgeSetup.Defaults.Placeholder);
    [Fact] public void Defaults_InputSize_Is_Regular() => Assert.Equal(InputSize.Regular, XForgeSetup.Defaults.InputSize);
    [Fact] public void Defaults_LabelType_Is_Automatic() => Assert.Equal(LabelType.Automatic, XForgeSetup.Defaults.LabelType);
    [Fact] public void Defaults_Can_Be_Overridden() { var orig = XForgeSetup.Defaults.InputSize; XForgeSetup.Defaults.InputSize = InputSize.Large; Assert.Equal(InputSize.Large, XForgeSetup.Defaults.InputSize); XForgeSetup.Defaults.InputSize = orig; }
}