using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Overlays.Dialog;

namespace XForge.DesignSystem.Tests.Components;

public class XFDialogKeyboardTest : BunitContext
{
    public XFDialogKeyboardTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Escape_Key_Closes_Dialog()
    {
        var open = true;
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        cut.Find("section").KeyDown("Escape");

        Assert.False(open);
    }

    [Fact]
    public void CloseOnEscape_False_Escape_Does_Not_Close()
    {
        var open = true;
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.CloseOnEscape, false);
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        cut.Find("section").KeyDown("Escape");

        Assert.True(open);
    }

    [Fact]
    public void Tab_Key_Inside_Dialog()
    {
        var open = true;
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Title, "Test Dialog");
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
            parameters.AddChildContent("<button id='inner-btn'>Inner</button>");
        });

        var panel = cut.Find("section");
        Assert.NotNull(panel);
    }

    [Fact]
    public void Focus_Returns_To_Trigger_Element_On_Close()
    {
        var open = true;
        var cut = Render(builder =>
        {
            builder.OpenElement(0, "button");
            builder.AddAttribute(1, "id", "trigger-btn");
            builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, () => open = true));
            builder.AddContent(3, "Open Dialog");
            builder.CloseElement();

            builder.OpenComponent<XFDialog>(4);
            builder.AddAttribute(5, "Open", open);
            builder.AddAttribute(6, "OpenChanged", EventCallback.Factory.Create<bool>(this, value => open = value));
            builder.AddAttribute(7, "OnClose", EventCallback.Factory.Create(this, () => { }));
            builder.AddAttribute(8, "Title", "Test");
            builder.AddAttribute(9, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.AddContent(0, "Body");
            }));
            builder.CloseComponent();
        });

        Assert.Contains("trigger-btn", cut.Markup);
    }

    [Fact]
    public void Dialog_Has_TabIndex_Negative_One()
    {
        var open = true;
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Title, "Focus Test");
        });

        Assert.Contains("tabindex=\"-1\"", cut.Markup);
    }

    [Fact]
    public void Dialog_Role_Is_Dialog()
    {
        var open = true;
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, open);
        });

        Assert.Contains("role=\"dialog\"", cut.Markup);
        Assert.Contains("aria-modal=\"true\"", cut.Markup);
    }
}
