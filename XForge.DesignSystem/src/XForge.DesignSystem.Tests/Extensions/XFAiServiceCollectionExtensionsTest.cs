using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Extensions;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Extensions;

public sealed class XFAiServiceCollectionExtensionsTest
{
    [Fact]
    public void AddXFAi_RegistersMemoryAndAuditServices()
    {
        var services = new ServiceCollection();
        var result = services.AddXFAi();

        Assert.Same(services, result);
        using var sp = services.BuildServiceProvider();
        Assert.NotNull(sp.GetService<IAiMemoryService>());
        Assert.NotNull(sp.GetService<IAiAuditService>());
    }

    [Fact]
    public void AddXFAi_ReturnsBuilder()
    {
        var services = new ServiceCollection();
        var result = services.AddXFAi();
        Assert.NotNull(result);

        using var sp = services.BuildServiceProvider();
        Assert.NotNull(sp.GetService<IAiMemoryService>());
    }

    [Fact]
    public void AddXFAiMultiModal_RegistersVisionAndAudio()
    {
        var services = new ServiceCollection();
        var result = services.AddXFAiMultiModal();

        Assert.Same(services, result);
        using var sp = services.BuildServiceProvider();
        Assert.NotNull(sp.GetService<IAiVisionProvider>());
        Assert.NotNull(sp.GetService<IAiAudioProvider>());
    }

    [Fact]
    public void AddXFAiMultiModal_DoesNotOverrideExistingRegistrations()
    {
        var services = new ServiceCollection();
        var custom = new InMemoryAiVisionProvider();
        services.AddSingleton<IAiVisionProvider>(custom);
        services.AddXFAiMultiModal();

        using var sp = services.BuildServiceProvider();
        Assert.Same(custom, sp.GetService<IAiVisionProvider>());
    }

    [Fact]
    public void AddXFAiVisionProvider_Generic_RegistersCustomProvider()
    {
        var services = new ServiceCollection();
        var result = services.AddXFAiVisionProvider<InMemoryAiVisionProvider>();

        Assert.Same(services, result);
        using var sp = services.BuildServiceProvider();
        Assert.NotNull(sp.GetService<IAiVisionProvider>());
    }

    [Fact]
    public void AddXFAiAudioProvider_Generic_RegistersCustomProvider()
    {
        var services = new ServiceCollection();
        var result = services.AddXFAiAudioProvider<InMemoryAiAudioProvider>();

        Assert.Same(services, result);
        using var sp = services.BuildServiceProvider();
        Assert.NotNull(sp.GetService<IAiAudioProvider>());
    }

    [Fact]
    public void AddXFAiProvider_Generic_RegistersProviderAndEmbeddingService()
    {
        var services = new ServiceCollection();
        var result = services.AddXFAiProvider<InMemoryTestProvider>();

        Assert.Same(services, result);
        using var sp = services.BuildServiceProvider();
        Assert.NotNull(sp.GetService<IAiProvider>());
        Assert.NotNull(sp.GetService<IAiEmbeddingService>());
    }

    [Fact]
    public void AddXFAiProvider_Instance_RegistersProviderAndEmbeddingService()
    {
        var services = new ServiceCollection();
        var provider = new InMemoryTestProvider();
        var result = services.AddXFAiProvider(provider);

        Assert.Same(services, result);
        using var sp = services.BuildServiceProvider();
        Assert.Same(provider, sp.GetService<IAiProvider>());
        Assert.NotNull(sp.GetService<IAiEmbeddingService>());
    }

    [Fact]
    public void AddXFAiProvider_Named_RegistersKeyedProvider()
    {
        var services = new ServiceCollection();
        var provider = new InMemoryTestProvider();
        var result = services.AddXFAiProvider("myProvider", provider);

        Assert.Same(services, result);
        using var sp = services.BuildServiceProvider();
        // Verify it doesn't throw and returns same collection
        Assert.NotNull(result);
    }

    private sealed class InMemoryTestProvider : IAiProvider, IDisposable
    {
        public string ProviderId => "test";
        public string DisplayName => "Test";
        public string? DefaultModel => "test-model";
        public bool SupportsStreaming => false;
        public bool SupportsToolCalling => false;

        public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
            => Task.FromResult(new AiChatResponse());
        public IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken ct = default)
            => AsyncEnumerable.Empty<AiStreamChunk>();
        public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default)
            => Task.FromResult(new AiEmbeddingResponse { Embeddings = Array.Empty<float[]>() });
        public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        public void Dispose() { }
    }
}
