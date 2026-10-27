using System.Threading.Tasks;
using XForge.DesignSystem.Services.AI;
using Xunit;
namespace XForge.DesignSystem.Tests.Services.AI;
public class AIChatServiceTests
{
    [Fact] public void New_Service_Has_Zero_Sessions() { var svc = new AIChatService(new System.Net.Http.HttpClient(), Microsoft.Extensions.Options.Options.Create(new AIChatServiceOptions())); Assert.Equal(0, svc.SessionCount); }
    [Fact] public void CreateSession_Returns_NonNull_Session() { var svc = new AIChatService(new System.Net.Http.HttpClient(), Microsoft.Extensions.Options.Options.Create(new AIChatServiceOptions())); var s = svc.CreateSession("s1"); Assert.NotNull(s); Assert.Equal("s1", s.Id); Assert.Equal(1, svc.SessionCount); }
    [Fact] public void GetSession_Returns_Null_For_Unknown() { var svc = new AIChatService(new System.Net.Http.HttpClient(), Microsoft.Extensions.Options.Options.Create(new AIChatServiceOptions())); Assert.Null(svc.GetSession("missing")); }
    [Fact] public void ClearSession_Clears_Messages() { var svc = new AIChatService(new System.Net.Http.HttpClient(), Microsoft.Extensions.Options.Options.Create(new AIChatServiceOptions())); var s = svc.CreateSession("s1"); svc.ClearSession("s1"); Assert.Equal(0, s.MessageCount); }
    [Fact] public void RemoveSession_Decrements_Count() { var svc = new AIChatService(new System.Net.Http.HttpClient(), Microsoft.Extensions.Options.Options.Create(new AIChatServiceOptions())); svc.CreateSession("s1"); svc.RemoveSession("s1"); Assert.Equal(0, svc.SessionCount); }
    [Fact] public async Task GetCompletionsAsync_Streams_Chunks() { var svc = new AIChatService(new System.Net.Http.HttpClient(), Microsoft.Extensions.Options.Options.Create(new AIChatServiceOptions())); svc.CreateSession("s1"); var combined = string.Empty; await foreach (var c in svc.GetCompletionsAsync("s1","hi")) combined += c; Assert.Equal("Hello from AI stream", combined); }
    [Fact] public async Task GetCompletionsAsync_Adds_User_And_Assistant() { var svc = new AIChatService(new System.Net.Http.HttpClient(), Microsoft.Extensions.Options.Options.Create(new AIChatServiceOptions())); svc.CreateSession("s1"); await foreach (var _ in svc.GetCompletionsAsync("s1","hi")) { } var s = svc.GetSession("s1"); Assert.Equal(2, s!.MessageCount); }
}