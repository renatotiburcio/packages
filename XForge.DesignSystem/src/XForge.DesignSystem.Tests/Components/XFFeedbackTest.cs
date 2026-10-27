using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Alert;
using XForge.DesignSystem.Components.Feedback.Progress;
using XForge.DesignSystem.Components.Feedback.Skeleton;
using XForge.DesignSystem.Components.Feedback.Toast;
using XForge.DesignSystem.Components.DataDisplay.Chip;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components;

public sealed class XFFeedbackTest : BunitContext
{
    public XFFeedbackTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    // Alert
    [Fact] public void Alert_Renders() => Render<XFAlert>();
    [Fact] public void Alert_Title() => Assert.Contains("Warning", Render<XFAlert>(p => p.Add(x => x.Title, "Warning")).Markup);
    [Fact] public void Alert_Text() => Assert.Contains("Error", Render<XFAlert>(p => p.Add(x => x.Text, "Error")).Markup);
    [Fact] public void Alert_IntentSuccess() => Render<XFAlert>(p => p.Add(x => x.Intent, XFIntent.Success));
    [Fact] public void Alert_IntentDanger() => Render<XFAlert>(p => p.Add(x => x.Intent, XFIntent.Danger));
    [Fact] public void Alert_VariantSolid() => Render<XFAlert>(p => p.Add(x => x.Variant, XFAlertVariant.Solid));
    [Fact] public void Alert_VariantOutline() => Render<XFAlert>(p => p.Add(x => x.Variant, XFAlertVariant.Outline));
    [Fact] public void Alert_ShowIconFalse() => Render<XFAlert>(p => p.Add(x => x.ShowIcon, false));
    [Fact] public void Alert_Dismissible() => Render<XFAlert>(p => p.Add(x => x.Dismissible, true));
    [Fact] public void Alert_VisibleFalse() => Render<XFAlert>(p => p.Add(x => x.Visible, false));

    // Progress
    [Fact] public void Progress_Renders() => Render<XFProgress>();
    [Fact] public void Progress_Value() => Render<XFProgress>(p => p.Add(x => x.Value, 50));
    [Fact] public void Progress_Min() => Render<XFProgress>(p => p.Add(x => x.Min, 0));
    [Fact] public void Progress_Max() => Render<XFProgress>(p => p.Add(x => x.Max, 200));
    [Fact] public void Progress_Indeterminate() => Render<XFProgress>(p => p.Add(x => x.Indeterminate, true));
    [Fact] public void Progress_ShowValue() => Render<XFProgress>(p => p.Add(x => x.ShowValue, true));
    [Fact] public void Progress_InlineValue() => Render<XFProgress>(p => p.Add(x => x.InlineValue, true));
    [Fact] public void Progress_RoundedFalse() => Render<XFProgress>(p => p.Add(x => x.Rounded, false));

    // Skeleton
    [Fact] public void Skeleton_Renders() => Render<XFSkeleton>();
    [Fact] public void Skeleton_ShapeCircle() => Render<XFSkeleton>(p => p.Add(x => x.Shape, XFSkeletonShape.Circle));
    [Fact] public void Skeleton_ShapeRectangle() => Render<XFSkeleton>(p => p.Add(x => x.Shape, XFSkeletonShape.Rectangle));
    [Fact] public void Skeleton_AnimationWave() => Render<XFSkeleton>(p => p.Add(x => x.Animation, XFSkeletonAnimation.Shimmer));
    [Fact] public void Skeleton_AnimationNone() => Render<XFSkeleton>(p => p.Add(x => x.Animation, XFSkeletonAnimation.None));
    [Fact] public void Skeleton_SizeSm() => Render<XFSkeleton>(p => p.Add(x => x.Size, XFExtendedSize.Sm));
    [Fact] public void Skeleton_SizeLg() => Render<XFSkeleton>(p => p.Add(x => x.Size, XFExtendedSize.Lg));
    [Fact] public void Skeleton_DecorativeFalse() => Render<XFSkeleton>(p => p.Add(x => x.Decorative, false));
    [Fact] public void Skeleton_VisibleFalse() => Render<XFSkeleton>(p => p.Add(x => x.Visible, false));

    // Toast
    [Fact] public void Toast_Renders() => Render<XFToast>();
    [Fact] public void Toast_RichColors() => Render<XFToast>(p => p.Add(x => x.RichColors, true));
    [Fact] public void Toast_SolidColors() => Render<XFToast>(p => p.Add(x => x.SolidColors, true));
    [Fact] public void Toast_Expanded() => Render<XFToast>(p => p.Add(x => x.Expanded, true));

    // ChipList
    [Fact] public void ChipList_Renders() => Render<XFChipList>();

}