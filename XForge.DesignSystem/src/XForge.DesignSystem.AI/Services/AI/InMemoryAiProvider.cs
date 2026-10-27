using System.Runtime.CompilerServices;
using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// In-memory implementation of IAiProvider for testing, development, and docs demos.
/// Returns simulated chat responses without requiring a real AI backend.
/// </summary>
public class InMemoryAiProvider : IAiProvider
{
    /// <inheritdoc />
    public string ProviderId => "inmemory";

    /// <inheritdoc />
    public string DisplayName => "In-Memory (Dev)";

    /// <inheritdoc />
    public bool SupportsStreaming => true;

    /// <inheritdoc />
    public bool SupportsToolCalling => true;

    /// <inheritdoc />
    public string? DefaultModel => "inmemory-v1";

    /// <inheritdoc />
    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var lastMessage = request.Messages.LastOrDefault();
        var userText = lastMessage?.Content ?? string.Empty;

        var response = new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = GenerateResponse(userText, request.SystemPrompt),
                Timestamp = DateTimeOffset.UtcNow
            },
            FinishReason = "stop",
            Usage = new AiTokenUsage
            {
                PromptTokens = request.Messages.Sum(m => m.Content.Length / 4),
                CompletionTokens = 60
            },
            ResponseId = $"inmemory-{Guid.NewGuid():N}",
            Duration = TimeSpan.FromMilliseconds(150)
        };

        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var lastMessage = request.Messages.LastOrDefault();
        var userText = lastMessage?.Content ?? string.Empty;
        var fullResponse = GenerateResponse(userText, request.SystemPrompt);
        var words = fullResponse.Split(' ');

        for (var i = 0; i < words.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var chunk = new AiStreamChunk
            {
                Delta = i == 0 ? words[i] : " " + words[i],
                ChunkIndex = i
            };
            yield return chunk;
            await Task.Delay(20, cancellationToken);
        }

        yield return new AiStreamChunk
        {
            Delta = "",
            FinishReason = "stop",
            ChunkIndex = words.Length
        };
    }

    /// <inheritdoc />
    public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken cancellationToken = default)
    {
        // Generate deterministic pseudo-embeddings for dev/demo
        var embeddings = request.Inputs.Select(text =>
        {
            var vec = new float[384];
            for (var i = 0; i < vec.Length; i++)
            {
                vec[i] = (float)(Math.Sin(text.GetHashCode() * (i + 1) * 0.01) * 0.5);
            }
            return vec;
        }).ToList();

        return Task.FromResult(new AiEmbeddingResponse
        {
            Embeddings = embeddings,
            Usage = new AiTokenUsage
            {
                PromptTokens = request.Inputs.Sum(t => t.Length / 4),
                CompletionTokens = 0
            },
            Model = "inmemory-embedding-v1"
        });
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<string> models = ["inmemory-v1", "inmemory-fast", "inmemory-embedding-v1"];
        return Task.FromResult(models);
    }

    private static string GenerateResponse(string userText, string? systemPrompt)
    {
        // Generate contextual mock responses based on input
        var lower = userText.ToLowerInvariant();

        if (lower.Contains("olá") || lower.Contains("ola") || lower.Contains("hello") || lower.Contains("hi"))
            return "Olá! Sou o assistente XForge. Como posso ajudar você hoje?";

        if (lower.Contains("nfe") || lower.Contains("nota fiscal") || lower.Contains("fiscal"))
            return "Posso ajudar com análise de NF-e. Os principais pontos a verificar são: emissão dentro do prazo, valores corretos dos itens, CFOP adequado à operação, e créditos de ICMS/PIS/COFINS aplicáveis.";

        if (lower.Contains("estoque") || lower.Contains("stock") || lower.Contains("inventário"))
            return "Para gestão de estoque, recomendo verificar: itens abaixo do estoque mínimo, produtos com giro lento, divergências de inventário, e pontos de reposição otimizados.";

        if (lower.Contains("relatório") || lower.Contains("report") || lower.Contains("dashboard"))
            return "Posso gerar relatórios personalizados. Especifique o período, métricas desejadas e formato de saída. Relatórios disponíveis: vendas, financeiro, estoque, fiscal e operacional.";

        if (lower.Contains("sql") || lower.Contains("query") || lower.Contains("consulta"))
            return "Para consultas SQL, posso ajudar a construir queries otimizadas. Especifique a tabela, campos desejados e filtros. Recomendo sempre usar índices adequados e evitar SELECT *.";

        if (lower.Contains("obrigad") || lower.Contains("thanks") || lower.Contains("valeu"))
            return "De nada! Estou sempre disponível para ajudar. Não hesite em perguntar se precisar de algo mais.";

        if (string.IsNullOrWhiteSpace(userText))
            return "Olá! Sou o assistente XForge. Digite sua pergunta ou solicitação para começar.";

        // Default contextual response
        return $"Entendi sua solicitação sobre \"{Truncate(userText, 50)}\". " +
               "Com base nos dados disponíveis, posso fornecer uma análise detalhada. " +
               "Deseja que eu aprofunde em algum ponto específico?";
    }

    private static string Truncate(string text, int maxLength)
        => text.Length <= maxLength ? text : text[..maxLength] + "...";
}
