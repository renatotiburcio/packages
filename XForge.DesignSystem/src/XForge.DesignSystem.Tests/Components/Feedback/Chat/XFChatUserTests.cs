using XForge.DesignSystem.Components.Feedback.Chat;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Feedback.Chat;

public class XFChatUserTests
{
    [Fact]
    public void Constructor_SetsDefaults()
    {
        var user = new XFChatUser();
        Assert.Equal(string.Empty, user.Name);
        Assert.True(user.IsOnline);
        Assert.Equal("?", user.Initials);
    }

    [Fact]
    public void Id_IsRandomGuid()
    {
        var u1 = new XFChatUser();
        var u2 = new XFChatUser();
        Assert.NotEqual(u1.Id, u2.Id);
    }

    [Fact]
    public void Initials_SingleName_ReturnsFirstTwoChars()
    {
        var user = new XFChatUser { Name = "John" };
        Assert.Equal("JO", user.Initials);
    }

    [Fact]
    public void Initials_FirstAndLastName_ReturnsFirstCharEach()
    {
        var user = new XFChatUser { Name = "John Doe" };
        Assert.Equal("JD", user.Initials);
    }

    [Fact]
    public void Initials_ThreeNames_ReturnsFirstAndLast()
    {
        var user = new XFChatUser { Name = "John Michael Doe" };
        Assert.Equal("JM", user.Initials);
    }

    [Fact]
    public void Initials_EmptyName_ReturnsQuestionMark()
    {
        var user = new XFChatUser { Name = "" };
        Assert.Equal("?", user.Initials);
    }

    [Fact]
    public void Initials_WhitespaceName_ReturnsQuestionMark()
    {
        var user = new XFChatUser { Name = "   " };
        Assert.Equal("?", user.Initials);
    }

    [Fact]
    public void Initials_SingleCharName_ReturnsUppercase()
    {
        var user = new XFChatUser { Name = "j" };
        Assert.Equal("J", user.Initials);
    }

    [Fact]
    public void Initials_CaseInsensitive_UppercasesResult()
    {
        var user = new XFChatUser { Name = "john doe" };
        Assert.Equal("JD", user.Initials);
    }

    [Fact]
    public void Initials_TwoCharName_ReturnsBothUppercase()
    {
        var user = new XFChatUser { Name = "jo" };
        Assert.Equal("JO", user.Initials);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var user = new XFChatUser
        {
            Id = "custom-id",
            Name = "Test User",
            AvatarUrl = "https://example.com/avatar.png",
            IsOnline = false
        };
        Assert.Equal("custom-id", user.Id);
        Assert.Equal("Test User", user.Name);
        Assert.Equal("https://example.com/avatar.png", user.AvatarUrl);
        Assert.False(user.IsOnline);
        Assert.Equal("TU", user.Initials);
    }
}
