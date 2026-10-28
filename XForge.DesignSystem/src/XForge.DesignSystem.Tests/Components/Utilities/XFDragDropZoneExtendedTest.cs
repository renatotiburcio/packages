using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Utilities.DragDrop;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Utilities;

public class XFDragDropZoneExtendedTest : XForgeTestContext
{
    public XFDragDropZoneExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<XFDragDropZone<string>> RenderDragDropZone(
        Action<ComponentParameterCollectionBuilder<XFDragDropZone<string>>>? configure = null)
    {
        return Render<XFDragDropZone<string>>(p => { configure?.Invoke(p); });
    }

    [Fact]
    public void Extended_RootClass_ContainsBaseClasses()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop"))));
        Assert.Contains("xf-drag-drop", cut.Markup);
        Assert.Contains("flex", cut.Markup);
        Assert.Contains("flex-col", cut.Markup);
        Assert.Contains("items-center", cut.Markup);
        Assert.Contains("justify-center", cut.Markup);
        Assert.Contains("rounded-xl", cut.Markup);
        Assert.Contains("border", cut.Markup);
        Assert.Contains("border-dashed", cut.Markup);
        Assert.Contains("text-center", cut.Markup);
        Assert.Contains("outline-none", cut.Markup);
        Assert.Contains("transition-all", cut.Markup);
        Assert.Contains("duration-200", cut.Markup);
        Assert.Contains("ease-out", cut.Markup);
    }

    [Fact]
    public void Extended_SizeClass_Sm()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Size, XFSize.Sm));
        Assert.Contains("min-h-24", cut.Markup);
        Assert.Contains("gap-2", cut.Markup);
        Assert.Contains("p-4", cut.Markup);
    }

    [Fact]
    public void Extended_SizeClass_Md()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Size, XFSize.Md));
        Assert.Contains("min-h-36", cut.Markup);
        Assert.Contains("gap-3", cut.Markup);
        Assert.Contains("p-6", cut.Markup);
    }

    [Fact]
    public void Extended_SizeClass_Lg()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Size, XFSize.Lg));
        Assert.Contains("min-h-52", cut.Markup);
        Assert.Contains("gap-4", cut.Markup);
        Assert.Contains("p-8", cut.Markup);
    }

    [Fact]
    public void Extended_StateClass_Idle()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop"))));
        Assert.Contains("border-border", cut.Markup);
        Assert.Contains("bg-bg-surface", cut.Markup);
    }

    [Fact]
    public void Extended_StateClass_DragOver()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop"))));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("ring-2", cut.Markup);
    }

    [Fact]
    public void Extended_StateClass_Rejected()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.AcceptTypes, new List<string> { "image/png" })
            .Add(x => x.RejectUnknownTypes, true));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("border-danger", cut.Markup);
        Assert.Contains("ring-2", cut.Markup);
    }

    [Fact]
    public void Extended_ToneBorderClass_Success()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Success));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("border-success", cut.Markup);
    }

    [Fact]
    public void Extended_ToneBorderClass_Warning()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Warning));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("border-warning", cut.Markup);
    }

    [Fact]
    public void Extended_ToneBorderClass_Danger()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Danger));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("border-danger", cut.Markup);
    }

    [Fact]
    public void Extended_ToneBorderClass_Default()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Default));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("border-border", cut.Markup);
    }

    [Fact]
    public void Extended_ToneBorderClass_Primary()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Primary));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("border-primary", cut.Markup);
    }

    [Fact]
    public void Extended_ToneBackgroundClass_Success()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Success));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-success)_7%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneBackgroundClass_Warning()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Warning));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-warning)_8%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneBackgroundClass_Danger()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Danger));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-danger)_7%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneBackgroundClass_Default()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Default));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("bg-surface-50", cut.Markup);
    }

    [Fact]
    public void Extended_ToneBackgroundClass_Primary()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Primary));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneRingClass_Success()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Success));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("ring-[color-mix(in_oklab,var(--color-success)_18%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneRingClass_Warning()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Warning));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("ring-[color-mix(in_oklab,var(--color-warning)_22%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneRingClass_Danger()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Danger));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("ring-[color-mix(in_oklab,var(--color-danger)_18%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneRingClass_Default()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Default));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("ring-surface-200/80", cut.Markup);
    }

    [Fact]
    public void Extended_ToneRingClass_Primary()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Tone, XFSemanticColor.Primary));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_FullWidth_True()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.FullWidth, true));
        Assert.Contains("w-full", cut.Markup);
    }

    [Fact]
    public void Extended_FullWidth_False()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.FullWidth, false));
        Assert.Contains("inline-flex", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled_HasDisabledStyles()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Disabled, true));
        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("opacity-60", cut.Markup);
    }

    [Fact]
    public void Extended_Enabled_HasCursorDefault()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Disabled, false));
        Assert.Contains("cursor-default", cut.Markup);
    }

    [Fact]
    public void Extended_TitleClass()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.Label, "Upload"));
        Assert.Contains("text-sm", cut.Markup);
        Assert.Contains("font-semibold", cut.Markup);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    [Fact]
    public void Extended_DescriptionClass()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.Description, "Max 10MB"));
        Assert.Contains("text-sm", cut.Markup);
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_BaseClass()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop"))));
        Assert.Contains("xf-drag-drop", cut.Markup);
    }

    [Fact]
    public void Extended_Label_ResolvedLabelText()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.Label, "Custom Label"));
        Assert.Contains("Custom Label", cut.Markup);
    }

    [Fact]
    public void Extended_DefaultLabel_WhenLabelNotSet()
    {
        var cut = RenderDragDropZone();
        Assert.Contains("Solte aqui", cut.Markup);
    }

    [Fact]
    public void Extended_Description_RenderedWhenSet()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.Description, "File upload description"));
        Assert.Contains("File upload description", cut.Markup);
    }

    [Fact]
    public void Extended_ChildContent_RenderedWhenProvided()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Custom child"))));
        Assert.Contains("Custom child", cut.Markup);
    }

    [Fact]
    public void Extended_StateContent_RenderedWhenProvided()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Child")))
            .Add(x => x.StateContent, (RenderFragment<XFDragDropState>)(state => builder => builder.AddContent(0, "State: " + state))));
        Assert.Contains("State: Idle", cut.Markup);
    }

    [Fact]
    public void Extended_StateContent_TakesPriorityOverChildContent()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Child")))
            .Add(x => x.StateContent, (RenderFragment<XFDragDropState>)(state => builder => builder.AddContent(0, "Priority"))));
        Assert.Contains("Priority", cut.Markup);
        Assert.DoesNotContain("Child", cut.Markup);
    }

    [Fact]
    public void Extended_AcceptTypes_Parameter()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.AcceptTypes, new List<string> { "image/png", "image/jpeg" }));
        Assert.Contains("xf-drag-drop", cut.Markup);
    }

    [Fact]
    public void Extended_Payload_Parameter()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Payload, "test-payload"));
        Assert.Contains("xf-drag-drop", cut.Markup);
    }

    [Fact]
    public void Extended_RejectUnknownTypes_Parameter()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.AcceptTypes, new List<string> { "image/png" })
            .Add(x => x.RejectUnknownTypes, true));
        Assert.Contains("xf-drag-drop", cut.Markup);
    }

    [Fact]
    public void Extended_DataState_Idle()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop"))));
        Assert.Contains("data-state=\"idle\"", cut.Markup);
    }

    [Fact]
    public void Extended_DataState_DragOver()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop"))));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("data-state=\"dragover\"", cut.Markup);
    }

    [Fact]
    public void Extended_DataState_Rejected()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.AcceptTypes, new List<string> { "image/png" })
            .Add(x => x.RejectUnknownTypes, true));
        var div = cut.Find("div");
        cut.InvokeAsync(() => div.TriggerEventAsync("ondragenter", new DragEventArgs()));
        Assert.Contains("data-state=\"rejected\"", cut.Markup);
    }

    [Fact]
    public void Extended_RootElement_IsDiv()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop"))));
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Extended_RootElement_HasRoleButton()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop"))));
        Assert.Contains("role=\"button\"", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled_HasAriaDisabled()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Disabled, true));
        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled_HasTabIndexMinus1()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Disabled, true));
        Assert.Contains("tabindex=\"-1\"", cut.Markup);
    }

    [Fact]
    public void Extended_Enabled_HasTabIndex0()
    {
        var cut = RenderDragDropZone(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
            .Add(x => x.Disabled, false));
        Assert.Contains("tabindex=\"0\"", cut.Markup);
    }

    [Fact]
    public void Extended_Description_NotRenderedWhenEmpty()
    {
        var cut = RenderDragDropZone(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop"))));
        Assert.DoesNotContain("text-text-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_DefaultContent_RendersIconAndLabel()
    {
        var cut = RenderDragDropZone();
        Assert.Contains("Solte aqui", cut.Markup);
        Assert.Contains("xf-drag-drop", cut.Markup);
    }

    [Fact]
    public void Extended_AllTones_AllSizes_Combination()
    {
        foreach (XFSemanticColor tone in Enum.GetValues<XFSemanticColor>())
        {
            foreach (XFSize size in Enum.GetValues<XFSize>())
            {
                var cut = RenderDragDropZone(p => p
                    .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drop")))
                    .Add(x => x.Tone, tone)
                    .Add(x => x.Size, size));
                Assert.Contains("xf-drag-drop", cut.Markup);
            }
        }
    }
}
