using Xunit;
using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for AI Common DTOs that currently have 0% coverage.
/// Covers AiChatResponse, AiStreamChunk, AiToolCallDelta, AiToolDefinition,
/// AiSpeechRequest/Response, AiVisionRequest/Response, AiTranscriptionRequest/Response/Segment/Word.
/// </summary>
public sealed class XFAiCommonDtosTest
{
    #region AiChatResponse

    [Fact]
    public void AiChatResponse_DefaultConstruction_HasDefaults()
    {
        var response = new AiChatResponse();
        Assert.NotNull(response.Message);
        Assert.Equal(AiRole.Assistant, response.Message.Role);
        Assert.Null(response.FinishReason);
        Assert.Null(response.Usage);
        Assert.Null(response.ResponseId);
        Assert.Null(response.Duration);
    }

    [Fact]
    public void AiChatResponse_WithValues_RoundtripsCorrectly()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage { Role = AiRole.Assistant, Content = "Hello" },
            FinishReason = "stop",
            Usage = new AiTokenUsage { PromptTokens = 10, CompletionTokens = 5 },
            ResponseId = "resp-123",
            Duration = TimeSpan.FromMilliseconds(500)
        };
        Assert.Equal("Hello", response.Message.Content);
        Assert.Equal("stop", response.FinishReason);
        Assert.Equal(15, response.Usage!.TotalTokens);
        Assert.Equal("resp-123", response.ResponseId);
        Assert.Equal(500, response.Duration!.Value.TotalMilliseconds);
    }

    #endregion

    #region AiStreamChunk

    [Fact]
    public void AiStreamChunk_DefaultConstruction_HasDefaults()
    {
        var chunk = new AiStreamChunk();
        Assert.Null(chunk.Delta);
        Assert.Null(chunk.ToolCallDelta);
        Assert.Null(chunk.ReasoningDelta);
        Assert.Null(chunk.FinishReason);
        Assert.Equal(0, chunk.ChunkIndex);
    }

    [Fact]
    public void AiStreamChunk_WithValues_RoundtripsCorrectly()
    {
        var chunk = new AiStreamChunk
        {
            Delta = "Hello ",
            ToolCallDelta = new AiToolCallDelta { Id = "tc1", Name = "get_weather", ArgumentsDelta = "{\"", Index = 0 },
            ReasoningDelta = "Let me think...",
            FinishReason = null,
            ChunkIndex = 5
        };
        Assert.Equal("Hello ", chunk.Delta);
        Assert.NotNull(chunk.ToolCallDelta);
        Assert.Equal("tc1", chunk.ToolCallDelta.Id);
        Assert.Equal("Let me think...", chunk.ReasoningDelta);
        Assert.Equal(5, chunk.ChunkIndex);
    }

    [Fact]
    public void AiStreamChunk_WithFinishReason_IndicatesEnd()
    {
        var chunk = new AiStreamChunk { FinishReason = "stop", ChunkIndex = 10 };
        Assert.Equal("stop", chunk.FinishReason);
    }

    #endregion

    #region AiToolCallDelta

    [Fact]
    public void AiToolCallDelta_DefaultConstruction_HasDefaults()
    {
        var delta = new AiToolCallDelta();
        Assert.Null(delta.Id);
        Assert.Null(delta.Name);
        Assert.Null(delta.ArgumentsDelta);
        Assert.Equal(0, delta.Index);
    }

    [Fact]
    public void AiToolCallDelta_WithValues_RoundtripsCorrectly()
    {
        var delta = new AiToolCallDelta
        {
            Id = "call-abc",
            Name = "search",
            ArgumentsDelta = "{\"query\":\"test\"",
            Index = 2
        };
        Assert.Equal("call-abc", delta.Id);
        Assert.Equal("search", delta.Name);
        Assert.Equal("{\"query\":\"test\"", delta.ArgumentsDelta);
        Assert.Equal(2, delta.Index);
    }

    #endregion

    #region AiToolDefinition

    [Fact]
    public void AiToolDefinition_RequiredProperties_AreSet()
    {
        var tool = new AiToolDefinition
        {
            Name = "get_weather",
            Description = "Gets the current weather",
            ParametersSchema = "{\"type\":\"object\"}"
        };
        Assert.Equal("get_weather", tool.Name);
        Assert.Equal("Gets the current weather", tool.Description);
        Assert.Equal("{\"type\":\"object\"}", tool.ParametersSchema);
    }

    [Fact]
    public void AiToolDefinition_NullSchema_IsAllowed()
    {
        var tool = new AiToolDefinition { Name = "no_params", Description = "No parameters" };
        Assert.Null(tool.ParametersSchema);
    }

    #endregion

    #region AiToolCall

    [Fact]
    public void AiToolCall_DefaultConstruction_HasEmptyStrings()
    {
        var call = new AiToolCall();
        Assert.Equal(string.Empty, call.Id);
        Assert.Equal(string.Empty, call.Name);
        Assert.Equal(string.Empty, call.Arguments);
    }

    [Fact]
    public void AiToolCall_WithValues_RoundtripsCorrectly()
    {
        var call = new AiToolCall { Id = "tc1", Name = "search", Arguments = "{\"q\":\"test\"}" };
        Assert.Equal("tc1", call.Id);
        Assert.Equal("search", call.Name);
        Assert.Equal("{\"q\":\"test\"}", call.Arguments);
    }

    #endregion

    #region AiCitation

    [Fact]
    public void AiCitation_DefaultConstruction_HasDefaults()
    {
        var citation = new AiCitation();
        Assert.Equal(string.Empty, citation.Title);
        Assert.Equal(string.Empty, citation.Url);
        Assert.Equal(0, citation.Score);
        Assert.Null(citation.Snippet);
        Assert.Null(citation.SourceType);
    }

    [Fact]
    public void AiCitation_WithValues_RoundtripsCorrectly()
    {
        var citation = new AiCitation
        {
            Title = "RFC 1234",
            Url = "https://example.com/rfc1234",
            Score = 0.95,
            Snippet = "This is a relevant excerpt...",
            SourceType = "document"
        };
        Assert.Equal("RFC 1234", citation.Title);
        Assert.Equal("https://example.com/rfc1234", citation.Url);
        Assert.Equal(0.95, citation.Score);
        Assert.Equal("This is a relevant excerpt...", citation.Snippet);
        Assert.Equal("document", citation.SourceType);
    }

    #endregion

    #region AiSpeechRequest

    [Fact]
    public void AiSpeechRequest_RequiredText_IsSet()
    {
        var request = new AiSpeechRequest { Text = "Hello world" };
        Assert.Equal("Hello world", request.Text);
        Assert.Null(request.Voice);
        Assert.Null(request.Language);
        Assert.Null(request.Speed);
    }

    [Fact]
    public void AiSpeechRequest_WithAllOptions_RoundtripsCorrectly()
    {
        var request = new AiSpeechRequest
        {
            Text = "Olá mundo",
            Voice = "alloy",
            Language = "pt-BR",
            Speed = 1.2f
        };
        Assert.Equal("Olá mundo", request.Text);
        Assert.Equal("alloy", request.Voice);
        Assert.Equal("pt-BR", request.Language);
        Assert.Equal(1.2f, request.Speed);
    }

    #endregion

    #region AiSpeechResponse

    [Fact]
    public void AiSpeechResponse_DefaultConstruction_HasNullBytes()
    {
        var response = new AiSpeechResponse();
        Assert.Null(response.AudioBytes);
        Assert.Null(response.ContentType);
    }

    [Fact]
    public void AiSpeechResponse_WithValues_RoundtripsCorrectly()
    {
        var audioData = new byte[] { 0x01, 0x02, 0x03 };
        var response = new AiSpeechResponse
        {
            AudioBytes = audioData,
            ContentType = "audio/mp3"
        };
        Assert.Equal(audioData, response.AudioBytes);
        Assert.Equal("audio/mp3", response.ContentType);
    }

    #endregion

    #region AiVisionRequest

    [Fact]
    public void AiVisionRequest_RequiredProperties_AreSet()
    {
        var request = new AiVisionRequest
        {
            Prompt = "Describe this image",
            Images = ["data:image/png;base64,abc123"]
        };
        Assert.Equal("Describe this image", request.Prompt);
        Assert.Single(request.Images);
        Assert.Null(request.MimeTypes);
        Assert.Null(request.Temperature);
        Assert.Null(request.MaxTokens);
    }

    [Fact]
    public void AiVisionRequest_MultipleImages_Roundtrips()
    {
        var request = new AiVisionRequest
        {
            Prompt = "Compare these",
            Images = ["img1", "img2", "img3"],
            MimeTypes = ["image/png", "image/jpeg", "image/gif"],
            Temperature = 0.5,
            MaxTokens = 2048
        };
        Assert.Equal(3, request.Images.Count);
        Assert.Equal(3, request.MimeTypes!.Count);
        Assert.Equal(0.5, request.Temperature);
        Assert.Equal(2048, request.MaxTokens);
    }

    #endregion

    #region AiVisionResponse

    [Fact]
    public void AiVisionResponse_DefaultConstruction_HasDefaults()
    {
        var response = new AiVisionResponse();
        Assert.Equal(string.Empty, response.Text);
        Assert.Null(response.StructuredData);
        Assert.Null(response.Confidence);
    }

    [Fact]
    public void AiVisionResponse_WithValues_RoundtripsCorrectly()
    {
        var response = new AiVisionResponse
        {
            Text = "A cat sitting on a mat",
            StructuredData = "{\"objects\":[\"cat\",\"mat\"]}",
            Confidence = 0.92f
        };
        Assert.Equal("A cat sitting on a mat", response.Text);
        Assert.Contains("cat", response.StructuredData);
        Assert.Equal(0.92f, response.Confidence);
    }

    #endregion

    #region AiTranscriptionRequest

    [Fact]
    public void AiTranscriptionRequest_DefaultConstruction_HasDefaults()
    {
        var request = new AiTranscriptionRequest();
        Assert.Null(request.Audio);
        Assert.Null(request.AudioStream);
        Assert.Null(request.AudioBytes);
        Assert.Null(request.Language);
        Assert.Null(request.ContentType);
        Assert.False(request.IncludeTimestamps);
        Assert.Null(request.Format);
    }

    [Fact]
    public void AiTranscriptionRequest_WithValues_RoundtripsCorrectly()
    {
        var request = new AiTranscriptionRequest
        {
            Audio = "file.wav",
            Language = "en-US",
            ContentType = "audio/wav",
            IncludeTimestamps = true,
            Format = "verbose_json"
        };
        Assert.Equal("file.wav", request.Audio);
        Assert.Equal("en-US", request.Language);
        Assert.True(request.IncludeTimestamps);
        Assert.Equal("verbose_json", request.Format);
    }

    #endregion

    #region AiTranscriptionResponse

    [Fact]
    public void AiTranscriptionResponse_DefaultConstruction_HasDefaults()
    {
        var response = new AiTranscriptionResponse();
        Assert.Equal(string.Empty, response.Text);
        Assert.Null(response.DetectedLanguage);
        Assert.Null(response.DurationSeconds);
        Assert.Null(response.Segments);
    }

    [Fact]
    public void AiTranscriptionResponse_WithValues_RoundtripsCorrectly()
    {
        var response = new AiTranscriptionResponse
        {
            Text = "Hello world",
            DetectedLanguage = "en",
            DurationSeconds = 5.5f,
            Segments = [new AiTranscriptionSegment { Text = "Hello world", Start = 0, End = 5.5f }]
        };
        Assert.Equal("Hello world", response.Text);
        Assert.Equal("en", response.DetectedLanguage);
        Assert.Equal(5.5f, response.DurationSeconds);
        Assert.Single(response.Segments!);
    }

    #endregion

    #region AiTranscriptionSegment

    [Fact]
    public void AiTranscriptionSegment_DefaultConstruction_HasDefaults()
    {
        var segment = new AiTranscriptionSegment();
        Assert.Equal(string.Empty, segment.Text);
        Assert.Null(segment.Start);
        Assert.Null(segment.End);
        Assert.Null(segment.Words);
    }

    [Fact]
    public void AiTranscriptionSegment_WithWords_RoundtripsCorrectly()
    {
        var segment = new AiTranscriptionSegment
        {
            Text = "Hello world",
            Start = 0.5f,
            End = 2.3f,
            Words = [
                new AiTranscriptionWord { Text = "Hello", Start = 0.5f, End = 1.0f, Confidence = 0.99f },
                new AiTranscriptionWord { Text = "world", Start = 1.1f, End = 2.3f, Confidence = 0.95f }
            ]
        };
        Assert.Equal("Hello world", segment.Text);
        Assert.Equal(0.5f, segment.Start);
        Assert.Equal(2.3f, segment.End);
        Assert.Equal(2, segment.Words!.Count);
        Assert.Equal(0.99f, segment.Words[0].Confidence);
    }

    #endregion

    #region AiTranscriptionWord

    [Fact]
    public void AiTranscriptionWord_DefaultConstruction_HasDefaults()
    {
        var word = new AiTranscriptionWord();
        Assert.Equal(string.Empty, word.Text);
        Assert.Null(word.Start);
        Assert.Null(word.End);
        Assert.Equal(0, word.Confidence);
    }

    [Fact]
    public void AiTranscriptionWord_WithValues_RoundtripsCorrectly()
    {
        var word = new AiTranscriptionWord
        {
            Text = "test",
            Start = 1.0f,
            End = 2.0f,
            Confidence = 0.88f
        };
        Assert.Equal("test", word.Text);
        Assert.Equal(1.0f, word.Start);
        Assert.Equal(2.0f, word.End);
        Assert.Equal(0.88f, word.Confidence);
    }

    #endregion

    #region AiTokenUsage — Additional Coverage

    [Fact]
    public void AiTokenUsage_WithCostAndModel_Roundtrips()
    {
        var usage = new AiTokenUsage
        {
            PromptTokens = 100,
            CompletionTokens = 50,
            EstimatedCostUsd = 0.015m,
            Model = "gpt-4o"
        };
        Assert.Equal(150, usage.TotalTokens);
        Assert.Equal(0.015m, usage.EstimatedCostUsd);
        Assert.Equal("gpt-4o", usage.Model);
    }

    [Fact]
    public void AiTokenUsage_ZeroTokens_TotalIsZero()
    {
        var usage = new AiTokenUsage { PromptTokens = 0, CompletionTokens = 0 };
        Assert.Equal(0, usage.TotalTokens);
    }

    #endregion

    #region AiMessage — Additional Coverage

    [Fact]
    public void AiMessage_WithToolCallId_Roundtrips()
    {
        var msg = new AiMessage
        {
            Role = AiRole.Tool,
            Content = "{\"result\": 42}",
            ToolCallId = "tc_abc123"
        };
        Assert.Equal(AiRole.Tool, msg.Role);
        Assert.Equal("tc_abc123", msg.ToolCallId);
    }

    [Fact]
    public void AiMessage_Timestamp_IsAutoSet()
    {
        var before = DateTimeOffset.UtcNow;
        var msg = new AiMessage { Role = AiRole.User, Content = "test" };
        var after = DateTimeOffset.UtcNow;
        Assert.True(msg.Timestamp >= before);
        Assert.True(msg.Timestamp <= after);
    }

    [Fact]
    public void AiMessage_MultipleToolCalls_Roundtrips()
    {
        var msg = new AiMessage
        {
            Role = AiRole.Assistant,
            ToolCalls = [
                new AiToolCall { Id = "tc1", Name = "search", Arguments = "{}" },
                new AiToolCall { Id = "tc2", Name = "calculate", Arguments = "{\"x\":1}" }
            ]
        };
        Assert.Equal(2, msg.ToolCalls!.Count);
    }

    [Fact]
    public void AiMessage_MultipleCitations_Roundtrips()
    {
        var msg = new AiMessage
        {
            Role = AiRole.Assistant,
            Citations = [
                new AiCitation { Title = "Doc 1", Score = 0.9 },
                new AiCitation { Title = "Doc 2", Score = 0.8 }
            ]
        };
        Assert.Equal(2, msg.Citations!.Count);
    }

    #endregion
}
