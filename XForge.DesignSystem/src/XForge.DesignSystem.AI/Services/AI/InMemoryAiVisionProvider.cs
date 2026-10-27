using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// In-memory implementation of IAiVisionProvider for testing and development.
/// Returns simulated vision analysis results.
/// </summary>
public class InMemoryAiVisionProvider : IAiVisionProvider
{
    /// <inheritdoc />
    public string ProviderId => "inmemory-vision";

    /// <inheritdoc />
    public string DisplayName => "In-Memory Vision (Dev)";

    /// <inheritdoc />
    public bool SupportsStreaming => false;

    /// <inheritdoc />
    public int MaxImagesPerRequest => 10;

    /// <inheritdoc />
    public long MaxImageSizeBytes => 20 * 1024 * 1024; // 20MB

    /// <inheritdoc />
    public Task<AiVisionResponse> AnalyzeAsync(AiVisionRequest request, CancellationToken cancellationToken = default)
    {
        var response = new AiVisionResponse
        {
            Text = $"Análise de {request.Images.Count} imagem(ns): " +
                   $"Identificada(s) {request.Images.Count} imagem(ns) com resolução adequada. " +
                   $"O conteúdo mostra elementos visuais consistentes com o contexto solicitado. " +
                   $"Prompt: {request.Prompt}",
            Confidence = 0.87f,
            Detections = request.Images.Select((_, i) => new AiVisionDetection
            {
                Label = $"Objeto_{i + 1}",
                Confidence = 0.85f + (i * 0.02f),
                BoundingBox = new[] { 0.1f * i, 0.1f * i, 0.3f, 0.3f },
                Description = $"Objeto detectado na imagem {i + 1}"
            }).ToList(),
            Usage = new AiTokenUsage
            {
                PromptTokens = request.Prompt.Length / 4 + request.Images.Count * 85,
                CompletionTokens = 120
            },
            Model = "inmemory-vision-v1"
        };

        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<AiStreamChunk> AnalyzeStreamAsync(AiVisionRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var words = $"Análise visual: {request.Images.Count} imagem(ns) processada(s). Conteúdo identificado com sucesso.".Split(' ');
        foreach (var word in words)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new AiStreamChunk { Delta = word + " " };
            await Task.Delay(10, cancellationToken);
        }
        yield return new AiStreamChunk { Delta = "", FinishReason = "stop" };
    }

    /// <inheritdoc />
    public Task<AiOcrResponse> ExtractTextAsync(AiOcrRequest request, CancellationToken cancellationToken = default)
    {
        var response = new AiOcrResponse
        {
            Text = "Texto extraído via OCR (simulado):\n" +
                   "Documento fiscal detectado.\n" +
                   "CNPJ: 12.345.678/0001-90\n" +
                   "Razão Social: Empresa Exemplo LTDA\n" +
                   "Valor Total: R$ 1.234,56",
            Confidence = 0.92f,
            DetectedLanguage = request.Language ?? "pt-BR",
            Regions = new List<AiOcrRegion>
            {
                new() { Text = "CNPJ: 12.345.678/0001-90", BoundingBox = new[] { 0.1f, 0.2f, 0.8f, 0.05f }, Confidence = 0.95f },
                new() { Text = "Razão Social: Empresa Exemplo LTDA", BoundingBox = new[] { 0.1f, 0.3f, 0.8f, 0.05f }, Confidence = 0.93f },
                new() { Text = "Valor Total: R$ 1.234,56", BoundingBox = new[] { 0.1f, 0.7f, 0.8f, 0.05f }, Confidence = 0.90f }
            },
            Usage = new AiTokenUsage { PromptTokens = 50, CompletionTokens = 80 }
        };

        return Task.FromResult(response);
    }
}
