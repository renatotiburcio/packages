#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.AI.Chat;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.Insights;
using XForge.DesignSystem.Components.AI.Prompting;
#endregion

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for S-AI002 AI Core Interaction components:
/// XFAiChat, XFAiCommandPalette, XFAiPromptBox, XFAiSuggestionPanel.
/// Covers rendering, parameters, events, states, accessibility, and interaction.
/// </summary>
public class XFAiComponentsTest : BunitContext
{
    public XFAiComponentsTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region XFAiChat

    [Fact]
    public void Chat_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiChat>();

        Assert.Contains("Assistente IA", cut.Markup);
        Assert.Contains("Digite sua mensagem para a IA...", cut.Markup);
    }

    [Fact]
    public void Chat_Renders_MessagesList()
    {
        var messages = new List<AiMessage>
        {
            new() { Role = AiRole.User, Content = "Hello AI" },
            new() { Role = AiRole.Assistant, Content = "Hello! How can I help?" }
        };

        var cut = Render<XFAiChat>(p => p.Add(x => x.Messages, messages));

        Assert.Contains("Hello AI", cut.Markup);
        Assert.Contains("Hello! How can I help?", cut.Markup);
    }

    [Fact]
    public void Chat_UserMessage_HasRightAlignedStyling()
    {
        var messages = new List<AiMessage>
        {
            new() { Role = AiRole.User, Content = "User msg" }
        };

        var cut = Render<XFAiChat>(p => p.Add(x => x.Messages, messages));

        Assert.Contains("justify-end", cut.Markup);
        Assert.Contains("bg-primary", cut.Markup);
    }

    [Fact]
    public void Chat_AssistantMessage_HasLeftAlignedStyling()
    {
        var messages = new List<AiMessage>
        {
            new() { Role = AiRole.Assistant, Content = "Assistant msg" }
        };

        var cut = Render<XFAiChat>(p => p.Add(x => x.Messages, messages));

        Assert.Contains("justify-start", cut.Markup);
    }

    [Fact]
    public void Chat_ShowsHeader_WhenShowHeaderTrue()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.ShowHeader, true));

        Assert.Contains("<header", cut.Markup);
    }

    [Fact]
    public void Chat_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.ShowHeader, false));

        Assert.DoesNotContain("<header", cut.Markup);
    }

    [Fact]
    public void Chat_State_StartsAsIdle()
    {
        var cut = Render<XFAiChat>();

        Assert.Equal(XFAiChatState.Idle, cut.Instance.State);
    }

    [Fact]
    public async Task Chat_OnSend_FiresOnEnterKey()
    {
        AiChatRequest? sentRequest = null;
        var cut = Render<XFAiChat>(p =>
        {
            p.Add(x => x.OnSend, EventCallback.Factory.Create<AiChatRequest>(this, r => sentRequest = r));
        });

        cut.Find("textarea").Input("Hello AI");
        cut.Find("textarea").KeyDown("Enter");

        Assert.NotNull(sentRequest);
        Assert.Single(sentRequest!.Messages);
        Assert.Equal(AiRole.User, sentRequest.Messages[0].Role);
        Assert.Contains("Hello AI", sentRequest.Messages[0].Content);
    }

    #endregion

    #region XFAiCommandPalette

    [Fact]
    public void CommandPalette_NotVisible_WhenVisibleFalse()
    {
        var cut = Render<XFAiCommandPalette>();

        Assert.DoesNotContain("Paleta de comandos", cut.Markup);
        Assert.DoesNotContain("role=\"dialog\"", cut.Markup);
    }

    [Fact]
    public void CommandPalette_Visible_WhenVisibleTrue()
    {
        var cut = Render<XFAiCommandPalette>(p => p.Add(x => x.Visible, true));

        Assert.Contains("role=\"dialog\"", cut.Markup);
        Assert.Contains("aria-modal=\"true\"", cut.Markup);
        Assert.Contains("Paleta de comandos", cut.Markup);
    }

    [Fact]
    public void CommandPalette_Renders_CommandsList()
    {
        var commands = new List<XFAiCommandItem>
        {
            new() { Label = "Generate Report", Description = "Create a new report" },
            new() { Label = "Analyze Data", Description = "Run analysis" }
        };

        var cut = Render<XFAiCommandPalette>(p =>
        {
            p.Add(x => x.Visible, true);
            p.Add(x => x.Commands, commands);
        });

        Assert.Contains("Generate Report", cut.Markup);
        Assert.Contains("Analyze Data", cut.Markup);
    }

    [Fact]
    public void CommandPalette_SearchInput_HasPlaceholder()
    {
        var cut = Render<XFAiCommandPalette>(p => p.Add(x => x.Visible, true));

        var input = cut.Find("input");
        Assert.Equal("Pesquisar comandos ou digite /...", input.GetAttribute("placeholder"));
    }

    [Fact]
    public async Task CommandPalette_OnCommandExecute_FiresOnClick()
    {
        XFAiCommandItem? executed = null;
        var commands = new List<XFAiCommandItem>
        {
            new() { Id = "cmd1", Label = "Test Command" }
        };

        var cut = Render<XFAiCommandPalette>(p =>
        {
            p.Add(x => x.Visible, true);
            p.Add(x => x.Commands, commands);
            p.Add(x => x.OnCommandExecute, EventCallback.Factory.Create<XFAiCommandItem>(this, c => executed = c));
        });

        var option = cut.Find("[role='option']");
        await option.ClickAsync();

        Assert.NotNull(executed);
        Assert.Equal("cmd1", executed!.Id);
    }

    [Fact]
    public void CommandPalette_VisibleChanged_FiresOnEscape()
    {
        bool? visibleChanged = null;

        var cut = Render<XFAiCommandPalette>(p =>
        {
            p.Add(x => x.Visible, true);
            p.Add(x => x.VisibleChanged, EventCallback.Factory.Create<bool>(this, v => visibleChanged = v));
        });

        cut.Find("section").KeyDown("Escape");

        Assert.False(visibleChanged);
    }

    [Fact]
    public void CommandPalette_Renders_CommandDescriptions()
    {
        var commands = new List<XFAiCommandItem>
        {
            new() { Label = "Cmd", Description = "This is a detailed description" }
        };

        var cut = Render<XFAiCommandPalette>(p =>
        {
            p.Add(x => x.Visible, true);
            p.Add(x => x.Commands, commands);
        });

        Assert.Contains("This is a detailed description", cut.Markup);
    }

    [Fact]
    public void CommandPalette_Renders_SlashCommandBadge()
    {
        var commands = new List<XFAiCommandItem>
        {
            new() { Label = "Report", IsSlashCommand = true, SlashPrefix = "/report" }
        };

        var cut = Render<XFAiCommandPalette>(p =>
        {
            p.Add(x => x.Visible, true);
            p.Add(x => x.Commands, commands);
        });

        Assert.Contains("/report", cut.Markup);
    }

    #endregion

    #region XFAiPromptBox

    [Fact]
    public void PromptBox_Renders_WithPlaceholder()
    {
        var cut = Render<XFAiPromptBox>();

        Assert.Contains("Escreva seu prompt aqui...", cut.Markup);
    }

    [Fact]
    public void PromptBox_Value_RendersInTextarea()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.Value, "Test prompt value"));

        var textarea = cut.Find("textarea");
        Assert.Equal("Test prompt value", textarea.GetAttribute("value"));
    }

    [Fact]
    public void PromptBox_TokenEstimation_Displays()
    {
        // 8 chars => ceil(8/4) = 2 tokens
        var cut = Render<XFAiPromptBox>(p =>
        {
            p.Add(x => x.Value, "abcdefgh");
            p.Add(x => x.ShowTokenEstimate, true);
        });

        Assert.Contains("Tokens estimados", cut.Markup);
        Assert.Contains("xf-ai-prompt-box", cut.Markup);
    }

    [Fact]
    public void PromptBox_CharacterCount_Displays()
    {
        var cut = Render<XFAiPromptBox>(p =>
        {
            p.Add(x => x.Value, "Hello");
            p.Add(x => x.ShowCharCount, true);
        });

        // CharCount = 5, rendered in footer
        Assert.Contains("5", cut.Markup);
        Assert.Contains("aria-live=\"polite\"", cut.Markup);
    }

    [Fact]
    public void PromptBox_TemplatesButton_Renders()
    {
        var templates = new List<XFAiPromptTemplate>
        {
            new() { Name = "Email", Content = "Write an email about..." }
        };

        var cut = Render<XFAiPromptBox>(p =>
        {
            p.Add(x => x.ShowTemplates, true);
            p.Add(x => x.Templates, templates);
        });

        Assert.Contains("Templates", cut.Markup);
    }

    [Fact]
    public void PromptBox_SendButton_Renders()
    {
        var cut = Render<XFAiPromptBox>(p => p.Add(x => x.ShowSendButton, true));

        Assert.Contains("Enviar", cut.Markup);
    }

    [Fact]
    public async Task PromptBox_OnSend_FiresOnEnterKey()
    {
        string? sentValue = null;
        var cut = Render<XFAiPromptBox>(p =>
        {
            p.Add(x => x.Value, "Test prompt");
            p.Add(x => x.OnSend, EventCallback.Factory.Create<string>(this, v => sentValue = v));
        });

        cut.Find("textarea").KeyDown("Enter");

        Assert.NotNull(sentValue);
        Assert.Equal("Test prompt", sentValue);
    }

    #endregion

    #region XFAiSuggestionPanel

    [Fact]
    public void SuggestionPanel_Renders_WithTitle()
    {
        var cut = Render<XFAiSuggestionPanel>();

        Assert.Contains("Sugestoes inteligentes", cut.Markup);
    }

    [Fact]
    public void SuggestionPanel_Renders_SuggestionsList()
    {
        var suggestions = new List<XFAiSuggestion>
        {
            new() { Title = "Optimize query", Type = XFAiSuggestionType.Recommendation },
            new() { Title = "Check index", Type = XFAiSuggestionType.Action }
        };

        var cut = Render<XFAiSuggestionPanel>(p => p.Add(x => x.Suggestions, suggestions));

        Assert.Contains("Optimize query", cut.Markup);
        Assert.Contains("Check index", cut.Markup);
    }

    [Fact]
    public void SuggestionPanel_EmptyState_WhenNoSuggestions()
    {
        var cut = Render<XFAiSuggestionPanel>();

        Assert.Contains("Nenhuma sugestao no momento.", cut.Markup);
    }

    [Fact]
    public void SuggestionPanel_LoadingState_ShowsSkeleton()
    {
        var cut = Render<XFAiSuggestionPanel>(p => p.Add(x => x.Loading, true));

        Assert.Contains("aria-busy=\"true\"", cut.Markup);
        Assert.Contains("Analisando...", cut.Markup);
    }

    [Fact]
    public async Task SuggestionPanel_OnSuggestionClick_FiresOnClick()
    {
        XFAiSuggestion? clicked = null;
        var suggestions = new List<XFAiSuggestion>
        {
            new() { Id = "s1", Title = "Test Suggestion" }
        };

        var cut = Render<XFAiSuggestionPanel>(p =>
        {
            p.Add(x => x.Suggestions, suggestions);
            p.Add(x => x.OnSuggestionClick, EventCallback.Factory.Create<XFAiSuggestion>(this, s => clicked = s));
        });

        await cut.Find("[role='listitem']").ClickAsync();

        Assert.NotNull(clicked);
        Assert.Equal("s1", clicked!.Id);
    }

    [Fact]
    public void SuggestionPanel_TypeBorderClass_AppliedCorrectly()
    {
        var suggestions = new List<XFAiSuggestion>
        {
            new() { Title = "Action Item", Type = XFAiSuggestionType.Action },
            new() { Title = "Warning Item", Type = XFAiSuggestionType.Warning }
        };

        var cut = Render<XFAiSuggestionPanel>(p => p.Add(x => x.Suggestions, suggestions));

        Assert.Contains("border-l-blue-500", cut.Markup);
        Assert.Contains("border-l-amber-500", cut.Markup);
    }

    [Fact]
    public void SuggestionPanel_AriaRoles_Correct()
    {
        var suggestions = new List<XFAiSuggestion>
        {
            new() { Title = "Item 1" }
        };

        var cut = Render<XFAiSuggestionPanel>(p => p.Add(x => x.Suggestions, suggestions));

        Assert.Contains("role=\"complementary\"", cut.Markup);
        Assert.Contains("role=\"list\"", cut.Markup);
        Assert.Contains("role=\"listitem\"", cut.Markup);
    }

    #endregion
}
