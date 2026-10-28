using Bunit;
using Xunit;
using Microsoft.AspNetCore.Components;
using XForge.Components.Lgpd;

namespace XForge.DesignSystem.Tests.Components.Lgpd;

public class XFDataDeleteRequestTests : XForgeTestContext
{
    [Fact]
    public void Render_ShowsDeleteForm()
    {
        var cut = Render<XFDataDeleteRequest>();
        Assert.Contains("xf-lgpd-delete", cut.Markup);
        Assert.Contains("Excluir meus dados", cut.Markup);
    }

    [Fact]
    public void Render_WithUserId_ShowsUserId()
    {
        var cut = Render<XFDataDeleteRequest>(p => p.Add(x => x.UserId, "user123"));
        Assert.Contains("user123", cut.Markup);
    }

    [Fact]
    public void Render_DeleteButton_InitiallyDisabled()
    {
        var cut = Render<XFDataDeleteRequest>();
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void Render_ShowsWarningMessage()
    {
        var cut = Render<XFDataDeleteRequest>();
        Assert.Contains("irreversivel", cut.Markup);
        Assert.Contains("anonimizados", cut.Markup);
    }

    [Fact]
    public void Render_ShowsCheckboxes()
    {
        var cut = Render<XFDataDeleteRequest>();
        var checkboxes = cut.FindAll("input[type='checkbox']");
        Assert.Equal(2, checkboxes.Count);
    }

    [Fact]
    public void Render_ShowsPasswordField()
    {
        var cut = Render<XFDataDeleteRequest>();
        var passwordField = cut.Find("input[type='password']");
        Assert.NotNull(passwordField);
    }

    [Fact]
    public void Render_ShowsDeleteButton()
    {
        var cut = Render<XFDataDeleteRequest>();
        var button = cut.Find("button");
        Assert.Contains("Excluir conta", button.TextContent);
    }

    [Fact]
    public void Render_ShowsCheckboxLabels()
    {
        var cut = Render<XFDataDeleteRequest>();
        Assert.Contains("Entendo que esta acao nao pode ser desfeita", cut.Markup);
        Assert.Contains("Confirmo que sou o titular dos dados", cut.Markup);
    }
}
