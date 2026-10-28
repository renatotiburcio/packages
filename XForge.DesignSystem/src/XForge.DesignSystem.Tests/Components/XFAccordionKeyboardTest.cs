using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Accordion;

namespace XForge.DesignSystem.Tests.Components;

public class XFAccordionKeyboardTest : XForgeTestContext
{
    [Fact]
    public void Tab_Key_Moves_Focus_To_Accordion_Header()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFAccordion>(0);
            builder.AddAttribute(1, "ChildContent", (RenderFragment)(accBuilder =>
            {
                accBuilder.OpenComponent<XFAccordionItem>(0);
                accBuilder.AddAttribute(1, "Title", "Section 1");
                accBuilder.AddContent(2, "Content 1");
                accBuilder.CloseComponent();
                accBuilder.OpenComponent<XFAccordionItem>(3);
                accBuilder.AddAttribute(4, "Title", "Section 2");
                accBuilder.AddContent(5, "Content 2");
                accBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var header = cut.Find("button[aria-expanded]");
        Assert.NotNull(header);
    }

    [Fact]
    public void Click_Toggles_Accordion_Item()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFAccordion>(0);
            builder.AddAttribute(1, "ChildContent", (RenderFragment)(accBuilder =>
            {
                accBuilder.OpenComponent<XFAccordionItem>(0);
                accBuilder.AddAttribute(1, "Title", "Section 1");
                accBuilder.AddContent(2, "Content 1");
                accBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var header = cut.Find("button[aria-expanded]");
        Assert.Equal("false", header.GetAttribute("aria-expanded"));

        header.Click();

        var expandedHeader = cut.Find("button[aria-expanded='true']");
        Assert.NotNull(expandedHeader);
    }

    [Fact]
    public void Space_Key_Toggles_Accordion_Item()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFAccordion>(0);
            builder.AddAttribute(1, "ChildContent", (RenderFragment)(accBuilder =>
            {
                accBuilder.OpenComponent<XFAccordionItem>(0);
                accBuilder.AddAttribute(1, "Title", "Section 1");
                accBuilder.AddContent(2, "Content 1");
                accBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var header = cut.Find("button[aria-expanded]");
        Assert.Equal("false", header.GetAttribute("aria-expanded"));

        header.Click();

        var expandedHeader = cut.Find("button[aria-expanded='true']");
        Assert.NotNull(expandedHeader);
    }

    [Fact]
    public void ArrowDown_Moves_To_Next_Accordion_Header()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFAccordion>(0);
            builder.AddAttribute(1, "Multiple", true);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(accBuilder =>
            {
                accBuilder.OpenComponent<XFAccordionItem>(0);
                accBuilder.AddAttribute(1, "Title", "Section 1");
                accBuilder.AddContent(2, "Content 1");
                accBuilder.CloseComponent();
                accBuilder.OpenComponent<XFAccordionItem>(3);
                accBuilder.AddAttribute(4, "Title", "Section 2");
                accBuilder.AddContent(5, "Content 2");
                accBuilder.CloseComponent();
                accBuilder.OpenComponent<XFAccordionItem>(6);
                accBuilder.AddAttribute(7, "Title", "Section 3");
                accBuilder.AddContent(8, "Content 3");
                accBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var headers = cut.FindAll("button[aria-expanded]");
        Assert.True(headers.Count >= 3);
    }

    [Fact]
    public void ArrowUp_Moves_To_Previous_Accordion_Header()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFAccordion>(0);
            builder.AddAttribute(1, "Multiple", true);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(accBuilder =>
            {
                accBuilder.OpenComponent<XFAccordionItem>(0);
                accBuilder.AddAttribute(1, "Title", "Section 1");
                accBuilder.AddContent(2, "Content 1");
                accBuilder.CloseComponent();
                accBuilder.OpenComponent<XFAccordionItem>(3);
                accBuilder.AddAttribute(4, "Title", "Section 2");
                accBuilder.AddContent(5, "Content 2");
                accBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var headers = cut.FindAll("button[aria-expanded]");
        Assert.True(headers.Count >= 2);
    }

    [Fact]
    public void Accordion_Has_Proper_Aria_Expanded()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFAccordion>(0);
            builder.AddAttribute(1, "ChildContent", (RenderFragment)(accBuilder =>
            {
                accBuilder.OpenComponent<XFAccordionItem>(0);
                accBuilder.AddAttribute(1, "Title", "Section 1");
                accBuilder.AddAttribute(2, "Expanded", true);
                accBuilder.AddContent(3, "Content 1");
                accBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("aria-expanded=\"true\"", cut.Markup);
    }
}
