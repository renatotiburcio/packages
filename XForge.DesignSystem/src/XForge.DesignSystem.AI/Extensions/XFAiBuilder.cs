using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Extensions;

/// <summary>
/// Fluent builder for configuring XForge AI services.
/// </summary>
public sealed class XFAiBuilder
{
    private readonly IServiceCollection _services;

    internal XFAiBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    /// Gets the service collection for additional registrations.
    /// </summary>
    public IServiceCollection Services => _services;

    /// <summary>
    /// Registers OpenAI as the default AI provider.
    /// </summary>
    public XFAiBuilder UseOpenAI(string apiKey, string? baseUrl = null, string? defaultModel = null)
    {
        _services.TryAddSingleton<HttpClient>();
        _services.AddSingleton<IAiProvider>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            return new OpenAiProvider(httpClient, apiKey, baseUrl ?? "https://api.openai.com/v1");
        });
        RegisterEmbeddingService();
        return this;
    }

    /// <summary>
    /// Registers Ollama as the default AI provider.
    /// </summary>
    public XFAiBuilder UseOllama(string baseUrl = "http://localhost:11434")
    {
        _services.TryAddSingleton<HttpClient>();
        _services.AddSingleton<IAiProvider>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            return new OllamaProvider(httpClient, baseUrl);
        });
        RegisterEmbeddingService();
        return this;
    }

    /// <summary>
    /// Registers Google Gemini as the default AI provider.
    /// </summary>
    public XFAiBuilder UseGemini(string apiKey, string? baseUrl = null)
    {
        _services.TryAddSingleton<HttpClient>();
        _services.AddSingleton<IAiProvider>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            return new GeminiProvider(httpClient, apiKey, baseUrl ?? "https://generativelanguage.googleapis.com/v1beta");
        });
        RegisterEmbeddingService();
        return this;
    }

    /// <summary>
    /// Registers Anthropic Claude as the default AI provider.
    /// </summary>
    public XFAiBuilder UseAnthropic(string apiKey, string? baseUrl = null)
    {
        _services.TryAddSingleton<HttpClient>();
        _services.AddSingleton<IAiProvider>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            return new AnthropicProvider(httpClient, apiKey, baseUrl ?? "https://api.anthropic.com/v1");
        });
        RegisterEmbeddingService();
        return this;
    }

    /// <summary>
    /// Registers Azure OpenAI as the default AI provider.
    /// </summary>
    public XFAiBuilder UseAzureOpenAI(string resourceName, string deploymentName, string apiKey, string apiVersion = "2024-10-21")
    {
        _services.TryAddSingleton<HttpClient>();
        _services.AddSingleton<IAiProvider>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            return new AzureOpenAiProvider(httpClient, resourceName, deploymentName, apiKey, apiVersion);
        });
        RegisterEmbeddingService();
        return this;
    }

    /// <summary>
    /// Registers OpenRouter as the default AI provider.
    /// </summary>
    public XFAiBuilder UseOpenRouter(string apiKey, string? httpReferer = null, string? appTitle = null)
    {
        _services.TryAddSingleton<HttpClient>();
        _services.AddSingleton<IAiProvider>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            return new OpenRouterProvider(httpClient, apiKey, httpReferer ?? "https://xforge.designsystem", appTitle ?? "XForge Design System");
        });
        RegisterEmbeddingService();
        return this;
    }

    /// <summary>
    /// Registers DeepSeek as the default AI provider.
    /// </summary>
    public XFAiBuilder UseDeepSeek(string apiKey, string? baseUrl = null)
    {
        _services.TryAddSingleton<HttpClient>();
        _services.AddSingleton<IAiProvider>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            return new DeepSeekProvider(httpClient, apiKey, baseUrl ?? "https://api.deepseek.com/v1");
        });
        RegisterEmbeddingService();
        return this;
    }

    /// <summary>
    /// Registers a local LLM endpoint as the default AI provider.
    /// </summary>
    public XFAiBuilder UseLocalLlm(string baseUrl, string? apiKey = null)
    {
        _services.TryAddSingleton<HttpClient>();
        _services.AddSingleton<IAiProvider>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            return new LocalLlmProvider(httpClient, baseUrl, apiKey);
        });
        RegisterEmbeddingService();
        return this;
    }

    /// <summary>
    /// Registers a custom provider implementation.
    /// </summary>
    public XFAiBuilder UseProvider<TProvider>() where TProvider : class, IAiProvider
    {
        _services.AddSingleton<IAiProvider, TProvider>();
        RegisterEmbeddingService();
        return this;
    }

    /// <summary>
    /// Registers a named provider for multi-provider scenarios.
    /// </summary>
    public XFAiBuilder AddProvider(string name, IAiProvider provider)
    {
        _services.AddKeyedSingleton(name, provider);
        return this;
    }

    /// <summary>
    /// Registers the in-memory AI provider (for development/testing).
    /// </summary>
    public XFAiBuilder UseInMemory()
    {
        _services.TryAddSingleton<IAiProvider, InMemoryAiProvider>();
        RegisterEmbeddingService();
        return this;
    }

    /// <summary>
    /// Registers the AI memory service.
    /// </summary>
    public XFAiBuilder WithMemory<TMemory>() where TMemory : class, IAiMemoryService
    {
        _services.AddSingleton<IAiMemoryService, TMemory>();
        return this;
    }

    /// <summary>
    /// Registers the in-memory AI memory service (default).
    /// </summary>
    public XFAiBuilder WithInMemoryMemory()
    {
        _services.TryAddSingleton<IAiMemoryService, InMemoryAiMemoryService>();
        return this;
    }

    /// <summary>
    /// Registers the AI audit service.
    /// </summary>
    public XFAiBuilder WithAudit<TAudit>() where TAudit : class, IAiAuditService
    {
        _services.AddSingleton<IAiAuditService, TAudit>();
        return this;
    }

    /// <summary>
    /// Registers the in-memory AI audit service (default).
    /// </summary>
    public XFAiBuilder WithInMemoryAudit()
    {
        _services.TryAddSingleton<IAiAuditService, InMemoryAiAuditService>();
        return this;
    }

    /// <summary>
    /// Registers a web search service.
    /// </summary>
    public XFAiBuilder WithWebSearch<TSearch>() where TSearch : class, IWebSearchService
    {
        _services.AddSingleton<IWebSearchService, TSearch>();
        return this;
    }

    /// <summary>
    /// Registers a vision provider.
    /// </summary>
    public XFAiBuilder WithVision<TVision>() where TVision : class, IAiVisionProvider
    {
        _services.AddSingleton<IAiVisionProvider, TVision>();
        return this;
    }

    /// <summary>
    /// Registers an audio provider.
    /// </summary>
    public XFAiBuilder WithAudio<TAudio>() where TAudio : class, IAiAudioProvider
    {
        _services.AddSingleton<IAiAudioProvider, TAudio>();
        return this;
    }

    /// <summary>
    /// Registers multi-modal services (vision + audio) with in-memory implementations.
    /// </summary>
    public XFAiBuilder WithMultiModal()
    {
        _services.TryAddSingleton<IAiVisionProvider, InMemoryAiVisionProvider>();
        _services.TryAddSingleton<IAiAudioProvider, InMemoryAiAudioProvider>();
        return this;
    }

    /// <summary>
    /// Binds configuration from appsettings.json.
    /// </summary>
    public XFAiBuilder BindConfiguration(string sectionName = "XForgeAI")
    {
        _services.AddOptions<XFAiOptions>()
            .BindConfiguration(sectionName)
            ;
        return this;
    }

    private void RegisterEmbeddingService()
    {
        _services.TryAddSingleton<IAiEmbeddingService>(sp =>
        {
            var provider = sp.GetRequiredService<IAiProvider>();
            return new XFAiEmbeddingService(provider);
        });
    }
}

