using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Forms.Login;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFLoginComponentTest : XForgeTestContext
{
    public XFLoginComponentTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_Default_RendersLoginForm()
    {
        var cut = Render<XFLogin>();

        Assert.Contains("xf-login", cut.Markup);
        Assert.Contains("<form", cut.Markup);
    }

    [Fact]
    public void Render_Default_ShowsUsernameInput()
    {
        var cut = Render<XFLogin>();

        Assert.Contains("type=\"text\"", cut.Markup);
        Assert.Contains("autocomplete=\"username\"", cut.Markup);
        Assert.Contains("Usuário", cut.Markup);
    }

    [Fact]
    public void Render_Default_ShowsPasswordInput()
    {
        var cut = Render<XFLogin>();

        Assert.Contains("type=\"password\"", cut.Markup);
        Assert.Contains("autocomplete=\"current-password\"", cut.Markup);
        Assert.Contains("Senha", cut.Markup);
    }

    [Fact]
    public void Render_Default_ShowsLoginButton()
    {
        var cut = Render<XFLogin>();

        Assert.Contains("type=\"submit\"", cut.Markup);
        Assert.Contains("Entrar", cut.Markup);
    }

    [Fact]
    public void Render_Default_ShowsRememberMe()
    {
        var cut = Render<XFLogin>();

        Assert.Contains("type=\"checkbox\"", cut.Markup);
        Assert.Contains("Lembrar de mim", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomLoginText_ShowsCustomText()
    {
        var cut = Render<XFLogin>(parameters =>
            parameters.Add(p => p.LoginText, "Sign In"));

        Assert.Contains("Sign In", cut.Markup);
    }

    [Fact]
    public void Render_ShowLoginFalse_HidesLoginButton()
    {
        var cut = Render<XFLogin>(parameters =>
            parameters.Add(p => p.ShowLoginButton, false));

        Assert.DoesNotContain("type=\"submit\"", cut.Markup);
    }

    [Fact]
    public void Render_AllowRememberMeFalse_HidesCheckbox()
    {
        var cut = Render<XFLogin>(parameters =>
            parameters.Add(p => p.AllowRememberMe, false));

        Assert.DoesNotContain("Lembrar de mim", cut.Markup);
        Assert.DoesNotContain("type=\"checkbox\"", cut.Markup);
    }

    [Fact]
    public void Render_ShowRegister_ShowsRegisterButton()
    {
        var cut = Render<XFLogin>(parameters =>
            parameters.Add(p => p.ShowRegister, true));

        Assert.Contains("Criar conta", cut.Markup);
    }

    [Fact]
    public void Render_ShowResetPassword_ShowsResetButton()
    {
        var cut = Render<XFLogin>(parameters =>
            parameters.Add(p => p.ShowResetPassword, true));

        Assert.Contains("Esqueceu a senha?", cut.Markup);
    }

    [Fact]
    public void Render_Disabled_DisablesAllInputs()
    {
        var cut = Render<XFLogin>(parameters =>
            parameters.Add(p => p.Disabled, true));

        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaLabel_SetsFormAriaLabel()
    {
        var cut = Render<XFLogin>(parameters =>
            parameters.Add(p => p.AriaLabel, "Login form"));

        Assert.Contains("aria-label=\"Login form\"", cut.Markup);
    }

    [Fact]
    public void Render_ShowsUsernameLabel()
    {
        var cut = Render<XFLogin>();

        Assert.Contains("label", cut.Markup);
    }

    [Fact]
    public void Render_InputFields_HaveCorrectAutocomplete()
    {
        var cut = Render<XFLogin>();

        Assert.Contains("autocomplete=\"username\"", cut.Markup);
        Assert.Contains("autocomplete=\"current-password\"", cut.Markup);
    }
}
