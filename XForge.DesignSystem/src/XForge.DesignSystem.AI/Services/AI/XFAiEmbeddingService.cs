using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Default embedding service that delegates to an IAiProvider.
/// Includes pure C# cosine similarity and nearest-neighbor search.
/// </summary>
public sealed class XFAiEmbeddingService : IAiEmbeddingService
{
    private readonly IAiProvider _provider;
    private readonly string? _model;

    public XFAiEmbeddingService(IAiProvider provider, string? model = null)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _model = model;
    }

    /// <inheritdoc/>
    public int Dimensions { get; private set; }

    /// <inheritdoc/>
    public async Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var response = await _provider.EmbedAsync(
            new AiEmbeddingRequest { Inputs = [text], Model = _model },
            cancellationToken).ConfigureAwait(false);

        var embedding = response.Embeddings.FirstOrDefault() ?? [];
        if (embedding.Length > 0 && Dimensions == 0)
        {
            Dimensions = embedding.Length;
        }
        return embedding;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<float[]>> GetEmbeddingsAsync(IReadOnlyList<string> texts, CancellationToken cancellationToken = default)
    {
        var response = await _provider.EmbedAsync(
            new AiEmbeddingRequest { Inputs = texts, Model = _model },
            cancellationToken).ConfigureAwait(false);

        if (response.Embeddings.Count > 0 && response.Embeddings[0].Length > 0 && Dimensions == 0)
        {
            Dimensions = response.Embeddings[0].Length;
        }
        return response.Embeddings;
    }

    /// <inheritdoc/>
    public double CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length || a.Length == 0) return 0;
        double dot = 0, normA = 0, normB = 0;
        for (var i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }
        var denominator = Math.Sqrt(normA) * Math.Sqrt(normB);
        return denominator == 0 ? 0 : dot / denominator;
    }

    /// <inheritdoc/>
    public IReadOnlyList<(int Index, double Score)> FindNearest(float[] query, IReadOnlyList<float[]> candidates, int topK = 5)
    {
        var scored = new List<(int Index, double Score)>(candidates.Count);
        for (var i = 0; i < candidates.Count; i++)
        {
            scored.Add((i, CosineSimilarity(query, candidates[i])));
        }
        return scored.OrderByDescending(s => s.Score).Take(topK).ToList();
    }
}
