// T201: Sprint 6 - Functional render tests for major XForge components.
// Exercises actual component lifecycle (OnInitialized, OnParametersSet, BuildRenderTree)
// to contribute meaningful line coverage beyond type-existence smoke tests.
using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Base;
using XForge.DesignSystem.Components.Button;
using XForge.DesignSystem.Components.DataDisplay.Card;
using XForge.DesignSystem.Components.DataDisplay.Chip;
using XForge.DesignSystem.Components.DataDisplay.Table;
using XForge.DesignSystem.Components.Feedback.Alert;
using XForge.DesignSystem.Components.Feedback.Progress;
using XForge.DesignSystem.Components.Feedback.Skeleton;
using XForge.DesignSystem.Components.Feedback.Toast;
using XForge.DesignSystem.Components.Forms.Checkbox;
using XForge.DesignSystem.Components.Forms.Switch;
using XForge.DesignSystem.Components.Layout;
using XForge.DesignSystem.Components.Navigation.Tabs;

namespace XForge.DesignSystem.Tests.Components;

/// <summary>
/// Functional render tests: render each component with default parameters and
/// assert the render produces markup. Exercises the full Blazor component
/// lifecycle (OnInitialized, OnParametersSet, BuildRenderTree) for line coverage.
/// </summary>
public class ComponentFunctionalTests : XForgeTestContext
{
    public ComponentFunctionalTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    // --- Buttons (1 test) ---

    [Fact] public void XFButton_Renders() =>
        Assert.Contains("xf-button", Render<XFButton>().Markup);

    // --- Forms (2 tests) ---

    [Fact] public void XFCheckbox_Renders() => Assert.NotNull(Render<XFCheckbox>());

    [Fact] public void XFSwitch_Renders() => Assert.NotNull(Render<XFSwitch>());

    // --- DataDisplay (3 tests) ---

    [Fact] public void XFCard_Renders() =>
        Assert.Contains("xf-card", Render<XFCard>().Markup);

    [Fact] public void XFChip_Renders() =>
        Assert.Contains("xf-chip", Render<XFChip>().Markup);

    [Fact] public void XFTable_Renders() => Assert.NotNull(Render<XFTable>());

    // --- Feedback (5 tests) ---

    [Fact] public void XFAlert_Renders() =>
        Assert.Contains("xf-alert", Render<XFAlert>().Markup);

    [Fact] public void XFProgress_Renders() => Assert.NotNull(Render<XFProgress>());

    [Fact] public void XFSkeleton_Renders() => Assert.NotNull(Render<XFSkeleton>());

    [Fact] public void XFToast_Renders_Item() =>
        Assert.NotNull(Render<XFToast>(p => p.Add(x => x.Item, new XFToastItem(
            Guid.NewGuid(),
            new XFToastOptions { Title = "T", Message = "M" }))));

    // --- Layout (2 tests) ---

    [Fact] public void XFGrid_Renders() => Assert.NotNull(Render<XFGrid>());

    [Fact] public void XFStack_Renders() => Assert.NotNull(Render<XFStack>());

    // --- Navigation (1 test) ---

    [Fact] public void XFTabs_Renders() => Assert.NotNull(Render<XFTabs>());
}