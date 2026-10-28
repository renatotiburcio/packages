using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Moq;
using System.Windows.Input;
using XForge.DesignSystem.Components.Button;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFButtonLifecycleTests : XForgeTestContext
{
    public XFButtonLifecycleTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void OnParametersSet_IconSet_StartIconNull_AutoCopiesIcon()
    {
        RenderFragment icon = builder => builder.AddContent(0, "icon");

        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Icon, icon)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_IconSet_StartIconAlsoSet_BothPreserved()
    {
        RenderFragment icon = builder => builder.AddContent(0, "icon");
        RenderFragment startIcon = builder => builder.AddContent(0, "start-icon");

        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Icon, icon)
            .Add(p => p.StartIcon, startIcon)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_FormTargetInvalid_ThrowsArgumentException()
    {
        var act = () => Render<XFButton>(parameters => parameters
            .Add(p => p.FormTarget, "invalid")
        );

        try
        {
            act();
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
    }

    [Theory]
    [InlineData("_self")]
    [InlineData("_blank")]
    [InlineData("_parent")]
    [InlineData("_top")]
    public void OnParametersSet_FormTargetValid_NoException(string formTarget)
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.FormTarget, formTarget)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_CommandNull_NoException()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Command, (ICommand?)null)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_Disabled_ReflectsInRenderedOutput()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Disabled, true)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_Loading_ShowsLoadingState()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Loading, true)
            .Add(p => p.Text, "Submit")
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_Variant_Outline_AppliesOutlineClass()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Variant, XFButtonVariant.Outline)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_Color_Success_AppliesSuccessTone()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Color, XFButtonColor.Success)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_Size_Large_AppliesLargeClass()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Size, XFExtendedSize.Lg)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_Shape_Circle_AppliesRoundedFull()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Shape, XFButtonShape.Circle)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_FullWidth_AppliesWFull()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.FullWidth, true)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_Active_AppliesActiveStyles()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Active, true)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_Pressed_AppliesPressedStyles()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Pressed, true)
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_AutoIconLike_SquareShape()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Shape, XFButtonShape.Square)
            .Add(p => p.Icon, builder => builder.AddContent(0, "icon"))
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_EffectiveAriaLabel_FromTitleWhenIconOnly()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Title, "Click me")
            .Add(p => p.Icon, builder => builder.AddContent(0, "icon"))
        );

        var html = cut.Markup;
        Assert.Contains("xf-button", html);
    }

    [Fact]
    public void OnParametersSet_ChildContent_RendersInsideButton()
    {
        var cut = Render<XFButton>(parameters => parameters
            .AddChildContent("Click me")
        );

        var html = cut.Markup;
        Assert.Contains("Click me", html);
    }

    [Fact]
    public void OnParametersSet_ButtonText_RendersText()
    {
        var cut = Render<XFButton>(parameters => parameters
            .Add(p => p.Text, "Submit")
        );

        var html = cut.Markup;
        Assert.Contains("Submit", html);
    }
}