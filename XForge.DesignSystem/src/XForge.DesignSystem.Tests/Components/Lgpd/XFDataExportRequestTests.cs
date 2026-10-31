using Bunit;
using Xunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Lgpd;

namespace XForge.DesignSystem.Tests.Components.Lgpd;

public class XFDataExportRequestTests : XForgeTestContext
{
    [Fact]
    public void Render_ShowsExportForm()
    {
        var cut = Render<XFDataExportRequest>();
        Assert.Contains("xf-lgpd-export", cut.Markup);
        Assert.Contains("Exportar meus dados", cut.Markup);
    }

    [Fact]
    public void Render_WithUserId_ShowsUserId()
    {
        var cut = Render<XFDataExportRequest>(p => p.Add(x => x.UserId, "user456"));
        Assert.Contains("user456", cut.Markup);
    }

    [Fact]
    public void Render_DefaultStatus_ShowsIdle()
    {
        var cut = Render<XFDataExportRequest>();
        Assert.DoesNotContain("Processando", cut.Markup);
        Assert.DoesNotContain("Download", cut.Markup);
    }

    [Fact]
    public void Render_ProcessingStatus_ShowsProcessing()
    {
        var cut = Render<XFDataExportRequest>(p => p.Add(x => x.Status, "processing"));
        Assert.Contains("Processando", cut.Markup);
    }

    [Fact]
    public void Render_ReadyStatus_ShowsDownloadLink()
    {
        var cut = Render<XFDataExportRequest>(p =>
        {
            p.Add(x => x.Status, "ready");
            p.Add(x => x.DownloadUrl, "/downloads/export.json");
        });
        Assert.Contains("/downloads/export.json", cut.Markup);
        Assert.Contains("Download", cut.Markup);
    }

    [Fact]
    public async Task ClickRequest_InvokesExportRequested()
    {
        bool exportRequested = false;
        var cut = Render<XFDataExportRequest>(p =>
            p.Add(x => x.OnExportRequested, EventCallback.Factory.Create(this, () => exportRequested = true)));

        await cut.Find("button").ClickAsync();

        Assert.True(exportRequested);
    }

    [Fact]
    public void Render_ShowsFormatOptions()
    {
        var cut = Render<XFDataExportRequest>();
        Assert.Contains("json", cut.Markup);
        Assert.Contains("csv", cut.Markup);
        Assert.Contains("pdf", cut.Markup);
    }

    [Fact]
    public void Render_ShowsLGPDReference()
    {
        var cut = Render<XFDataExportRequest>();
        Assert.Contains("LGPD art. 18", cut.Markup);
    }
}
