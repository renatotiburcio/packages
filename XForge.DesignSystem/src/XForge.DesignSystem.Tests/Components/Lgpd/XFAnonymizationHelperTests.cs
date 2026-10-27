using Bunit;
using Xunit;
using XForge.Components.Lgpd;

namespace XForge.DesignSystem.Tests.Components.Lgpd;

public class XFAnonymizationHelperTests : BunitContext
{
    [Fact]
    public void Render_ShowsAnonymizationForm()
    {
        var cut = Render<XFAnonymizationHelper>();
        Assert.Contains("xf-lgpd-anon", cut.Markup);
        Assert.Contains("Anonimizar", cut.Markup);
    }

    [Fact]
    public void Render_ShowsModeSelector()
    {
        var cut = Render<XFAnonymizationHelper>();
        Assert.Contains("mask", cut.Markup);
        Assert.Contains("hash", cut.Markup);
        Assert.Contains("redact", cut.Markup);
    }

    [Fact]
    public async Task ClickAnonymize_WithMaskMode_MasksDigits()
    {
        var cut = Render<XFAnonymizationHelper>(p => p.Add(x => x.Input, "12345678901"));
        await cut.Find("button").ClickAsync();

        var output = cut.Find(".xf-lgpd-output");
        Assert.Contains("*", output.TextContent);
        Assert.DoesNotContain("12345678901", output.TextContent);
    }

    [Fact]
    public async Task ClickAnonymize_WithRedactMode_RedactsDigits()
    {
        var cut = Render<XFAnonymizationHelper>(p =>
        {
            p.Add(x => x.Input, "12345678901");
            p.Add(x => x.Mode, "redact");
        });
        await cut.Find("button").ClickAsync();

        var output = cut.Find(".xf-lgpd-output");
        Assert.Contains("[REDACTED]", output.TextContent);
    }

    [Fact]
    public async Task ClickAnonymize_WithHashMode_HashesDigits()
    {
        var cut = Render<XFAnonymizationHelper>(p =>
        {
            p.Add(x => x.Input, "12345678901");
            p.Add(x => x.Mode, "hash");
        });
        await cut.Find("button").ClickAsync();

        var output = cut.Find(".xf-lgpd-output");
        // Hash should be 8 hex characters (upper or lower case)
        Assert.Matches("[0-9a-fA-F]{8}", output.TextContent);
    }

    [Fact]
    public async Task ChangeMode_UpdatesOutput()
    {
        var cut = Render<XFAnonymizationHelper>(p => p.Add(x => x.Input, "123"));

        // First with mask
        await cut.Find("button").ClickAsync();
        var output1 = cut.Find(".xf-lgpd-output");
        Assert.Contains("*", output1.TextContent);

        // Change to redact
        cut.Find("select").Change("redact");
        await cut.Find("button").ClickAsync();
        var output2 = cut.Find(".xf-lgpd-output");
        Assert.Contains("[REDACTED]", output2.TextContent);
    }
}
