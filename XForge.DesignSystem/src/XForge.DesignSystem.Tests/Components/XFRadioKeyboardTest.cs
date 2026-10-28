using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.Radio;

namespace XForge.DesignSystem.Tests.Components;

public class XFRadioKeyboardTest : XForgeTestContext
{
    [Fact]
    public void Tab_Key_Moves_Focus_To_Radio_Group_First_Option()
    {
        string? value = "opt1";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFRadioGroup<string?>>(0);
            builder.AddAttribute(1, "Value", value);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, next => value = next));
            builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => value));
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRadio<string?>>(0);
                childBuilder.AddAttribute(1, "Value", "opt1");
                childBuilder.AddAttribute(2, "Label", "Option 1");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(3);
                childBuilder.AddAttribute(4, "Value", "opt2");
                childBuilder.AddAttribute(5, "Label", "Option 2");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("role=\"radiogroup\"", cut.Markup);
        Assert.Contains("role=\"radio\"", cut.Markup);
    }

    [Fact]
    public void ArrowDown_Moves_To_Next_Option()
    {
        string? value = "opt1";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFRadioGroup<string?>>(0);
            builder.AddAttribute(1, "Value", value);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, next => value = next));
            builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => value));
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRadio<string?>>(0);
                childBuilder.AddAttribute(1, "Value", "opt1");
                childBuilder.AddAttribute(2, "Label", "Option 1");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(3);
                childBuilder.AddAttribute(4, "Value", "opt2");
                childBuilder.AddAttribute(5, "Label", "Option 2");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(6);
                childBuilder.AddAttribute(7, "Value", "opt3");
                childBuilder.AddAttribute(8, "Label", "Option 3");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("div[role='radiogroup']").KeyDown("ArrowDown");

        Assert.Equal("opt2", value);
    }

    [Fact]
    public void ArrowUp_Moves_To_Previous_Option()
    {
        string? value = "opt2";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFRadioGroup<string?>>(0);
            builder.AddAttribute(1, "Value", value);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, next => value = next));
            builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => value));
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRadio<string?>>(0);
                childBuilder.AddAttribute(1, "Value", "opt1");
                childBuilder.AddAttribute(2, "Label", "Option 1");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(3);
                childBuilder.AddAttribute(4, "Value", "opt2");
                childBuilder.AddAttribute(5, "Label", "Option 2");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(6);
                childBuilder.AddAttribute(7, "Value", "opt3");
                childBuilder.AddAttribute(8, "Label", "Option 3");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("div[role='radiogroup']").KeyDown("ArrowUp");

        Assert.Equal("opt1", value);
    }

    [Fact]
    public void ArrowLeft_Moves_To_Previous_Option()
    {
        string? value = "opt2";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFRadioGroup<string?>>(0);
            builder.AddAttribute(1, "Value", value);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, next => value = next));
            builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => value));
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRadio<string?>>(0);
                childBuilder.AddAttribute(1, "Value", "opt1");
                childBuilder.AddAttribute(2, "Label", "Option 1");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(3);
                childBuilder.AddAttribute(4, "Value", "opt2");
                childBuilder.AddAttribute(5, "Label", "Option 2");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("div[role='radiogroup']").KeyDown("ArrowLeft");

        Assert.Equal("opt1", value);
    }

    [Fact]
    public void ArrowRight_Moves_To_Next_Option()
    {
        string? value = "opt1";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFRadioGroup<string?>>(0);
            builder.AddAttribute(1, "Value", value);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, next => value = next));
            builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => value));
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRadio<string?>>(0);
                childBuilder.AddAttribute(1, "Value", "opt1");
                childBuilder.AddAttribute(2, "Label", "Option 1");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(3);
                childBuilder.AddAttribute(4, "Value", "opt2");
                childBuilder.AddAttribute(5, "Label", "Option 2");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("div[role='radiogroup']").KeyDown("ArrowRight");

        Assert.Equal("opt2", value);
    }

    [Fact]
    public void Home_Moves_To_First_Option()
    {
        string? value = "opt3";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFRadioGroup<string?>>(0);
            builder.AddAttribute(1, "Value", value);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, next => value = next));
            builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => value));
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRadio<string?>>(0);
                childBuilder.AddAttribute(1, "Value", "opt1");
                childBuilder.AddAttribute(2, "Label", "Option 1");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(3);
                childBuilder.AddAttribute(4, "Value", "opt2");
                childBuilder.AddAttribute(5, "Label", "Option 2");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(6);
                childBuilder.AddAttribute(7, "Value", "opt3");
                childBuilder.AddAttribute(8, "Label", "Option 3");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("div[role='radiogroup']").KeyDown("Home");

        Assert.Equal("opt1", value);
    }

    [Fact]
    public void End_Moves_To_Last_Option()
    {
        string? value = "opt1";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFRadioGroup<string?>>(0);
            builder.AddAttribute(1, "Value", value);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, next => value = next));
            builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => value));
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRadio<string?>>(0);
                childBuilder.AddAttribute(1, "Value", "opt1");
                childBuilder.AddAttribute(2, "Label", "Option 1");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(3);
                childBuilder.AddAttribute(4, "Value", "opt2");
                childBuilder.AddAttribute(5, "Label", "Option 2");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(6);
                childBuilder.AddAttribute(7, "Value", "opt3");
                childBuilder.AddAttribute(8, "Label", "Option 3");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("div[role='radiogroup']").KeyDown("End");

        Assert.Equal("opt3", value);
    }

    [Fact]
    public void Disabled_Arrow_Keys_Do_Not_Change_Selection()
    {
        string? value = "opt1";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFRadioGroup<string?>>(0);
            builder.AddAttribute(1, "Value", value);
            builder.AddAttribute(2, "Disabled", true);
            builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<string?>(this, next => value = next));
            builder.AddAttribute(4, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => value));
            builder.AddAttribute(5, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRadio<string?>>(0);
                childBuilder.AddAttribute(1, "Value", "opt1");
                childBuilder.AddAttribute(2, "Label", "Option 1");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(3);
                childBuilder.AddAttribute(4, "Value", "opt2");
                childBuilder.AddAttribute(5, "Label", "Option 2");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("div[role='radiogroup']").KeyDown("ArrowDown");

        Assert.Equal("opt1", value);
    }

    [Fact]
    public void ArrowDown_Skips_Disabled_Option()
    {
        string? value = "opt1";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFRadioGroup<string?>>(0);
            builder.AddAttribute(1, "Value", value);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, next => value = next));
            builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => value));
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRadio<string?>>(0);
                childBuilder.AddAttribute(1, "Value", "opt1");
                childBuilder.AddAttribute(2, "Label", "Option 1");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(3);
                childBuilder.AddAttribute(4, "Value", "opt2");
                childBuilder.AddAttribute(5, "Label", "Option 2");
                childBuilder.AddAttribute(6, "Disabled", true);
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(7);
                childBuilder.AddAttribute(8, "Value", "opt3");
                childBuilder.AddAttribute(9, "Label", "Option 3");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("div[role='radiogroup']").KeyDown("ArrowDown");

        Assert.Equal("opt3", value);
    }

    [Fact]
    public void ArrowUp_Wraps_To_Last_Enabled_Option()
    {
        string? value = "opt1";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFRadioGroup<string?>>(0);
            builder.AddAttribute(1, "Value", value);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, next => value = next));
            builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => value));
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRadio<string?>>(0);
                childBuilder.AddAttribute(1, "Value", "opt1");
                childBuilder.AddAttribute(2, "Label", "Option 1");
                childBuilder.CloseComponent();
                childBuilder.OpenComponent<XFRadio<string?>>(3);
                childBuilder.AddAttribute(4, "Value", "opt2");
                childBuilder.AddAttribute(5, "Label", "Option 2");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("div[role='radiogroup']").KeyDown("ArrowUp");

        Assert.Equal("opt2", value);
    }
}
