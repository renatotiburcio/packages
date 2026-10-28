using Bunit;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Avatar;

namespace XForge.DesignSystem.Tests.Components;

public class XFAvatarComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_Name_GeneratesInitialsAndAriaLabel()
    {
        var cut = Render<XFAvatar>(parameters => parameters.Add(p => p.Name, "Ana Souza"));

        var avatar = cut.Find(".xf-avatar");
        Assert.Equal("img", avatar.GetAttribute("role"));
        Assert.Equal("Ana Souza", avatar.GetAttribute("aria-label"));
        Assert.Contains("AS", cut.Markup);
    }

    [Fact]
    public void Render_Image_UsesAltAndSource()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "Ana Souza");
            parameters.Add(p => p.ImageSrc, "/avatar.png");
            parameters.Add(p => p.Alt, "Foto de Ana");
        });

        var image = cut.Find("img");
        Assert.Equal("/avatar.png", image.GetAttribute("src"));
        Assert.Equal("Foto de Ana", image.GetAttribute("alt"));
    }

    [Fact]
    public void Render_Status_AddsStatusBadge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "Ana Souza");
            parameters.Add(p => p.Status, XFAvatarStatus.Online);
        });

        Assert.Contains("bg-success", cut.Markup);
        Assert.Contains("Disponivel", cut.Markup);
    }

    [Fact]
    public void Render_ImageWithoutName_UsesPortugueseDefaultAltText()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.ImageSrc, "/avatar.png");
        });

        Assert.Equal("Avatar", cut.Find("img").GetAttribute("alt"));
    }

    [Fact]
    public void Render_Status_WithLocalizedLabels_UsesConfiguredStatusText()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "Ana Souza");
            parameters.Add(p => p.Status, XFAvatarStatus.Offline);
            parameters.Add(p => p.OfflineStatusLabel, "Fora de servico");
        });

        Assert.Contains("aria-label=\"Fora de servico\"", cut.Markup);
    }

    [Fact]
    public void Render_Gravatar_GeneratesSecureUrl()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "User");
            parameters.Add(p => p.Email, "USER@example.com");
            parameters.Add(p => p.UseGravatar, true);
        });

        var image = cut.Find("img");
        Assert.StartsWith("https://secure.gravatar.com/avatar/", image.GetAttribute("src"));
        Assert.Contains("?d=retro&s=160", image.GetAttribute("src"));
    }

    [Fact]
    public void Render_Group_ShowsOverflow()
    {
        var cut = Render<XFAvatarGroup>(parameters =>
        {
            parameters.Add(p => p.OverflowCount, 3);
            parameters.AddChildContent("<span>users</span>");
        });

        Assert.Equal("group", cut.Find(".xf-avatar-group").GetAttribute("role"));
        Assert.Contains("+3", cut.Markup);
        Assert.Contains("aria-label=\"3 usuarios adicionais\"", cut.Markup);
    }

    [Fact]
    public void Render_GroupOverflow_WithLocalizedFormats_UsesCountLabel()
    {
        var cut = Render<XFAvatarGroup>(parameters =>
        {
            parameters.Add(p => p.OverflowCount, 1);
            parameters.Add(p => p.OverflowSingleLabelFormat, "{0} pessoa fora da lista");
            parameters.Add(p => p.OverflowMultipleLabelFormat, "{0} pessoas fora da lista");
        });

        Assert.Contains("+1", cut.Markup);
        Assert.Contains("aria-label=\"1 pessoa fora da lista\"", cut.Markup);
    }
}
