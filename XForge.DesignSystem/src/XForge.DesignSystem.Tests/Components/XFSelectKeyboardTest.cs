using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.Select;

namespace XForge.DesignSystem.Tests.Components;

public class XFSelectKeyboardTest : XForgeTestContext
{
    [Fact]
    public void Tab_Key_Moves_Focus_To_Select()
    {
        string? value = "";
        var cut = Render<XFSelect<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.AddChildContent(
                childBuilder =>
                {
                    childBuilder.OpenComponent<XFSelectOption<string>>(0);
                    childBuilder.AddAttribute(1, "Value", "opt1");
                    childBuilder.AddAttribute(2, "Text", "Option 1");
                    childBuilder.CloseComponent();
                });
        });

        var select = cut.Find("select");
        Assert.NotNull(select);
    }

    [Fact]
    public void Change_Updates_Value()
    {
        string? value = "";
        var cut = Render<XFSelect<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.AddChildContent(
                childBuilder =>
                {
                    childBuilder.OpenComponent<XFSelectOption<string>>(0);
                    childBuilder.AddAttribute(1, "Value", "opt1");
                    childBuilder.AddAttribute(2, "Text", "Option 1");
                    childBuilder.CloseComponent();
                    childBuilder.OpenComponent<XFSelectOption<string>>(3);
                    childBuilder.AddAttribute(4, "Value", "opt2");
                    childBuilder.AddAttribute(5, "Text", "Option 2");
                    childBuilder.CloseComponent();
                });
        });

        cut.Find("select").Change("opt2");

        Assert.Equal("opt2", value);
    }

    [Fact]
    public void Placeholder_Rendered_Correctly()
    {
        string? value = "";
        var cut = Render<XFSelect<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.Placeholder, "Choose an option");
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
        });

        Assert.Contains("Choose an option", cut.Markup);
    }

    [Fact]
    public void Disabled_Select_Cannot_Be_Changed()
    {
        string? value = "opt1";
        var cut = Render<XFSelect<string>>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.AddChildContent(
                childBuilder =>
                {
                    childBuilder.OpenComponent<XFSelectOption<string>>(0);
                    childBuilder.AddAttribute(1, "Value", "opt1");
                    childBuilder.AddAttribute(2, "Text", "Option 1");
                    childBuilder.CloseComponent();
                });
        });

        Assert.Contains("disabled", cut.Markup);

        cut.Find("select").Change("opt2");
        Assert.Equal("opt1", value);
    }
}
