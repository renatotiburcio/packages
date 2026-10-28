using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Overlays.ContextMenu;
using XForge.DesignSystem.Components.Overlays.DropdownMenu;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Overlays;

public sealed class XFContextMenuExtendedTest : XForgeTestContext
{
    public XFContextMenuExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupVoid("xfFocusTrap.activate", _ => true);
        JSInterop.SetupVoid("xfFocusTrap.release", _ => true);
    }

    private static readonly IReadOnlyList<XFDropdownMenuItem> DefaultItems = new[]
    {
        new XFDropdownMenuItem { Id = "copy", Text = "Copy", Value = "copy" },
        new XFDropdownMenuItem { Id = "paste", Text = "Paste", Value = "paste" },
        new XFDropdownMenuItem { Id = "sep1", Kind = XFDropdownMenuItemKind.Separator },
        new XFDropdownMenuItem { Id = "delete", Text = "Delete", Tone = XFSemanticColor.Danger }
    };

    private IRenderedComponent<XFContextMenu> RenderContextMenu(
        Action<ComponentParameterCollectionBuilder<XFContextMenu>>? configure = null)
    {
        return Render<XFContextMenu>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Right-click me")));
            p.Add(x => x.Items, DefaultItems);
            p.Add(x => x.Open, true);
            configure?.Invoke(p);
        });
    }

    private IRenderedComponent<XFContextMenu> RenderClosed(
        Action<ComponentParameterCollectionBuilder<XFContextMenu>>? configure = null)
    {
        return Render<XFContextMenu>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Right-click me")));
            p.Add(x => x.Items, DefaultItems);
            p.Add(x => x.Open, false);
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_BaseClass_ContainsContextMenuContentsClass()
    {
        var cut = RenderClosed();
        Assert.Contains("xf-context-menu", cut.Markup);
        Assert.Contains("contents", cut.Markup);
    }

    [Fact]
    public void Extended_SizeClass_Sm_ContainsW56()
    {
        var cut = RenderContextMenu(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("w-56", cut.Markup);
    }

    [Fact]
    public void Extended_SizeClass_Md_Default_ContainsW72()
    {
        var cut = RenderContextMenu();
        Assert.Contains("w-72", cut.Markup);
    }

    [Fact]
    public void Extended_SizeClass_Lg_ContainsW96()
    {
        var cut = RenderContextMenu(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("w-96", cut.Markup);
    }

    [Fact]
    public void Extended_Class_CustomClassAppended()
    {
        var cut = RenderClosed(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("xf-context-menu", cut.Markup);
        Assert.Contains("contents", cut.Markup);
        Assert.Contains("my-custom-class", cut.Markup);
    }

    [Fact]
    public void Extended_Style_CustomStyleApplied()
    {
        var cut = RenderClosed(p => p.Add(x => x.Style, "color:red"));
        Assert.Contains("color:red", cut.Markup);
    }

    [Fact]
    public void Extended_Id_CustomIdApplied()
    {
        var cut = RenderClosed(p => p.Add(x => x.Id, "ctx-menu-1"));
        Assert.Contains("ctx-menu-1", cut.Markup);
    }

    [Fact]
    public void Extended_AdditionalAttributes_AppliedToWrapper()
    {
        var cut = RenderClosed(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "context-menu" }
        }));
        Assert.Contains("data-testid", cut.Markup);
        Assert.Contains("context-menu", cut.Markup);
    }

    [Fact]
    public void Extended_AriaLabel_DefaultIsMenuDeContexto()
    {
        var cut = RenderContextMenu();
        Assert.Contains("Menu de contexto", cut.Markup);
    }

    [Fact]
    public void Extended_AriaLabel_CustomValue()
    {
        var cut = RenderContextMenu(p => p.Add(x => x.AriaLabel, "Custom context menu"));
        Assert.Contains("Custom context menu", cut.Markup);
        Assert.DoesNotContain("Menu de contexto", cut.Markup);
    }

    [Fact]
    public void Extended_SubmenuAriaLabelFormat_CustomFormat()
    {
        var cut = Render<XFContextMenu>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Right-click me")));
            p.Add(x => x.Items, new[]
            {
                new XFDropdownMenuItem
                {
                    Id = "parent",
                    Text = "Parent",
                    Children = new[] { new XFDropdownMenuItem { Id = "child", Text = "Child" } }
                }
            });
            p.Add(x => x.Open, true);
            p.Add(x => x.SubmenuAriaLabelFormat, "Submenu: {0}");
        });
        Assert.Contains("xf-context-menu", cut.Markup);
        Assert.Equal("Submenu: {0}", cut.Instance.SubmenuAriaLabelFormat);
    }

    [Fact]
    public void Extended_CloseOnSelect_ParameterAccepted()
    {
        var cut = RenderContextMenu(p => p.Add(x => x.CloseOnSelect, false));
        Assert.Contains("xf-context-menu", cut.Markup);
    }

    [Fact]
    public void Extended_CloseOnEscape_ParameterAccepted()
    {
        var cut = RenderContextMenu(p => p.Add(x => x.CloseOnEscape, false));
        Assert.Contains("xf-context-menu", cut.Markup);
    }

    [Fact]
    public void Extended_CloseOnOutsideClick_ParameterAccepted()
    {
        var cut = RenderContextMenu(p => p.Add(x => x.CloseOnOutsideClick, false));
        Assert.Contains("xf-context-menu", cut.Markup);
    }

    [Fact]
    public void Extended_CollisionPadding_ParameterAccepted()
    {
        var cut = RenderContextMenu(p => p.Add(x => x.CollisionPadding, 20));
        Assert.Contains("xf-context-menu", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled_ParameterAccepted()
    {
        var cut = RenderContextMenu(p => p.Add(x => x.Disabled, true));
        Assert.Contains("xf-context-menu", cut.Markup);
    }

    [Fact]
    public void Extended_Open_False_DoesNotRenderMenu()
    {
        var cut = RenderClosed();
        Assert.Contains("xf-context-menu", cut.Markup);
        Assert.DoesNotContain("role=\"menu\"", cut.Markup);
    }

    [Fact]
    public void Extended_Open_True_RendersMenu()
    {
        var cut = RenderContextMenu();
        Assert.Contains("role=\"menu\"", cut.Markup);
    }

    [Fact]
    public void Extended_Items_EmptyList()
    {
        var cut = Render<XFContextMenu>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Target")));
            p.Add(x => x.Items, Array.Empty<XFDropdownMenuItem>());
            p.Add(x => x.Open, true);
        });
        Assert.Contains("xf-context-menu", cut.Markup);
        Assert.Contains("role=\"menu\"", cut.Markup);
    }

    [Fact]
    public void Extended_Items_WithItems_RendersItemText()
    {
        var cut = RenderContextMenu();
        Assert.Contains("Copy", cut.Markup);
        Assert.Contains("Paste", cut.Markup);
        Assert.Contains("Delete", cut.Markup);
    }

    [Fact]
    public void Extended_ItemSelected_EventCallbackAccepted()
    {
        XFDropdownMenuItem? selected = null;
        var cut = RenderContextMenu(p => p.Add(
            x => x.ItemSelected,
            EventCallback.Factory.Create<XFDropdownMenuItem>(this, item => selected = item)));
        Assert.Contains("xf-context-menu", cut.Markup);
    }

    [Fact]
    public void Extended_ChildContent_RendersInsideWrapper()
    {
        var cut = RenderClosed();
        Assert.Contains("Right-click me", cut.Markup);
    }

    [Fact]
    public void Extended_AdditionalAttributes_VisibleFalse_HidesComponent()
    {
        var cut = RenderClosed(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "hidden", "hidden" }
        }));
        Assert.Contains("hidden", cut.Markup);
    }

    [Fact]
    public void Extended_DefaultValues_VerifyDefaults()
    {
        var cut = Render<XFContextMenu>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Target")));
            p.Add(x => x.Items, DefaultItems);
        });

        Assert.False(cut.Instance.Open);
        Assert.True(cut.Instance.CloseOnSelect);
        Assert.True(cut.Instance.CloseOnEscape);
        Assert.True(cut.Instance.CloseOnOutsideClick);
        Assert.Equal(8, cut.Instance.CollisionPadding);
        Assert.Equal(XFSize.Md, cut.Instance.Size);
        Assert.Equal("Menu de contexto", cut.Instance.AriaLabel);
        Assert.Equal("{0}", cut.Instance.SubmenuAriaLabelFormat);
    }

    [Fact]
    public void Extended_XFDropdownMenuItemKind_EnumHasAllValues()
    {
        var values = Enum.GetValues<XFDropdownMenuItemKind>();
        Assert.Contains(XFDropdownMenuItemKind.Action, values);
        Assert.Contains(XFDropdownMenuItemKind.XFCheckbox, values);
        Assert.Contains(XFDropdownMenuItemKind.XFRadio, values);
        Assert.Contains(XFDropdownMenuItemKind.Separator, values);
        Assert.Contains(XFDropdownMenuItemKind.Header, values);
        Assert.Equal(5, values.Length);
    }

    [Fact]
    public void Extended_XFSemanticColor_EnumValuesUsedByItems()
    {
        var items = new[]
        {
            new XFDropdownMenuItem { Id = "i1", Text = "Primary", Tone = XFSemanticColor.Primary },
            new XFDropdownMenuItem { Id = "i2", Text = "Secondary", Tone = XFSemanticColor.Secondary },
            new XFDropdownMenuItem { Id = "i3", Text = "Success", Tone = XFSemanticColor.Success },
            new XFDropdownMenuItem { Id = "i4", Text = "Warning", Tone = XFSemanticColor.Warning },
            new XFDropdownMenuItem { Id = "i5", Text = "Danger", Tone = XFSemanticColor.Danger },
            new XFDropdownMenuItem { Id = "i6", Text = "Info", Tone = XFSemanticColor.Info },
            new XFDropdownMenuItem { Id = "i7", Text = "Neutral", Tone = XFSemanticColor.Neutral },
            new XFDropdownMenuItem { Id = "i8", Text = "Default", Tone = XFSemanticColor.Default }
        };

        Assert.Equal(XFSemanticColor.Primary, items[0].Tone);
        Assert.Equal(XFSemanticColor.Secondary, items[1].Tone);
        Assert.Equal(XFSemanticColor.Success, items[2].Tone);
        Assert.Equal(XFSemanticColor.Warning, items[3].Tone);
        Assert.Equal(XFSemanticColor.Danger, items[4].Tone);
        Assert.Equal(XFSemanticColor.Info, items[5].Tone);
        Assert.Equal(XFSemanticColor.Neutral, items[6].Tone);
        Assert.Equal(XFSemanticColor.Default, items[7].Tone);
    }
}
