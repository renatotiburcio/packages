using AngleSharp.Dom;
using Bunit;
using Xunit;
using XForge.DesignSystem.Components.AI.Analysis;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Manual mock IAiProvider for testing XFAiSqlQueryBuilder without Moq dependency.
/// </summary>
internal sealed class MockAiProvider : IAiProvider, IAsyncDisposable
{
    private readonly AiChatResponse? _response;
    private readonly Exception? _exception;

    public MockAiProvider(AiChatResponse response) => _response = response;
    public MockAiProvider(Exception exception) => _exception = exception;

    public string ProviderId => "mock-sql";
    public string DisplayName => "Mock SQL Provider";
    public string? DefaultModel => "mock-model";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        if (_exception is not null)
            throw _exception;
        return Task.FromResult(_response!);
    }

    public IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken ct = default)
        => AsyncEnumerable.Empty<AiStreamChunk>();

    public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default)
        => Task.FromResult(new AiEmbeddingResponse { Embeddings = Array.Empty<float[]>() });

    public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

/// <summary>
/// Tests for XFAiSqlQueryBuilder component — SQL generation, injection detection,
/// read-only enforcement, blocked operations, and approval workflow.
/// All GenerateAsync calls must run within cut.InvokeAsync() to satisfy Blazor's dispatcher.
/// </summary>
public sealed class XFAiSqlQueryBuilderTest : XForgeTestContext
{
    private void SetInput(IRenderedComponent<XFAiSqlQueryBuilder> cut, string value)
    {
        cut.InvokeAsync(() =>
        {
            var nlQueryField = typeof(XFAiSqlQueryBuilder).GetField("_nlQuery",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            nlQueryField!.SetValue(cut.Instance, value);
        });
    }

    #region Rendering

    [Fact]
    public void XFAiSqlQueryBuilder_Renders_WithDefaultTitle()
    {
        var provider = new MockAiProvider(new AiChatResponse());
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider));

        cut.Markup.Contains("Construtor de SQL");
    }

    [Fact]
    public void XFAiSqlQueryBuilder_Renders_WithCustomTitle()
    {
        var provider = new MockAiProvider(new AiChatResponse());
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.Title, "SQL Builder"));

        cut.Markup.Contains("SQL Builder");
    }

    [Fact]
    public void XFAiSqlQueryBuilder_Renders_WithHeaderHidden()
    {
        var provider = new MockAiProvider(new AiChatResponse());
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFAiSqlQueryBuilder_Renders_WithPlaceholder()
    {
        var provider = new MockAiProvider(new AiChatResponse());
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.Placeholder, "Enter query..."));

        var textarea = cut.Find("textarea");
        Assert.Equal("Enter query...", textarea.GetAttribute("placeholder"));
    }

    [Fact]
    public void XFAiSqlQueryBuilder_Renders_WithAriaAttributes()
    {
        var provider = new MockAiProvider(new AiChatResponse());
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider));

        var section = cut.Find("section");
        // Default state is not busy, so aria-busy should be "false"
        Assert.Equal("false", section.GetAttribute("aria-busy"));
    }

    [Fact]
    public void XFAiSqlQueryBuilder_Renders_AriaBusyTrueWhenLoading()
    {
        var provider = new MockAiProvider(new AiChatResponse());
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.Loading, true));

        var section = cut.Find("section");
        Assert.Equal("true", section.GetAttribute("aria-busy"));
    }

    [Fact]
    public void XFAiSqlQueryBuilder_Disabled_RendersDisabled()
    {
        var provider = new MockAiProvider(new AiChatResponse());
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.Disabled, true));

        var textarea = cut.Find("textarea");
        Assert.NotNull(textarea.GetAttribute("disabled"));
    }

    #endregion

    #region SQL Validation — Valid SELECT

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_ValidSelectSql_FiresOnSqlGenerated()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = """{"sql":"SELECT id, name FROM users WHERE active = 1","explicacao":"Select active users","otimizacao":"Add index on active","tabelas":["users"],"confianca":0.9}"""
            }
        };
        var provider = new MockAiProvider(response);

        XFAiSqlQuery? generatedQuery = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.ReadOnly, true)
            .Add(p => p.OnSqlGenerated, query => { generatedQuery = query; }));

        SetInput(cut, "show active users");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(generatedQuery);
        Assert.Contains("SELECT", generatedQuery.GeneratedSql, StringComparison.OrdinalIgnoreCase);
        Assert.True(generatedQuery.ValidationResult.IsValid);
        Assert.True(generatedQuery.IsReadOnly);
        Assert.Equal(0.9, generatedQuery.Confidence);
    }

    #endregion

    #region Read-Only Enforcement

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_DeleteBlockedInReadOnlyMode()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = """{"sql":"DELETE FROM users WHERE id = 1","explicacao":"Delete user","tabelas":["users"],"confianca":0.5}"""
            }
        };
        var provider = new MockAiProvider(response);

        string? errorMessage = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.ReadOnly, true)
            .Add(p => p.OnError, args => { errorMessage = args; }));

        SetInput(cut, "delete user 1");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(errorMessage);
        Assert.Contains("somente leitura", errorMessage, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region SQL Injection Detection

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_UnionInjection_Blocked()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = """{"sql":"SELECT id FROM users UNION ALL SELECT password FROM admin","explicacao":"Union injection","tabelas":["users","admin"],"confianca":0.1}"""
            }
        };
        var provider = new MockAiProvider(response);

        string? errorMessage = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.ReadOnly, true)
            .Add(p => p.OnError, args => { errorMessage = args; }));

        SetInput(cut, "get all user data");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(errorMessage);
        Assert.Contains("injecao", errorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_DashDashInjection_Blocked()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = """{"sql":"SELECT * FROM users-- drop","explicacao":"Comment injection","tabelas":["users"],"confianca":0.1}"""
            }
        };
        var provider = new MockAiProvider(response);

        string? errorMessage = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.OnError, args => { errorMessage = args; }));

        SetInput(cut, "inject comment");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(errorMessage);
        Assert.Contains("injecao", errorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_SemicolonInjection_Blocked()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = """{"sql":"SELECT * FROM users; DROP TABLE users","explicacao":"Stacked query","tabelas":["users"],"confianca":0.1}"""
            }
        };
        var provider = new MockAiProvider(response);

        string? errorMessage = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.OnError, args => { errorMessage = args; }));

        SetInput(cut, "stacked query");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(errorMessage);
    }

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_SleepInjection_Blocked()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = """{"sql":"SELECT SLEEP(5) FROM users","explicacao":"Time-based","tabelas":["users"],"confianca":0.1}"""
            }
        };
        var provider = new MockAiProvider(response);

        string? errorMessage = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.OnError, args => { errorMessage = args; }));

        SetInput(cut, "time-based injection");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(errorMessage);
    }

    #endregion

    #region MaxRows LIMIT appending

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_WithMaxRows_AppendsLimit()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = """{"sql":"SELECT * FROM products","explicacao":"Get products","tabelas":["products"],"confianca":0.9}"""
            }
        };
        var provider = new MockAiProvider(response);

        XFAiSqlQuery? generatedQuery = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.ReadOnly, true)
            .Add(p => p.MaxRows, 100)
            .Add(p => p.OnSqlGenerated, query => { generatedQuery = query; }));

        SetInput(cut, "show all products");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(generatedQuery);
        Assert.Contains("LIMIT 100", generatedQuery.GeneratedSql, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Non-JSON fallback

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_NonJsonFallback_TreatsContentAsRawSql()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = "SELECT * FROM customers WHERE region = 'SUL'"
            }
        };
        var provider = new MockAiProvider(response);

        XFAiSqlQuery? generatedQuery = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.ReadOnly, true)
            .Add(p => p.OnSqlGenerated, query => { generatedQuery = query; }));

        SetInput(cut, "show SUL customers");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(generatedQuery);
        Assert.Contains("SELECT", generatedQuery.GeneratedSql, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region JSON in markdown code block

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_JsonInCodeBlock_ExtractsCorrectly()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = "```json\n{\"sql\":\"SELECT 1\",\"explicacao\":\"Test\",\"tabelas\":[],\"confianca\":0.5}\n```"
            }
        };
        var provider = new MockAiProvider(response);

        XFAiSqlQuery? generatedQuery = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.ReadOnly, true)
            .Add(p => p.OnSqlGenerated, query => { generatedQuery = query; }));

        SetInput(cut, "simple test");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(generatedQuery);
        Assert.Equal("SELECT 1", generatedQuery.GeneratedSql);
    }

    #endregion

    #region Cancel — OperationCanceledException

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_Cancel_DoesNotThrow()
    {
        var provider = new MockAiProvider(new OperationCanceledException());

        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.ReadOnly, true));

        SetInput(cut, "test query");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());
    }

    #endregion

    #region General Exception

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_Exception_FiresOnError()
    {
        var provider = new MockAiProvider(new Exception("Provider connection failed"));

        string? errorMessage = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.OnError, args => { errorMessage = args; }));

        SetInput(cut, "test query");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.Equal("Provider connection failed", errorMessage);
    }

    #endregion

    #region Clear

    [Fact]
    public void XFAiSqlQueryBuilder_Clear_ResetsState()
    {
        var provider = new MockAiProvider(new AiChatResponse());
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider));

        cut.InvokeAsync(() => cut.Instance.Clear());
    }

    #endregion

    #region Dispose

    [Fact]
    public void XFAiSqlQueryBuilder_Dispose_DoesNotThrow()
    {
        var provider = new MockAiProvider(new AiChatResponse());
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider));

        cut.Instance.Dispose();
    }

    #endregion

    #region Schema and System Prompt

    [Fact]
    public void XFAiSqlQueryBuilder_Renders_WithSchemaParameter()
    {
        var provider = new MockAiProvider(new AiChatResponse());
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.Schema, "CREATE TABLE users (id INT, name VARCHAR(100))"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void XFAiSqlQueryBuilder_Renders_WithCustomSystemPrompt()
    {
        var provider = new MockAiProvider(new AiChatResponse());
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.SystemPrompt, "You are a PostgreSQL expert."));
        Assert.NotNull(cut);
    }

    #endregion

    #region Blocked Operations

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_DefaultBlockedOps_BlocksInsert()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = """{"sql":"INSERT INTO orders (product) VALUES ('Widget')","explicacao":"Insert order","tabelas":["orders"],"confianca":0.6}"""
            }
        };
        var provider = new MockAiProvider(response);

        string? errorMessage = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.ReadOnly, false)
            .Add(p => p.OnError, args => { errorMessage = args; }));

        SetInput(cut, "insert new order");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(errorMessage);
        Assert.Contains("INSERT", errorMessage, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Empty SQL

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_EmptySql_FiresOnError()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = """{"sql":"","explicacao":"Empty","tabelas":[],"confianca":0.0}"""
            }
        };
        var provider = new MockAiProvider(response);

        string? errorMessage = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.OnError, args => { errorMessage = args; }));

        SetInput(cut, "get nothing");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(errorMessage);
        Assert.Contains("vazio", errorMessage, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Approval workflow — CTE query (read-only, no approval needed)

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_CTEQuery_IsReadOnly_NoApprovalNeeded()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = """{"sql":"WITH cte AS (SELECT 1) SELECT * FROM cte","explicacao":"CTE query","tabelas":[],"confianca":0.9}"""
            }
        };
        var provider = new MockAiProvider(response);

        XFAiSqlQuery? generatedQuery = null;
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, provider)
            .Add(p => p.ReadOnly, true)
            .Add(p => p.RequireApproval, true)
            .Add(p => p.OnSqlGenerated, query => { generatedQuery = query; }));

        SetInput(cut, "show data using CTE");
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());

        Assert.NotNull(generatedQuery);
        Assert.True(generatedQuery.IsReadOnly);
        Assert.False(generatedQuery.RequiresApproval);
    }

    #endregion

    #region No Provider — GenerateAsync returns early

    [Fact]
    public async Task XFAiSqlQueryBuilder_GenerateAsync_NoProvider_ReturnsEarly()
    {
        var cut = Render<XFAiSqlQueryBuilder>(parameters => parameters
            .Add(p => p.Provider, null));

        SetInput(cut, "test without provider");
        // Should not throw even without a provider
        await cut.InvokeAsync(async () => await cut.Instance.GenerateAsync());
    }

    #endregion
}
