using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Forms.PasswordStrength;

namespace XForge.DesignSystem.Tests.Components.Forms.PasswordStrength;

public class XFPasswordStrengthExtendedTest : BunitContext
{
    private IRenderedComponent<XFPasswordStrength> RenderPassword(
        Action<ComponentParameterCollectionBuilder<XFPasswordStrength>>? configure = null)
    {
        return Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "test");
            configure?.Invoke(parameters);
        });
    }

    [Fact]
    public void HelperText_RendersHelperDiv()
    {
        var cut = RenderPassword(p => p.Add(x => x.HelperText, "Use a strong password"));
        Assert.Contains("Use a strong password", cut.Markup);
        Assert.Contains("aria-describedby", cut.Markup);
    }

    [Fact]
    public void ChangeEvent_InvokedOnBlur()
    {
        string? changeValue = null;
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "");
            parameters.Add(p => p.Change, EventCallback.Factory.Create<string?>(this, v => changeValue = v));
        });

        cut.Find("input").InputAsync(new ChangeEventArgs { Value = "newpass123!" });
        cut.Find("input").ChangeAsync(new ChangeEventArgs { Value = "newpass123!" });

        Assert.Equal("newpass123!", changeValue);
    }

    [Fact]
    public void EmptyPassword_ShowsNoneLevel()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
            parameters.Add(p => p.Value, ""));

        Assert.Contains("Nao informada", cut.Markup);
    }

    [Fact]
    public void WeakPassword_ShowsWeakState()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
            parameters.Add(p => p.Value, "abc"));

        Assert.Contains("Fraca", cut.Markup);
        Assert.Contains("text-danger", cut.Markup);
        Assert.Contains("bg-danger", cut.Markup);
    }

    [Fact]
    public void FairPassword_ShowsFairState()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
            parameters.Add(p => p.Value, "Abcdefgh12"));

        Assert.Contains("Media", cut.Markup);
        Assert.Contains("text-warning", cut.Markup);
        Assert.Contains("bg-warning", cut.Markup);
    }

    [Fact]
    public void GoodPassword_ShowsGoodState()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
            parameters.Add(p => p.Value, "Abcdefgh1234"));

        Assert.Contains("Boa", cut.Markup);
        Assert.Contains("text-primary", cut.Markup);
        Assert.Contains("bg-primary", cut.Markup);
    }

    [Fact]
    public void StrongPassword_ShowsStrongState()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
            parameters.Add(p => p.Value, "MinhaSenha@2026"));

        Assert.Contains("Forte", cut.Markup);
        Assert.Contains("text-success", cut.Markup);
        Assert.Contains("bg-success", cut.Markup);
    }

    [Fact]
    public void BlockedPassword_ShowsRuleViolation()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
            parameters.Add(p => p.Value, "password"));

        Assert.Contains("Nao usar senha comum", cut.Markup);
    }

    [Fact]
    public void CustomBlockedPasswords_RespectsList()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "mysecret");
            parameters.Add(p => p.BlockedPasswords, new[] { "mysecret", "blocked123" });
        });

        Assert.Contains("Nao usar senha comum", cut.Markup);
    }

    [Fact]
    public void NotCommonPassword_Disabled_SkipsRule()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "password");
            parameters.Add(p => p.RequireNotCommonPassword, false);
        });

        Assert.DoesNotContain("Nao usar senha comum", cut.Markup);
    }

    [Fact]
    public void Uppercase_Disabled_SkipsRule()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "lowercase123!");
            parameters.Add(p => p.RequireUppercase, false);
        });

        Assert.DoesNotContain("Uma letra maiuscula", cut.Markup);
    }

    [Fact]
    public void Lowercase_Disabled_SkipsRule()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "UPPERCASE123!");
            parameters.Add(p => p.RequireLowercase, false);
        });

        Assert.DoesNotContain("Uma letra minuscula", cut.Markup);
    }

    [Fact]
    public void Number_Disabled_SkipsRule()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "NoNumbers!");
            parameters.Add(p => p.RequireNumber, false);
        });

        Assert.DoesNotContain("Um numero", cut.Markup);
    }

    [Fact]
    public void SpecialCharacter_Disabled_SkipsRule()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "NoSpecial123");
            parameters.Add(p => p.RequireSpecialCharacter, false);
        });

        Assert.DoesNotContain("Um caractere especial", cut.Markup);
    }

    [Fact]
    public void MinimumLength_CustomValue_UpdatesRule()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "ab");
            parameters.Add(p => p.MinimumLength, 4);
        });

        Assert.Contains("Pelo menos 4 caracteres", cut.Markup);
    }

    [Fact]
    public void FullWidth_False_RendersInlineFlex()
    {
        var cut = RenderPassword(p => p.Add(x => x.FullWidth, false));
        Assert.Contains("inline-flex", cut.Markup);
    }

    [Fact]
    public void ShowPasswordToggle_False_HidesToggle()
    {
        var cut = RenderPassword(p => p.Add(x => x.ShowPasswordToggle, false));
        Assert.DoesNotContain("Mostrar senha", cut.Markup);
    }

    [Fact]
    public void Required_True_ShowsAriaRequired()
    {
        var cut = RenderPassword(p => p.Add(x => x.Required, true));
        Assert.Equal("true", cut.Find("input").GetAttribute("aria-required"));
    }

    [Fact]
    public void ReadOnly_True_SetsReadonlyAttribute()
    {
        var cut = RenderPassword(p => p.Add(x => x.ReadOnly, true));
        Assert.Equal("true", cut.Find("input").GetAttribute("aria-readonly"));
    }

    [Fact]
    public void AriaLabel_SetsInputAriaLabel()
    {
        var cut = RenderPassword(p => p.Add(x => x.AriaLabel, "Password field"));
        Assert.Equal("Password field", cut.Find("input").GetAttribute("aria-label"));
    }

    [Fact]
    public void Name_SetsInputName()
    {
        var cut = RenderPassword(p => p.Add(x => x.Name, "password-field"));
        Assert.Equal("password-field", cut.Find("input").GetAttribute("name"));
    }

    [Fact]
    public void Placeholder_CustomValue_SetsPlaceholder()
    {
        var cut = RenderPassword(p => p.Add(x => x.Placeholder, "Enter password"));
        Assert.Equal("Enter password", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void AutoComplete_CustomValue_SetsAttribute()
    {
        var cut = RenderPassword(p => p.Add(x => x.AutoComplete, "off"));
        Assert.Equal("off", cut.Find("input").GetAttribute("autocomplete"));
    }

    [Fact]
    public void Score_UpdatesStrengthChangedEvent()
    {
        XFPasswordStrengthChangedEventArgs? args = null;
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "");
            parameters.Add(p => p.StrengthChanged, EventCallback.Factory.Create<XFPasswordStrengthChangedEventArgs>(this, a => args = a));
        });

        cut.Find("input").InputAsync(new ChangeEventArgs { Value = "MinhaSenha@2026" });

        Assert.NotNull(args);
        Assert.Equal(XFPasswordStrengthLevel.Strong, args!.Level);
        Assert.True(args.IsValid);
    }

    [Fact]
    public void OnFocus_TriggersEvent()
    {
        FocusEventArgs? args = null;
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "test");
            parameters.Add(p => p.OnFocus, EventCallback.Factory.Create<FocusEventArgs>(this, a => args = a));
        });

        cut.Find("input").FocusAsync(new FocusEventArgs());
        Assert.NotNull(args);
    }

    [Fact]
    public void OnBlur_TriggersEvent()
    {
        FocusEventArgs? args = null;
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "test");
            parameters.Add(p => p.OnBlur, EventCallback.Factory.Create<FocusEventArgs>(this, a => args = a));
        });

        cut.Find("input").BlurAsync(new FocusEventArgs());
        Assert.NotNull(args);
    }

    [Fact]
    public void Label_WithRequired_ShowsAsterisk()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Label, "Password");
            parameters.Add(p => p.Required, true);
        });

        Assert.Contains("*", cut.Markup);
    }

    [Fact]
    public void Immediate_False_DoesNotCommitOnInput()
    {
        string? value = null;
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "");
            parameters.Add(p => p.Immediate, false);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
        });

        cut.Find("input").InputAsync(new ChangeEventArgs { Value = "test" });

        Assert.Null(value);
    }

    [Fact]
    public void Immediate_True_CommitsOnInput()
    {
        string? value = null;
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "");
            parameters.Add(p => p.Immediate, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
        });

        cut.Find("input").InputAsync(new ChangeEventArgs { Value = "test" });

        Assert.Equal("test", value);
    }

    [Fact]
    public void RuleStates_SatisfiedRule_ShowsSuccessColor()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
            parameters.Add(p => p.Value, "MinhaSenha@2026"));

        Assert.Contains("text-success", cut.Markup);
    }

    [Fact]
    public void RuleStates_UnsatisfiedRule_ShowsSecondaryColor()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
            parameters.Add(p => p.Value, "a"));

        Assert.Contains("text-text-secondary", cut.Markup);
    }

    [Fact]
    public void Disabled_ShowsDisabledFieldClass()
    {
        var cut = Render<XFPasswordStrength>(parameters =>
        {
            parameters.Add(p => p.Value, "test");
            parameters.Add(p => p.Disabled, true);
        });

        Assert.Contains("cursor-not-allowed", cut.Markup);
    }
}
