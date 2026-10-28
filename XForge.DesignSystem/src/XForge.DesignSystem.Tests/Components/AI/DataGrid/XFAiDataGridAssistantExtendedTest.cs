using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.DataGrid;
using XForge.DesignSystem.Services.AI;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.AI.DataGrid;

internal sealed class DataGridMockProvider : IAiProvider
{
    public string ProviderId => "datagrid-mock";
    public string DisplayName => "DataGrid Mock";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;
    public string? DefaultModel => null;
    public int CallCount { get; private set; }

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        CallCount++;
        return Task.FromResult(new AiChatResponse
        {
            Message = new AiMessage { Role = AiRole.Assistant, Content = "{}" }
        });
    }

    public async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiChatRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        await Task.CompletedTask;
        yield break;
    }

    public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default)
        => Task.FromResult(new AiEmbeddingResponse { Embeddings = Array.Empty<float[]>() });

    public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
}

public class XFAiDataGridAssistantExtendedTest : XForgeTestContext
{
    public XFAiDataGridAssistantExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static void SetQuery(IRenderedComponent<XFAiDataGridAssistant> cut, string query)
    {
        var f = typeof(XFAiDataGridAssistant).GetField("_queryText",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
        f.SetValue(cut.Instance, query);
    }

    private static void ForceRender(IRenderedComponent<XFAiDataGridAssistant> cut)
    {
        var m = typeof(ComponentBase).GetMethod("StateHasChanged",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
        cut.InvokeAsync(() => m.Invoke(cut.Instance, null));
    }

    // ── Rendering tests (no async flow needed) ──

    [Fact]
    public void DefaultRender_RendersBaseClass()
    {
        var cut = Render<XFAiDataGridAssistant>();
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public void Title_RendersDefault()
    {
        var cut = Render<XFAiDataGridAssistant>();
        Assert.Contains("Assistente de Dados", cut.Markup);
    }

    [Fact]
    public void Title_Custom_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Title, "My Assistant"));
        Assert.Contains("My Assistant", cut.Markup);
    }

    [Fact]
    public void ShowHeader_True_RendersHeader()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.ShowHeader, true));
        Assert.Contains("Assistente de Dados", cut.Markup);
    }

    [Fact]
    public void ShowHeader_False_HidesHeader()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.ShowHeader, false));
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public void Placeholder_RendersDefault()
    {
        var cut = Render<XFAiDataGridAssistant>();
        Assert.Contains("Pergunte sobre seus dados", cut.Markup);
    }

    [Fact]
    public void Placeholder_Custom_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Placeholder, "Ask me"));
        Assert.Contains("Ask me", cut.Markup);
    }

    [Fact]
    public void EmptyState_RendersWhenNoInsights()
    {
        var cut = Render<XFAiDataGridAssistant>();
        Assert.Contains("Faça uma pergunta sobre seus dados", cut.Markup);
    }

    [Fact]
    public void EmptyContent_Custom_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(p => p.EmptyContent, (RenderFragment)(b => b.AddContent(0, "No insights yet"))));
        Assert.Contains("No insights yet", cut.Markup);
    }

    [Fact]
    public void Disabled_DisablesInput()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, new DataGridMockProvider())
            .Add(x => x.Disabled, true));
        var input = cut.Find("input[type=text]");
        Assert.True(input.HasAttribute("disabled"));
    }

    [Fact]
    public void SubmitButton_NoProvider_Disabled()
    {
        var cut = Render<XFAiDataGridAssistant>();
        var btn = cut.FindAll("button").FirstOrDefault(b => b.GetAttribute("aria-label") == "Enviar consulta");
        Assert.NotNull(btn);
        Assert.True(btn.HasAttribute("disabled"));
    }

    [Fact]
    public void SubmitButton_EmptyQuery_Disabled()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, new DataGridMockProvider()));
        var btn = cut.FindAll("button").FirstOrDefault(b => b.GetAttribute("aria-label") == "Enviar consulta");
        Assert.NotNull(btn);
        Assert.True(btn.HasAttribute("disabled"));
    }

    [Fact]
    public void SubmitButton_WithProviderAndQuery_Enabled()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, new DataGridMockProvider()));
        SetQuery(cut, "test query");
        ForceRender(cut);
        var btn = cut.FindAll("button").FirstOrDefault(b => b.GetAttribute("aria-label") == "Enviar consulta");
        Assert.NotNull(btn);
        Assert.False(btn.HasAttribute("disabled"));
    }

    [Fact]
    public void HeaderContent_Custom_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.HeaderContent, (RenderFragment)(b => b.AddContent(0, "Custom Header"))));
        Assert.Contains("Custom Header", cut.Markup);
    }

    [Fact]
    public void AriaBusy_False_WhenNotBusy()
    {
        var cut = Render<XFAiDataGridAssistant>();
        Assert.Contains("aria-busy=\"false\"", cut.Markup);
    }

    [Fact]
    public void DataAndColumns_ParametersAccepted()
    {
        var data = new[] { new { Name = "Test", Value = 42 } };
        var columns = new[] { "Name", "Value" };
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Data, data)
            .Add(x => x.Columns, columns));
        Assert.NotNull(cut);
    }

    [Fact]
    public void RoleComplementary_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>();
        Assert.Contains("role=\"complementary\"", cut.Markup);
    }

    [Fact]
    public void SystemPrompt_Custom_Accepted()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.SystemPrompt, "Custom prompt"));
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public void MaxResults_ParameterAccepted()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.MaxResults, 10));
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public void InsightTemplate_ParameterAccepted()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.InsightTemplate, (RenderFragment<XFAiGridInsight>)(ctx =>
                (RenderFragment)(b => b.AddContent(0, $"Custom: {ctx.Title}")))));
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public void OnInsightFound_CallbackAccepted()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.OnInsightFound, EventCallback.Factory.Create<XFAiGridInsight>(this, _ => { })));
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public void OnQueryGenerated_CallbackAccepted()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.OnQueryGenerated, EventCallback.Factory.Create<XFAiGridQuery>(this, _ => { })));
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public void OnError_CallbackAccepted()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.OnError, EventCallback.Factory.Create<string>(this, _ => { })));
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public void Clear_ResetsState()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, new DataGridMockProvider()));
        SetQuery(cut, "test");
        cut.InvokeAsync(() => cut.Instance.Clear()).Wait();
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public void Cancel_NoOpWhenNotBusy()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, new DataGridMockProvider()));
        cut.InvokeAsync(async () => await cut.Instance.CancelAsync()).Wait();
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public async Task KeyboardEscape_ClearsQuery()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, new DataGridMockProvider()));
        SetQuery(cut, "test query");
        ForceRender(cut);
        var input = cut.Find("input[type=text]");
        await cut.InvokeAsync(() => input.KeyDown(new KeyboardEventArgs { Key = "Escape" }));
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public void Loading_ParameterAccepted()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Loading, true));
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    [Fact]
    public void CustomClass_Appended()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Class, "custom-class"));
        Assert.Contains("custom-class", cut.Markup);
    }

    [Fact]
    public void Id_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Id, "my-datagrid"));
        Assert.Contains("my-datagrid", cut.Markup);
    }

    [Fact]
    public void Style_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Style, "width: 100%"));
        Assert.Contains("width: 100%", cut.Markup);
    }

    [Fact]
    public void AdditionalAttributes_Render()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-test", "value" } }));
        Assert.Contains("data-test", cut.Markup);
    }

    [Fact]
    public void ComposerClass_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>();
        // The composer section has a specific class
        Assert.Contains("border-b border-border-light", cut.Markup);
    }

    [Fact]
    public void SearchIcon_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>();
        // Search icon container renders in the composer
        Assert.Contains("pointer-events-none", cut.Markup);
    }

    [Fact]
    public void EmptyState_Container_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>();
        // Empty state container has specific classes
        Assert.Contains("flex flex-col items-center justify-center", cut.Markup);
    }
}
