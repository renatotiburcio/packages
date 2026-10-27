namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Semantic embedding service for RAG, search, and similarity operations.
/// Wraps an IAiProvider to provide high-level embedding operations.
/// </summary>
public interface IAiEmbeddingService
{
    /// <summary>
    /// Generates an embedding vector for the given text.
    /// </summary>
    Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embedding vectors for multiple texts in a batch.
    /// </summary>
    Task<IReadOnlyList<float[]>> GetEmbeddingsAsync(IReadOnlyList<string> texts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates cosine similarity between two embedding vectors.
    /// Returns a value between -1.0 (opposite) and 1.0 (identical).
    /// </summary>
    double CosineSimilarity(float[] a, float[] b);

    /// <summary>
    /// Finds the top-K most similar embeddings from a collection.
    /// Returns indices and similarity scores sorted by descending similarity.
    /// </summary>
    IReadOnlyList<(int Index, double Score)> FindNearest(float[] query, IReadOnlyList<float[]> candidates, int topK = 5);

    /// <summary>
    /// The dimension size of embeddings produced by this service.
    /// </summary>
    int Dimensions { get; }
}
