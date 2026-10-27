using System;
using XForge.DesignSystem.Core.Primitives;
using XForge.DesignSystem.Services.AI;
using Xunit;
namespace XForge.DesignSystem.Tests.Services.AI;

public class XFAiDomainModelTests
{
    [Fact] public void AIChatServiceOptions_Defaults_Model() { var o = new AIChatServiceOptions(); Assert.Equal("gpt-4o-mini", o.Model); }
    [Fact] public void AIChatServiceOptions_Defaults_Endpoint() { var o = new AIChatServiceOptions(); Assert.Equal("https://api.openai.com/v1/chat/completions", o.Endpoint); }
    [Fact] public void AIChatServiceOptions_Defaults_ApiKeyHeader() { var o = new AIChatServiceOptions(); Assert.Equal("Authorization", o.ApiKeyHeader); }
    [Fact] public void AIChatServiceOptions_Defaults_ApiKeyScheme() { var o = new AIChatServiceOptions(); Assert.Equal("Bearer", o.ApiKeyScheme); }
    [Fact] public void AIChatServiceOptions_Defaults_MaxMessages() { var o = new AIChatServiceOptions(); Assert.Equal(20, o.MaxMessages); }
    [Fact] public void AIChatServiceOptions_Defaults_Temperature() { var o = new AIChatServiceOptions(); Assert.Equal(0.7, o.Temperature); }
    [Fact] public void AIChatServiceOptions_Defaults_MaxTokens() { var o = new AIChatServiceOptions(); Assert.Equal(2048, o.MaxTokens); }
    [Fact] public void AIChatServiceOptions_Defaults_ApiKeyIsNull() { var o = new AIChatServiceOptions(); Assert.Null(o.ApiKey); }
    [Fact] public void AIChatServiceOptions_Defaults_SystemPromptIsNull() { var o = new AIChatServiceOptions(); Assert.Null(o.SystemPrompt); }

    [Fact]
    public void AIChatServiceOptions_SetProperties()
    {
        var o = new AIChatServiceOptions
        {
            Model = "gpt-4",
            Endpoint = "https://custom.ai/v1/chat",
            ApiKey = "sk-test-123",
            ApiKeyHeader = "X-API-Key",
            ApiKeyScheme = "CustomScheme",
            SystemPrompt = "You are helpful.",
            MaxMessages = 50,
            Temperature = 0.5,
            MaxTokens = 4096
        };
        Assert.Equal("gpt-4", o.Model);
        Assert.Equal("https://custom.ai/v1/chat", o.Endpoint);
        Assert.Equal("sk-test-123", o.ApiKey);
        Assert.Equal("X-API-Key", o.ApiKeyHeader);
        Assert.Equal("CustomScheme", o.ApiKeyScheme);
        Assert.Equal("You are helpful.", o.SystemPrompt);
        Assert.Equal(50, o.MaxMessages);
        Assert.Equal(0.5, o.Temperature);
        Assert.Equal(4096, o.MaxTokens);
    }

    [Fact]
    public void AIChatServiceOptions_ApiKey_Nullable()
    {
        var o = new AIChatServiceOptions { ApiKey = null };
        Assert.Null(o.ApiKey);
        o.ApiKey = "new-key";
        Assert.Equal("new-key", o.ApiKey);
        o.ApiKey = null;
        Assert.Null(o.ApiKey);
    }

    [Fact] public void AiRole_System_IntValue() { Assert.Equal(0, (int)AiRole.System); }
    [Fact] public void AiRole_User_IntValue() { Assert.Equal(1, (int)AiRole.User); }
    [Fact] public void AiRole_Assistant_IntValue() { Assert.Equal(2, (int)AiRole.Assistant); }
    [Fact] public void AiRole_Tool_IntValue() { Assert.Equal(3, (int)AiRole.Tool); }
    [Fact] public void AiRole_Other_IntValue() { Assert.Equal(4, (int)AiRole.Other); }
    [Fact] public void AiRole_Enum_HasFiveMembers() { Assert.Equal(5, Enum.GetValues<AiRole>().Length); }
    [Fact] public void AiRole_System_ToString() { Assert.Equal("System", AiRole.System.ToString()); }
    [Fact] public void AiRole_User_ToString() { Assert.Equal("User", AiRole.User.ToString()); }
    [Fact] public void AiRole_Assistant_ToString() { Assert.Equal("Assistant", AiRole.Assistant.ToString()); }
    [Fact] public void AiRole_Tool_ToString() { Assert.Equal("Tool", AiRole.Tool.ToString()); }
    [Fact] public void AiRole_Other_ToString() { Assert.Equal("Other", AiRole.Other.ToString()); }

    [Fact]
    public void AIChatMessage_Constructor_SetsProperties()
    {
        var ts = new DateTime(2026, 6, 27, 10, 0, 0, DateTimeKind.Utc);
        var msg = new AIChatMessage("user", "Hello", ts);
        Assert.Equal("user", msg.Role);
        Assert.Equal("Hello", msg.Content);
        Assert.Equal(ts, msg.Timestamp);
    }

    [Fact]
    public void AIChatMessage_ToString_IncludesValues()
    {
        var ts = new DateTime(2026, 6, 27, 10, 0, 0, DateTimeKind.Utc);
        var msg = new AIChatMessage("assistant", "Hi there", ts);
        var str = msg.ToString();
        Assert.Contains("assistant", str);
        Assert.Contains("Hi there", str);
    }

    [Fact]
    public void AIChatMessage_Equality_SameValues()
    {
        var ts = new DateTime(2026, 6, 27, 10, 0, 0, DateTimeKind.Utc);
        var a = new AIChatMessage("user", "Hello", ts);
        var b = new AIChatMessage("user", "Hello", ts);
        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void AIChatMessage_Equality_DifferentRole()
    {
        var ts = new DateTime(2026, 6, 27, 10, 0, 0, DateTimeKind.Utc);
        var a = new AIChatMessage("user", "Hello", ts);
        var b = new AIChatMessage("assistant", "Hello", ts);
        Assert.NotEqual(a, b);
        Assert.True(a != b);
    }

    [Fact]
    public void AIChatMessage_Equality_DifferentContent()
    {
        var ts = new DateTime(2026, 6, 27, 10, 0, 0, DateTimeKind.Utc);
        var a = new AIChatMessage("user", "Hello", ts);
        var b = new AIChatMessage("user", "World", ts);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void AIChatMessage_Equality_DifferentTimestamp()
    {
        var ts1 = new DateTime(2026, 6, 27, 10, 0, 0, DateTimeKind.Utc);
        var ts2 = new DateTime(2026, 6, 27, 11, 0, 0, DateTimeKind.Utc);
        var a = new AIChatMessage("user", "Hello", ts1);
        var b = new AIChatMessage("user", "Hello", ts2);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void ConversationSession_Constructor_SetsId()
    {
        var s = new ConversationSession("s1");
        Assert.Equal("s1", s.Id);
        Assert.NotNull(s.Messages);
        Assert.Empty(s.Messages);
        Assert.Equal(0, s.MessageCount);
    }

    [Fact]
    public void ConversationSession_CreatedAt_SetOnConstruction()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var s = new ConversationSession("s1");
        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.InRange(s.CreatedAt, before, after);
    }

    [Fact]
    public void ConversationSession_LastUpdatedAt_EqualsCreatedAtInitially()
    {
        var s = new ConversationSession("s1");
        Assert.True((s.LastUpdatedAt - s.CreatedAt).TotalSeconds < 1);
    }

    [Fact]
    public void ConversationSession_AddMessage_AppendsToList()
    {
        var s = new ConversationSession("s1");
        var msg = new AIChatMessage("user", "Hi", DateTime.UtcNow);
        s.AddMessage(msg);
        Assert.Equal(1, s.MessageCount);
        Assert.Same(msg, Assert.Single(s.Messages));
    }

    [Fact]
    public void ConversationSession_AddMessage_UpdatesLastUpdatedAt()
    {
        var s = new ConversationSession("s1");
        var before = s.LastUpdatedAt;
        System.Threading.Thread.Sleep(5);
        s.AddMessage(new AIChatMessage("user", "Hi", DateTime.UtcNow));
        Assert.True(s.LastUpdatedAt > before, "LastUpdatedAt should advance after AddMessage");
    }

    [Fact]
    public void ConversationSession_AddMessage_TrimsToMax20()
    {
        var s = new ConversationSession("s1");
        for (var i = 0; i < 25; i++)
            s.AddMessage(new AIChatMessage("user", "Msg" + i, DateTime.UtcNow));
        Assert.Equal(20, s.MessageCount);
        Assert.DoesNotContain(s.Messages, m => m.Content == "Msg0");
        Assert.DoesNotContain(s.Messages, m => m.Content == "Msg1");
        Assert.DoesNotContain(s.Messages, m => m.Content == "Msg2");
        Assert.DoesNotContain(s.Messages, m => m.Content == "Msg3");
        Assert.DoesNotContain(s.Messages, m => m.Content == "Msg4");
        Assert.Contains(s.Messages, m => m.Content == "Msg24");
    }

    [Fact]
    public void ConversationSession_AddMessage_DoesNotTrimBelow20()
    {
        var s = new ConversationSession("s1");
        for (var i = 0; i < 15; i++)
            s.AddMessage(new AIChatMessage("user", "Msg" + i, DateTime.UtcNow));
        Assert.Equal(15, s.MessageCount);
    }

    [Fact]
    public void ConversationSession_Clear_RemovesAllMessages()
    {
        var s = new ConversationSession("s1");
        s.AddMessage(new AIChatMessage("user", "Hi", DateTime.UtcNow));
        s.AddMessage(new AIChatMessage("assistant", "Hello", DateTime.UtcNow));
        s.Clear();
        Assert.Empty(s.Messages);
        Assert.Equal(0, s.MessageCount);
    }

    [Fact]
    public void ConversationSession_Clear_AfterClearCanAddAgain()
    {
        var s = new ConversationSession("s1");
        s.AddMessage(new AIChatMessage("user", "Hi", DateTime.UtcNow));
        s.Clear();
        s.AddMessage(new AIChatMessage("user", "New", DateTime.UtcNow));
        Assert.Equal(1, s.MessageCount);
        Assert.Equal("New", s.Messages[0].Content);
    }

    [Fact]
    public void ConversationSession_MessageCount_TracksAdds()
    {
        var s = new ConversationSession("s1");
        Assert.Equal(0, s.MessageCount);
        s.AddMessage(new AIChatMessage("user", "A", DateTime.UtcNow));
        Assert.Equal(1, s.MessageCount);
        s.AddMessage(new AIChatMessage("user", "B", DateTime.UtcNow));
        Assert.Equal(2, s.MessageCount);
        s.Clear();
        Assert.Equal(0, s.MessageCount);
    }

    [Fact]
    public void ConversationSession_Id_Immutable()
    {
        var s = new ConversationSession("test-id");
        Assert.Equal("test-id", s.Id);
    }

    [Fact]
    public void ConversationSession_AddMessage_Multiple_Ordered()
    {
        var s = new ConversationSession("s1");
        var m1 = new AIChatMessage("user", "First", DateTime.UtcNow);
        var m2 = new AIChatMessage("assistant", "Second", DateTime.UtcNow);
        var m3 = new AIChatMessage("user", "Third", DateTime.UtcNow);
        s.AddMessage(m1);
        s.AddMessage(m2);
        s.AddMessage(m3);
        Assert.Equal(3, s.MessageCount);
        Assert.Same(m1, s.Messages[0]);
        Assert.Same(m2, s.Messages[1]);
        Assert.Same(m3, s.Messages[2]);
    }

    [Fact]
    public void ConversationSession_Messages_CanBeEnumerated()
    {
        var s = new ConversationSession("s1");
        s.AddMessage(new AIChatMessage("user", "A", DateTime.UtcNow));
        s.AddMessage(new AIChatMessage("user", "B", DateTime.UtcNow));
        Assert.Collection(s.Messages,
            m => Assert.Equal("A", m.Content),
            m => Assert.Equal("B", m.Content));
    }

    [Fact]
    public void IAIChatService_ImplementedByAIChatService()
    {
        Assert.True(typeof(IAIChatService).IsAssignableFrom(typeof(AIChatService)));
    }

    [Fact]
    public void IAIChatService_HasExpectedMembers()
    {
        var members = typeof(IAIChatService).GetMethods();
        Assert.Contains(members, m => m.Name == "GetCompletionsAsync" && m.GetParameters().Length == 3);
        Assert.Contains(members, m => m.Name == "GetSession" && m.GetParameters().Length == 1);
        Assert.Contains(members, m => m.Name == "CreateSession" && m.GetParameters().Length == 1);
        Assert.Contains(members, m => m.Name == "ClearSession" && m.GetParameters().Length == 1);
        Assert.Contains(members, m => m.Name == "RemoveSession" && m.GetParameters().Length == 1);
        Assert.True(typeof(IAIChatService).GetProperty("SessionCount") is not null);
    }
}
