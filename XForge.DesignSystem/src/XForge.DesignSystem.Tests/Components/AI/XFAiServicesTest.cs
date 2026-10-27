using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Extensions;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for AI Services Foundation: base types, interfaces, provider contracts,
/// embedding service, memory service, audit service, and DI registration.
/// </summary>
public sealed class XFAiServicesTest
{
    #region Base Types — AiRole

    [Fact]
    public void AiRole_HasExpectedValues()
    {
        Assert.Equal(4, Enum.GetValues<AiRole>().Length);
        Assert.Equal(AiRole.System, (AiRole)0);
        Assert.Equal(AiRole.User, (AiRole)1);
        Assert.Equal(AiRole.Assistant, (AiRole)2);
        Assert.Equal(AiRole.Tool, (AiRole)3);
    }

    #endregion

    #region Base Types — AiMessage

    [Fact]
    public void AiMessage_DefaultConstruction_HasDefaults()
    {
        var msg = new AiMessage();
        Assert.Equal(AiRole.System, msg.Role); // Default enum value is 0 = System
        Assert.Equal(string.Empty, msg.Content);
        Assert.Null(msg.Name);
        Assert.Null(msg.ToolCalls);
        Assert.Null(msg.ToolCallId);
        Assert.Null(msg.Citations);
        Assert.Null(msg.Reasoning);
        Assert.Null(msg.Metadata);
        Assert.True(msg.Timestamp <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void AiMessage_WithValues_RoundtripsCorrectly()
    {
        var msg = new AiMessage
        {
            Role = AiRole.Assistant,
            Content = "Hello world",
            Name = "gpt-4o",
            ToolCalls = [new AiToolCall { Id = "tc1", Name = "get_weather", Arguments = "{}" }],
            Citations = [new AiCitation { Title = "Doc", Score = 0.9 }],
            Reasoning = "Let me think...",
            Metadata = new Dictionary<string, string> { ["key"] = "value" }
        };
        Assert.Equal(AiRole.Assistant, msg.Role);
        Assert.Equal("Hello world", msg.Content);
        Assert.Single(msg.ToolCalls!);
        Assert.Single(msg.Citations!);
        Assert.Equal("Let me think...", msg.Reasoning);
    }

    #endregion

    #region Base Types — AiTokenUsage

    [Fact]
    public void AiTokenUsage_TotalTokens_IsComputed()
    {
        var usage = new AiTokenUsage { PromptTokens = 100, CompletionTokens = 50 };
        Assert.Equal(150, usage.TotalTokens);
    }

    #endregion

    #region Base Types — AiChatRequest

    [Fact]
    public void AiChatRequest_RequiredMessages_MustBeSet()
    {
        var request = new AiChatRequest
        {
            Messages = [new AiMessage { Role = AiRole.User, Content = "Hi" }],
            Model = "gpt-4o",
            Temperature = 0.7,
            MaxTokens = 1024
        };
        Assert.Single(request.Messages);
        Assert.Equal("gpt-4o", request.Model);
    }

    #endregion

    #region Provider Contracts

    [Fact]
    public void OllamaProvider_HasCorrectIdentity()
    {
        using var http = new HttpClient();
        var provider = new OllamaProvider(http);
        Assert.Equal("ollama", provider.ProviderId);
        Assert.Equal("Ollama", provider.DisplayName);
        Assert.True(provider.SupportsStreaming);
        Assert.True(provider.SupportsToolCalling);
        Assert.Equal("llama3", provider.DefaultModel);
    }

    [Fact]
    public void OpenAiProvider_HasCorrectIdentity()
    {
        using var http = new HttpClient();
        var provider = new OpenAiProvider(http, "test-key");
        Assert.Equal("openai", provider.ProviderId);
        Assert.Equal("OpenAI", provider.DisplayName);
        Assert.True(provider.SupportsStreaming);
        Assert.True(provider.SupportsToolCalling);
        Assert.Equal("gpt-4o", provider.DefaultModel);
    }

    [Fact]
    public void AzureOpenAiProvider_HasCorrectIdentity()
    {
        using var http = new HttpClient();
        var provider = new AzureOpenAiProvider(http, "myresource", "gpt-4o", "test-key");
        Assert.Equal("azure-openai", provider.ProviderId);
        Assert.Equal("Azure OpenAI", provider.DisplayName);
        Assert.True(provider.SupportsStreaming);
        Assert.Equal("gpt-4o", provider.DefaultModel);
    }

    [Fact]
    public void AnthropicProvider_HasCorrectIdentity()
    {
        using var http = new HttpClient();
        var provider = new AnthropicProvider(http, "test-key");
        Assert.Equal("anthropic", provider.ProviderId);
        Assert.Equal("Anthropic (Claude)", provider.DisplayName);
        Assert.True(provider.SupportsStreaming);
        Assert.True(provider.SupportsToolCalling);
        Assert.Equal("claude-sonnet-4-20250514", provider.DefaultModel);
    }

    [Fact]
    public void GeminiProvider_HasCorrectIdentity()
    {
        using var http = new HttpClient();
        var provider = new GeminiProvider(http, "test-key");
        Assert.Equal("gemini", provider.ProviderId);
        Assert.Equal("Google Gemini", provider.DisplayName);
        Assert.True(provider.SupportsStreaming);
        Assert.True(provider.SupportsToolCalling);
        Assert.Equal("gemini-2.0-flash", provider.DefaultModel);
    }

    [Fact]
    public void LocalLlmProvider_HasCorrectIdentity()
    {
        using var http = new HttpClient();
        var provider = new LocalLlmProvider(http, "http://localhost:8080");
        Assert.Equal("local-llm", provider.ProviderId);
        Assert.Equal("Local LLM", provider.DisplayName);
        Assert.True(provider.SupportsStreaming);
        Assert.False(provider.SupportsToolCalling);
        Assert.Null(provider.DefaultModel);
    }

    [Fact]
    public void OpenAiProvider_NullApiKey_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new OpenAiProvider(new HttpClient(), null!));
        Assert.Equal("apiKey", ex.ParamName);
    }

    [Fact]
    public void AnthropicProvider_NullApiKey_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new AnthropicProvider(new HttpClient(), null!));
        Assert.Equal("apiKey", ex.ParamName);
    }

    [Fact]
    public void GeminiProvider_NullApiKey_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new GeminiProvider(new HttpClient(), null!));
        Assert.Equal("apiKey", ex.ParamName);
    }

    [Fact]
    public void LocalLlmProvider_NullBaseUrl_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new LocalLlmProvider(new HttpClient(), null!));
        Assert.Equal("baseUrl", ex.ParamName);
    }

    #endregion

    #region Embedding Service — CosineSimilarity

    [Fact]
    public void CosineSimilarity_IdenticalVectors_ReturnsOne()
    {
        var service = CreateEmbeddingService();
        var result = service.CosineSimilarity([1, 0, 0], [1, 0, 0]);
        Assert.Equal(1.0, result, 10);
    }

    [Fact]
    public void CosineSimilarity_OrthogonalVectors_ReturnsZero()
    {
        var service = CreateEmbeddingService();
        var result = service.CosineSimilarity([1, 0, 0], [0, 1, 0]);
        Assert.Equal(0.0, result, 10);
    }

    [Fact]
    public void CosineSimilarity_OppositeVectors_ReturnsMinusOne()
    {
        var service = CreateEmbeddingService();
        var result = service.CosineSimilarity([1, 0], [-1, 0]);
        Assert.Equal(-1.0, result, 10);
    }

    #endregion

    #region Embedding Service — FindNearest

    [Fact]
    public void FindNearest_ReturnsTopKSortedBySimilarity()
    {
        var service = CreateEmbeddingService();
        float[] query = [1, 0, 0];
        float[][] candidates = [[0, 1, 0], [1, 0, 0], [0.7f, 0.7f, 0]];
        var results = service.FindNearest(query, candidates, topK: 2);
        Assert.Equal(2, results.Count);
        Assert.Equal(1, results[0].Index); // [1,0,0] is most similar
        Assert.Equal(2, results[1].Index); // [0.7,0.7,0] is second
        Assert.True(results[0].Score > results[1].Score);
    }

    #endregion

    #region Memory Service

    [Fact]
    public async Task MemoryService_StoreAndRecallAsync()
    {
        var service = new InMemoryAiMemoryService();
        await service.StoreAsync(new AiMemoryEntry { Content = "User prefers dark mode", Scope = AiMemoryScope.LongTerm, Tags = ["preference"] });
        await service.StoreAsync(new AiMemoryEntry { Content = "Last order was #1234", Scope = AiMemoryScope.Session, SessionId = "s1" });

        var longTerm = await service.RecallAsync("dark mode", AiMemoryScope.LongTerm);
        Assert.Single(longTerm);
        Assert.Contains("dark mode", longTerm[0].Content);
    }

    [Fact]
    public async Task MemoryService_GetSessionMemoriesAsync_ReturnsOnlySessionEntries()
    {
        var service = new InMemoryAiMemoryService();
        await service.StoreAsync(new AiMemoryEntry { Content = "Session fact 1", Scope = AiMemoryScope.Session, SessionId = "s1" });
        await service.StoreAsync(new AiMemoryEntry { Content = "Session fact 2", Scope = AiMemoryScope.Session, SessionId = "s1" });
        await service.StoreAsync(new AiMemoryEntry { Content = "Other session", Scope = AiMemoryScope.Session, SessionId = "s2" });

        var results = await service.GetSessionMemoriesAsync("s1");
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task MemoryService_ForgetAsync_RemovesEntry()
    {
        var service = new InMemoryAiMemoryService();
        await service.StoreAsync(new AiMemoryEntry { Id = "m1", Content = "Forget me", Scope = AiMemoryScope.Semantic });
        var removed = await service.ForgetAsync("m1");
        Assert.True(removed);
        var results = await service.RecallAsync("Forget me", AiMemoryScope.Semantic);
        Assert.Empty(results);
    }

    [Fact]
    public async Task MemoryService_ClearAsync_RemovesScopedEntries()
    {
        var service = new InMemoryAiMemoryService();
        await service.StoreAsync(new AiMemoryEntry { Content = "Fact 1", Scope = AiMemoryScope.Semantic });
        await service.StoreAsync(new AiMemoryEntry { Content = "Fact 2", Scope = AiMemoryScope.Semantic });
        await service.StoreAsync(new AiMemoryEntry { Content = "Long term", Scope = AiMemoryScope.LongTerm });

        var removed = await service.ClearAsync(AiMemoryScope.Semantic);
        Assert.Equal(2, removed);

        var remaining = await service.RecallAsync("Long term", AiMemoryScope.LongTerm);
        Assert.Single(remaining);
    }

    [Fact]
    public async Task MemoryService_GetStatsAsync_ReturnsCountsByScope()
    {
        var service = new InMemoryAiMemoryService();
        await service.StoreAsync(new AiMemoryEntry { Content = "a", Scope = AiMemoryScope.Semantic });
        await service.StoreAsync(new AiMemoryEntry { Content = "b", Scope = AiMemoryScope.Semantic });
        await service.StoreAsync(new AiMemoryEntry { Content = "c", Scope = AiMemoryScope.Session, SessionId = "s1" });

        var stats = await service.GetStatsAsync();
        Assert.Equal(2, stats[AiMemoryScope.Semantic]);
        Assert.Equal(1, stats[AiMemoryScope.Session]);
        Assert.Equal(0, stats[AiMemoryScope.LongTerm]);
    }

    #endregion

    #region Audit Service

    [Fact]
    public async Task AuditService_LogAndQueryAsync()
    {
        var service = new InMemoryAiAuditService();
        await service.LogInteractionAsync(new AiAuditEntry
        {
            Provider = "openai",
            UserId = "u1",
            Prompt = "What is 2+2?",
            Completion = "4",
            Success = true,
            Usage = new AiTokenUsage { PromptTokens = 10, CompletionTokens = 5 },
            RiskScore = 0.1
        });

        var results = await service.QueryAsync(new AiAuditQuery { Provider = "openai" });
        Assert.Single(results);
        Assert.Equal("u1", results[0].UserId);
    }

    [Fact]
    public async Task AuditService_GetStatsAsync_AggregatesCorrectly()
    {
        var service = new InMemoryAiAuditService();
        var now = DateTimeOffset.UtcNow;
        await service.LogInteractionAsync(new AiAuditEntry
        {
            Provider = "openai", Success = true,
            Usage = new AiTokenUsage { PromptTokens = 100, CompletionTokens = 50, EstimatedCostUsd = 0.01m },
            RiskScore = 0.2, Duration = TimeSpan.FromSeconds(1), Timestamp = now
        });
        await service.LogInteractionAsync(new AiAuditEntry
        {
            Provider = "openai", Success = false,
            Usage = new AiTokenUsage { PromptTokens = 200, CompletionTokens = 0, EstimatedCostUsd = 0.005m },
            RiskScore = 0.8, Duration = TimeSpan.FromSeconds(3), Timestamp = now
        });

        var stats = await service.GetStatsAsync(now.AddMinutes(-1), now.AddMinutes(1));
        Assert.Equal(2, stats.TotalInteractions);
        Assert.Equal(1, stats.SuccessfulInteractions);
        Assert.Equal(1, stats.FailedInteractions);
        Assert.Equal(300, stats.TotalPromptTokens);
        Assert.Equal(50, stats.TotalCompletionTokens);
        Assert.Equal(0.015m, stats.TotalCostUsd);
        Assert.Equal(0.5, stats.AverageRiskScore, 1);
    }

    [Fact]
    public async Task AuditService_GetUserUsageAsync_ReturnsUserTotals()
    {
        var service = new InMemoryAiAuditService();
        var now = DateTimeOffset.UtcNow;
        await service.LogInteractionAsync(new AiAuditEntry
        {
            Provider = "openai", UserId = "u1",
            Usage = new AiTokenUsage { PromptTokens = 100, CompletionTokens = 50, EstimatedCostUsd = 0.01m },
            Timestamp = now
        });
        await service.LogInteractionAsync(new AiAuditEntry
        {
            Provider = "openai", UserId = "u2",
            Usage = new AiTokenUsage { PromptTokens = 200, CompletionTokens = 100, EstimatedCostUsd = 0.02m },
            Timestamp = now
        });

        var usage = await service.GetUserUsageAsync("u1", now.AddMinutes(-1), now.AddMinutes(1));
        Assert.Equal(100, usage.PromptTokens);
        Assert.Equal(50, usage.CompletionTokens);
        Assert.Equal(0.01m, usage.EstimatedCostUsd);
    }

    #endregion

    #region DI Registration

    [Fact]
    public void AddXFAi_RegistersAllFoundationServices()
    {
        var services = new ServiceCollection();
        services.AddXFAi();
        var sp = services.BuildServiceProvider();

        Assert.NotNull(sp.GetService<IAiMemoryService>());
        Assert.NotNull(sp.GetService<IAiAuditService>());
        Assert.IsType<InMemoryAiMemoryService>(sp.GetService<IAiMemoryService>());
        Assert.IsType<InMemoryAiAuditService>(sp.GetService<IAiAuditService>());
    }

    #endregion

    #region Helpers

    private static XFAiEmbeddingService CreateEmbeddingService()
    {
        // Uses a mock provider — only CosineSimilarity and FindNearest are tested
        return new XFAiEmbeddingService(new MockProvider());
    }

    private sealed class MockProvider : IAiProvider
    {
        public string ProviderId => "mock";
        public string DisplayName => "Mock";
        public bool SupportsStreaming => false;
        public bool SupportsToolCalling => false;
        public string? DefaultModel => null;
        public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default) =>
            Task.FromResult(new AiChatResponse());
        public IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken ct = default) =>
            throw new NotSupportedException();
        public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default) =>
            Task.FromResult(new AiEmbeddingResponse { Embeddings = request.Inputs.Select(_ => new float[] { 1, 0, 0 }).ToList() });
        public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<string>>([]);
    }

    #endregion
}
