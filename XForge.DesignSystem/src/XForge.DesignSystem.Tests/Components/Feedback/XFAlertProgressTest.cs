using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Alert;
using XForge.DesignSystem.Components.Feedback.Progress;
using XForge.DesignSystem.Components.Feedback.Skeleton;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Feedback;

public sealed class XFAlertProgressTest : BunitContext
{
    public XFAlertProgressTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    // Alert
    [Fact] public void Alert_Renders() => Render<XFAlert>();
    [Fact] public void Alert_WithTitle() { var cut = Render<XFAlert>(p => p.Add(x => x.Title, "Warning")); Assert.Contains("Warning", cut.Markup); }
    [Fact] public void Alert_WithText() { var cut = Render<XFAlert>(p => p.Add(x => x.Text, "Error")); Assert.Contains("Error", cut.Markup); }
    [Fact] public void Alert_IntentInfo() => Render<XFAlert>(p => p.Add(x => x.Intent, XFIntent.Info));
    [Fact] public void Alert_IntentSuccess() => Render<XFAlert>(p => p.Add(x => x.Intent, XFIntent.Success));
    [Fact] public void Alert_IntentWarning() => Render<XFAlert>(p => p.Add(x => x.Intent, XFIntent.Warning));
    [Fact] public void Alert_IntentDanger() => Render<XFAlert>(p => p.Add(x => x.Intent, XFIntent.Danger));
    [Fact] public void Alert_VariantSoft() => Render<XFAlert>(p => p.Add(x => x.Variant, XFAlertVariant.Soft));
    [Fact] public void Alert_VariantSolid() => Render<XFAlert>(p => p.Add(x => x.Variant, XFAlertVariant.Solid));
    [Fact] public void Alert_VariantOutline() => Render<XFAlert>(p => p.Add(x => x.Variant, XFAlertVariant.Outline));
    [Fact] public void Alert_SizeSm() => Render<XFAlert>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Alert_SizeLg() => Render<XFAlert>(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void Alert_ShowIconFalse() => Render<XFAlert>(p => p.Add(x => x.ShowIcon, false));
    [Fact] public void Alert_Dismissible() => Render<XFAlert>(p => p.Add(x => x.Dismissible, true));
    [Fact] public void Alert_VisibleFalse() => Render<XFAlert>(p => p.Add(x => x.Visible, false));
    [Fact] public void Alert_AriaLabel() => Render<XFAlert>(p => p.Add(x => x.AriaLabel, "alert"));
    [Fact] public void Alert_LiveAssertive() => Render<XFAlert>(p => p.Add(x => x.Live, XFAriaLive.Assertive));
    [Fact] public void Enums_Intent() => Assert.Equal(8, Enum.GetValues<XFIntent>().Length);
    [Fact] public void Enums_AlertVariant() => Assert.Equal(4, Enum.GetValues<XFAlertVariant>().Length);
    [Fact] public void Enums_AriaLive() => Assert.Equal(3, Enum.GetValues<XFAlertLive>().Length);
    // Progress
    [Fact] public void Progress_Renders() => Render<XFProgress>();
    [Fact] public void Progress_WithMin() => Render<XFProgress>(p => p.Add(x => x.Min, 0));
    [Fact] public void Progress_WithMax() => Render<XFProgress>(p => p.Add(x => x.Max, 200));
    [Fact] public void Progress_Indeterminate() => Render<XFProgress>(p => p.Add(x => x.Indeterminate, true));
    [Fact] public void Progress_ShowValue() => Render<XFProgress>(p => p.Add(x => x.ShowValue, true));
    [Fact] public void Progress_InlineValue() => Render<XFProgress>(p => p.Add(x => x.InlineValue, true));
    [Fact] public void Progress_RoundedFalse() => Render<XFProgress>(p => p.Add(x => x.Rounded, false));
    [Fact] public void Progress_VisibleFalse() => Render<XFProgress>(p => p.Add(x => x.Visible, false));
    [Fact] public void Progress_AriaLabel() => Render<XFProgress>(p => p.Add(x => x.AriaLabel, "Loading"));
    // Skeleton
    [Fact] public void Skeleton_Renders() => Render<XFSkeleton>();
    [Fact] public void Skeleton_AnimationPulse() => Render<XFSkeleton>(p => p.Add(x => x.Animation, XFSkeletonAnimation.Pulse));
    [Fact] public void Skeleton_AnimationWave() => Render<XFSkeleton>(p => p.Add(x => x.Animation, XFSkeletonAnimation.Shimmer));
    [Fact] public void Skeleton_SizeSm() => Render<XFSkeleton>(p => p.Add(x => x.Size, XFExtendedSize.Sm));
    [Fact] public void Skeleton_SizeLg() => Render<XFSkeleton>(p => p.Add(x => x.Size, XFExtendedSize.Lg));
    [Fact] public void Skeleton_DecorativeFalse() => Render<XFSkeleton>(p => p.Add(x => x.Decorative, false));
    [Fact] public void Skeleton_VisibleFalse() => Render<XFSkeleton>(p => p.Add(x => x.Visible, false));
    [Fact] public void Enums_SkeletonAnimation() => Assert.Equal(3, Enum.GetValues<XFSkeletonAnimation>().Length);
    [Fact] public void Enums_ExtendedSize() => Assert.Equal(5, Enum.GetValues<XFExtendedSize>().Length);
}
