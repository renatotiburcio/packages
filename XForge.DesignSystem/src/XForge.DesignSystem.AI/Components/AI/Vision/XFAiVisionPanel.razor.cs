using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Vision;

/// <summary>
/// Enterprise AI vision analysis panel. Supports image upload, OCR, object detection,
/// chart analysis, and document reading via IAiVisionProvider.
/// </summary>
public partial class XFAiVisionPanel : XForgeComponentBase, IDisposable
{
    private readonly List<string> _imageBase64List = new();
    private readonly List<string> _imagePreviews = new();
    private string _prompt = "";
    private XFAiVisionAnalysisType _selectedType = XFAiVisionAnalysisType.Describe;
    private XFAiVisionResult? _result;
    private string? _error;
    private bool _isLoading;

    private static readonly Dictionary<XFAiVisionAnalysisType, string> _analysisTypes = new()
    {
        [XFAiVisionAnalysisType.Describe] = "Descrever",
        [XFAiVisionAnalysisType.Ocr] = "OCR",
        [XFAiVisionAnalysisType.DetectObjects] = "Objetos",
        [XFAiVisionAnalysisType.AnalyzeChart] = "Gráfico",
        [XFAiVisionAnalysisType.ReadDocument] = "Documento",
        [XFAiVisionAnalysisType.Compare] = "Comparar"
    };

    [Inject] public IAiVisionProvider? VisionProvider { get; set; }

    /// <summary>Title displayed in the header.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Whether to show the header.</summary>
    [Parameter] public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the analysis type selector.</summary>
    [Parameter] public bool ShowAnalysisTypeSelector { get; set; } = true;

    /// <summary>Placeholder text for the prompt input.</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>Text for the analyze button.</summary>
    [Parameter] public string? AnalyzeText { get; set; }

    /// <summary>Text shown during loading.</summary>
    [Parameter] public string? LoadingText { get; set; }

    /// <summary>Text for the drop zone.</summary>
    [Parameter] public string? DropText { get; set; }

    /// <summary>Maximum number of images.</summary>
    [Parameter] public int MaxImages { get; set; } = 5;

    /// <summary>ARIA label for the panel.</summary>
    [Parameter] public string AriaLabel { get; set; } = "Painel de análise visual AI";

    /// <summary>Custom header content.</summary>
    [Parameter] public RenderFragment? HeaderContent { get; set; }

    /// <summary>Custom result content.</summary>
    [Parameter] public RenderFragment? ResultContent { get; set; }

    /// <summary>Callback when analysis completes.</summary>
    [Parameter] public EventCallback<XFAiVisionResult> OnAnalysisComplete { get; set; }

    /// <summary>Callback when an error occurs.</summary>
    [Parameter] public EventCallback<string> OnError { get; set; }

    /// <summary>CSS class for confidence bar colors.</summary>
    private static string GetConfidenceColor(float confidence) => confidence switch
    {
        >= 0.8f => "bg-green-500",
        >= 0.6f => "bg-yellow-500",
        >= 0.4f => "bg-orange-500",
        _ => "bg-red-500"
    };

    private void SelectAnalysisType(XFAiVisionAnalysisType type)
    {
        _selectedType = type;
    }

    private void RemoveImage(int index)
    {
        if (index >= 0 && index < _imagePreviews.Count)
        {
            _imagePreviews.RemoveAt(index);
            _imageBase64List.RemoveAt(index);
        }
    }

    private void TriggerFileInput()
    {
        // File input will be handled by JavaScript interop in production
        // For now, this is a placeholder for the UI action
    }

    /// <summary>
    /// Adds an image to the analysis queue (base64 data).
    /// </summary>
    public void AddImage(string base64Data, string? previewUrl = null)
    {
        _imageBase64List.Add(base64Data);
        _imagePreviews.Add(previewUrl ?? $"data:image/png;base64,{base64Data[..Math.Min(100, base64Data.Length)]}");
        StateHasChanged();
    }

    /// <summary>
    /// Clears all images and results.
    /// </summary>
    public void Clear()
    {
        _imageBase64List.Clear();
        _imagePreviews.Clear();
        _result = null;
        _error = null;
        _prompt = "";
        StateHasChanged();
    }

    private async Task AnalyzeAsync()
    {
        if (VisionProvider == null || _imageBase64List.Count == 0 || string.IsNullOrWhiteSpace(_prompt))
            return;

        _isLoading = true;
        _error = null;
        _result = null;
        StateHasChanged();

        try
        {
            var enhancedPrompt = _selectedType switch
            {
                XFAiVisionAnalysisType.Ocr => $"Extraia todo o texto desta imagem. {_prompt}",
                XFAiVisionAnalysisType.DetectObjects => $"Detecte e classifique todos os objetos nesta imagem. {_prompt}",
                XFAiVisionAnalysisType.AnalyzeChart => $"Analise este gráfico/chart e extraia os dados. {_prompt}",
                XFAiVisionAnalysisType.ReadDocument => $"Leia este documento fiscal/comercial e extraia os dados. {_prompt}",
                XFAiVisionAnalysisType.Compare => $"Compare as imagens fornecidas. {_prompt}",
                _ => _prompt
            };

            var request = new AiVisionRequest
            {
                Prompt = enhancedPrompt,
                Images = _imageBase64List,
                MaxTokens = 1024
            };

            var response = await VisionProvider.AnalyzeAsync(request);

            _result = new XFAiVisionResult
            {
                Text = response.Text,
                StructuredData = response.StructuredData,
                Confidence = response.Confidence,
                AnalysisType = _selectedType,
                ImageCount = _imageBase64List.Count
            };

            await OnAnalysisComplete.InvokeAsync(_result);
        }
        catch (Exception ex)
        {
            _error = $"Erro na análise: {ex.Message}";
            await OnError.InvokeAsync(_error);
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        // No unmanaged resources to dispose
    }
}
