using Xunit;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for AI Service DTOs that currently have 0% coverage:
/// AiEmbeddingRequest, AiEmbeddingResponse, AiHealthStatus, TransientHttpRequestException.
/// </summary>
public sealed class XFAiServicesDtosTest
{
    #region AiEmbeddingRequest

    [Fact]
    public void AiEmbeddingRequest_RequiredInputs_AreSet()
    {
        var request = new AiEmbeddingRequest { Inputs = ["hello", "world"] };
        Assert.Equal(2, request.Inputs.Count);
        Assert.Contains("hello", request.Inputs);
        Assert.Contains("world", request.Inputs);
    }

    [Fact]
    public void AiEmbeddingRequest_WithModelAndDimensions_Roundtrips()
    {
        var request = new AiEmbeddingRequest
        {
            Inputs = ["test"],
            Model = "text-embedding-3-small",
            Dimensions = 1536
        };
        Assert.Equal("text-embedding-3-small", request.Model);
        Assert.Equal(1536, request.Dimensions);
    }

    [Fact]
    public void AiEmbeddingRequest_NullModelAndDimensions_AreAllowed()
    {
        var request = new AiEmbeddingRequest { Inputs = ["test"] };
        Assert.Null(request.Model);
        Assert.Null(request.Dimensions);
    }

    #endregion

    #region AiEmbeddingResponse

    [Fact]
    public void AiEmbeddingResponse_RequiredEmbeddings_AreSet()
    {
        var response = new AiEmbeddingResponse
        {
            Embeddings = [new float[] { 0.1f, 0.2f, 0.3f }],
            Model = "text-embedding-3-small"
        };
        Assert.Single(response.Embeddings);
        Assert.Equal(3, response.Embeddings[0].Length);
        Assert.Equal("text-embedding-3-small", response.Model);
    }

    [Fact]
    public void AiEmbeddingResponse_WithUsage_Roundtrips()
    {
        var response = new AiEmbeddingResponse
        {
            Embeddings = [new float[] { 0.1f }],
            Usage = new XForge.DesignSystem.Components.AI.Common.AiTokenUsage { PromptTokens = 10 },
            Model = "model"
        };
        Assert.NotNull(response.Usage);
        Assert.Equal(10, response.Usage.PromptTokens);
    }

    [Fact]
    public void AiEmbeddingResponse_EmptyEmbeddings_IsAllowed()
    {
        var response = new AiEmbeddingResponse { Embeddings = [] };
        Assert.Empty(response.Embeddings);
    }

    [Fact]
    public void AiEmbeddingResponse_MultipleEmbeddings_Roundtrips()
    {
        var response = new AiEmbeddingResponse
        {
            Embeddings = [new float[] { 0.1f }, new float[] { 0.2f }, new float[] { 0.3f }]
        };
        Assert.Equal(3, response.Embeddings.Count);
    }

    #endregion

    #region AiHealthStatus

    [Fact]
    public void AiHealthStatus_Healthy_HasCorrectValues()
    {
        var status = new AiHealthStatus(true, "Provider is reachable", 150);
        Assert.True(status.IsHealthy);
        Assert.Equal("Provider is reachable", status.Message);
        Assert.Equal(150, status.LatencyMs);
    }

    [Fact]
    public void AiHealthStatus_Unhealthy_HasCorrectValues()
    {
        var status = new AiHealthStatus(false, "Connection refused");
        Assert.False(status.IsHealthy);
        Assert.Equal("Connection refused", status.Message);
        Assert.Null(status.LatencyMs);
    }

    [Fact]
    public void AiHealthStatus_DefaultMessage_IsNull()
    {
        var status = new AiHealthStatus(true);
        Assert.True(status.IsHealthy);
        Assert.Null(status.Message);
        Assert.Null(status.LatencyMs);
    }

    [Fact]
    public void AiHealthStatus_RecordEquality_WorksCorrectly()
    {
        var status1 = new AiHealthStatus(true, "OK", 100);
        var status2 = new AiHealthStatus(true, "OK", 100);
        Assert.Equal(status1, status2);
        Assert.Equal(status1.GetHashCode(), status2.GetHashCode());
    }

    #endregion

    #region TransientHttpRequestException — Tested indirectly via retry infrastructure

    // TransientHttpRequestException is internal — tested indirectly through
    // the AiProviderBase retry tests in XFAiProvidersTest.
    // The exception is constructed inside EnsureSuccessWithRetryAwarenessAsync
    // and caught in ExecuteWithRetryAsync. The retry tests validate the full
    // transient error handling path including 429/500/502/503/504 responses.

    #endregion
}
