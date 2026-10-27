using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.Prompting;
using XForge.DesignSystem.Services.AI;
using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Tests.Components.AI;

internal sealed class PromptBoxMockProvider : IAiProvider, IDisposable
{
    public string ProviderId => "mock-promptbox";
    public string DisplayName => "Mock PromptBox";
    public string? DefaultModel => "mock-model";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
        => Task.FromResult(new AiChatResponse
        {
            Message = new AiMessage { Role = AiRole.Assistant, Content = "mock response" }
        });

    public IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken ct = default)
        => AsyncEnumerable.Empty<AiStreamChunk>();

    public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default)
        => Task.FromResult(new AiEmbeddingResponse { Embeddings = Array.Empty<float[]>() });

    public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

    public void Dispose() { }
}

public sealed class XFAiPromptBoxTest : BunitContext
{
    public XFAiPromptBoxTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAiProvider>(new PromptBoxMockProvider());
    }

    [Fact] public void Renders_Default()
    {
        var cut = Render<XFAiPromptBox>();
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_Textarea()
    {
        var cut = Render<XFAiPromptBox>();
        Assert.NotNull(cut.Find("textarea"));
    }

    [Fact] public void Value_RendersInTextarea()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "Hello AI"));
        Assert.Equal("Hello AI", cut.Find("textarea").GetAttribute("value"));
    }

    [Fact] public void Value_Changed_RendersUpdated()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "initial"));
        var updatedCut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "updated"));
        Assert.Equal("updated", updatedCut.Find("textarea").GetAttribute("value"));
    }

    [Fact] public void Placeholder_CustomValue()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Placeholder, "Custom placeholder"));
        Assert.Equal("Custom placeholder", cut.Find("textarea").GetAttribute("placeholder"));
    }

    [Fact] public void Templates_WithItems_ShowsTemplateButton()
    {
        var templates = new List<XFAiPromptTemplate> { new() { Name = "T1", Content = "C1" } };
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Templates, templates));
        Assert.Contains(cut.FindAll("button"), b => b.TextContent!.Contains("Templates"));
    }

    [Fact] public void ShowTemplates_False_HidesTemplateButton()
    {
        var templates = new List<XFAiPromptTemplate> { new() { Name = "T1", Content = "C1" } };
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Templates, templates).Add(x => x.ShowTemplates, false));
        Assert.DoesNotContain(cut.FindAll("button"), b => b.TextContent!.Contains("Templates"));
    }

    [Fact] public void ShowTokenEstimate_True_ShowsTokenCount()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.ShowTokenEstimate, true));
        Assert.Contains("Tokens estimados", cut.Markup);
    }

    [Fact] public void ShowTokenEstimate_False_HidesTokenCount()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.ShowTokenEstimate, false));
        Assert.DoesNotContain("Tokens estimados", cut.Markup);
    }

    [Fact] public void ShowCharCount_True_ShowsCharCount()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "abc").Add(x => x.ShowCharCount, true));
        Assert.Contains("3", cut.Markup);
    }

    [Fact] public void MaxTokens_ShowsLimitInFooter()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.MaxTokens, 100).Add(x => x.ShowTokenEstimate, true));
        Assert.Contains("/ 100", cut.Markup);
    }

    [Fact] public void MaxLength_ShowsLimitInFooter()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.MaxLength, 500).Add(x => x.ShowCharCount, true));
        Assert.Contains("/ 500", cut.Markup);
    }

    [Fact] public void Rows_CustomValue()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Rows, 8));
        Assert.Equal("8", cut.Find("textarea").GetAttribute("rows"));
    }

    [Fact] public void ReadOnly_True_TextareaReadonly()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.ReadOnly, true));
        Assert.True(cut.Find("textarea").HasAttribute("readonly"));
    }

    [Fact] public void Disabled_True_TextareaDisabled()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("textarea").HasAttribute("disabled"));
    }

    [Fact] public void ShowSendButton_True_ShowsSendButton()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.ShowSendButton, true));
        Assert.Contains(cut.FindAll("button"), b => b.GetAttribute("aria-label") == "Enviar");
    }

    [Fact] public void ShowSendButton_False_HidesSendButton()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.ShowSendButton, false));
        Assert.DoesNotContain(cut.FindAll("button"), b => b.GetAttribute("aria-label") == "Enviar");
    }

    [Fact] public void Class_CustomValue()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Class, "my-class"));
        Assert.Contains("my-class", cut.Find(".xf-ai-prompt-box").GetAttribute("class"));
    }

    [Fact] public void Id_CustomValue()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Id, "my-prompt-box"));
        Assert.NotNull(cut.Find("#my-prompt-box"));
    }

    [Fact] public void OnSend_Fires_WhenEnterPressed()
    {
        string? sentValue = null;
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "test prompt").Add(x => x.OnSend, EventCallback.Factory.Create<string>(this, val => sentValue = val)));
        cut.Find("textarea").KeyDown("Enter");
        Assert.Equal("test prompt", sentValue);
    }

    [Fact] public void OnSend_Fires_WhenSendButtonClicked()
    {
        string? sentValue = null;
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "click prompt").Add(x => x.ShowSendButton, true).Add(x => x.OnSend, EventCallback.Factory.Create<string>(this, val => sentValue = val)));
        cut.FindAll("button").First(b => b.GetAttribute("aria-label") == "Enviar").Click();
        Assert.Equal("click prompt", sentValue);
    }

    [Fact] public void OnSend_NotFired_WhenDisabled()
    {
        var count = 0;
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "test").Add(x => x.Disabled, true).Add(x => x.OnSend, EventCallback.Factory.Create<string>(this, _ => count++)));
        cut.Find("textarea").KeyDown("Enter");
        Assert.Equal(0, count);
    }

    [Fact] public void OnSend_NotFired_WhenEmpty()
    {
        var count = 0;
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "").Add(x => x.OnSend, EventCallback.Factory.Create<string>(this, _ => count++)));
        cut.Find("textarea").KeyDown("Enter");
        Assert.Equal(0, count);
    }

    [Fact] public void OnSend_TrimsValue()
    {
        string? sentValue = null;
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "  trimmed  ").Add(x => x.OnSend, EventCallback.Factory.Create<string>(this, val => sentValue = val)));
        cut.Find("textarea").KeyDown("Enter");
        Assert.Equal("trimmed", sentValue);
    }

    [Fact] public void ShiftEnter_DoesNotSend()
    {
        var count = 0;
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "test").Add(x => x.OnSend, EventCallback.Factory.Create<string>(this, _ => count++)));
        var textarea = cut.Find("textarea");
        textarea.KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Enter", ShiftKey = true });
        Assert.Equal(0, count);
    }

    [Fact] public void TemplateSelect_UpdatesValue()
    {
        var templates = new List<XFAiPromptTemplate> { new() { Name = "T1", Content = "Template content" } };
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Templates, templates));
        var tmplBtn = cut.FindAll("button").First(b => b.TextContent!.Contains("Templates"));
        tmplBtn.Click();
        var option = cut.Find("[role=\"option\"]");
        option.Click();
        Assert.Equal("Template content", cut.Find("textarea").GetAttribute("value"));
    }

    [Fact] public void TemplateSelect_ClosesDropdown()
    {
        var templates = new List<XFAiPromptTemplate> { new() { Name = "T1", Content = "C1" } };
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Templates, templates));
        var tmplBtn = cut.FindAll("button").First(b => b.TextContent!.Contains("Templates"));
        tmplBtn.Click();
        Assert.Contains("role=\"listbox\"", cut.Markup);
        cut.Find("[role=\"option\"]").Click();
        Assert.DoesNotContain("role=\"listbox\"", cut.Markup);
    }

    [Fact] public void Templates_WithCategories_GroupsCorrectly()
    {
        var templates = new List<XFAiPromptTemplate>
        {
            new() { Name = "T1", Content = "C1", Category = "CatA" },
            new() { Name = "T2", Content = "C2", Category = null }
        };
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Templates, templates));
        cut.FindAll("button").First(b => b.TextContent!.Contains("Templates")).Click();
        Assert.Contains("CatA", cut.Markup);
        Assert.Contains("Outros", cut.Markup);
    }

    [Fact] public void SendButton_Disabled_WhenCannotSend()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "").Add(x => x.ShowSendButton, true));
        Assert.True(cut.FindAll("button").First(b => b.GetAttribute("aria-label") == "Enviar").HasAttribute("disabled"));
    }

    [Fact] public void SendButton_Enabled_WhenCanSend()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "valid").Add(x => x.ShowSendButton, true));
        Assert.False(cut.FindAll("button").First(b => b.GetAttribute("aria-label") == "Enviar").HasAttribute("disabled"));
    }

    [Fact] public void MultipleTemplates_AllRendered()
    {
        var templates = Enumerable.Range(1, 5).Select(i => new XFAiPromptTemplate { Name = $"T{i}", Content = $"C{i}" }).ToList();
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Templates, templates));
        cut.FindAll("button").First(b => b.TextContent!.Contains("Templates")).Click();
        Assert.Equal(5, cut.FindAll("[role=\"option\"]").Count);
    }

    [Fact] public void NullTemplates_RendersWithoutError()
    {
        var cut = Render<XFAiPromptBox>();
        Assert.NotNull(cut);
    }

    [Fact] public void EmptyTemplates_NoTemplateButton()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Templates, Array.Empty<XFAiPromptTemplate>()));
        Assert.DoesNotContain(cut.FindAll("button"), b => b.TextContent!.Contains("Templates"));
    }

    [Fact] public void LargeValue_RendersWithoutError()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, new string('x', 10000)));
        Assert.NotNull(cut);
    }

    [Fact] public void OnSend_NoDelegate_DoesNotThrow()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "test"));
        cut.Find("textarea").KeyDown("Enter");
        Assert.NotNull(cut);
    }

    [Fact] public void OnTemplateSelect_NoDelegate_DoesNotThrow()
    {
        var templates = new List<XFAiPromptTemplate> { new() { Name = "T1", Content = "C1" } };
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Templates, templates));
        var tmplBtn = cut.FindAll("button").First(b => b.TextContent!.Contains("Templates"));
        tmplBtn.Click();
        cut.Find("[role=\"option\"]").Click();
        Assert.NotNull(cut);
    }

    [Fact] public void Textarea_HasAriaLabel()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Placeholder, "My placeholder"));
        Assert.Equal("My placeholder", cut.Find("textarea").GetAttribute("aria-label"));
    }

    [Fact] public void Root_HasAriaBusy()
    {
        var cut = Render<XFAiPromptBox>();
        Assert.Equal("false", cut.Find(".xf-ai-prompt-box").GetAttribute("aria-busy"));
    }

    [Fact] public void TemplateButton_HasAriaExpanded()
    {
        var templates = new List<XFAiPromptTemplate> { new() { Name = "T1", Content = "C1" } };
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Templates, templates));
        var tmplBtn = cut.FindAll("button").First(b => b.TextContent!.Contains("Templates"));
        cut.InvokeAsync(() => tmplBtn.Click());
        var updatedBtn = cut.FindAll("button").First(b => b.TextContent!.Contains("Templates"));
        Assert.True(updatedBtn.HasAttribute("aria-expanded"));
    }

    [Fact] public void TokenCount_HasAriaLivePolite()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.ShowTokenEstimate, true));
        Assert.NotNull(cut.Find("[aria-live=\"polite\"]"));
    }
}
