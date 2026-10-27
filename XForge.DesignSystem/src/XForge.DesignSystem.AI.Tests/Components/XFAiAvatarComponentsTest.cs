using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.AI.Avatars;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.AI;

public class XFAiAvatarComponentsTest : BunitContext
{
    #region XFAiAvatar

    [Fact]
    public void Avatar_RendersDefault()
    {
        var cut = Render<XFAiAvatar>();

        Assert.Contains("xf-ai-avatar", cut.Markup);
        Assert.Contains("role=\"img\"", cut.Markup);
        Assert.Contains("aria-label=\"Agente IA\"", cut.Markup);
    }

    [Fact]
    public void Avatar_ShowsInitials()
    {
        var cut = Render<XFAiAvatar>(p => p.Add(x => x.Name, "Maria Silva"));

        Assert.Contains("MS", cut.Markup);
    }

    [Fact]
    public void Avatar_ShowsStatusIndicator()
    {
        var cut = Render<XFAiAvatar>(p => p.Add(x => x.Status, XFAiAvatarStatus.Online));

        Assert.Contains("bg-green-500", cut.Markup);
    }

    [Fact]
    public void Avatar_SizeVariants()
    {
        var sm = Render<XFAiAvatar>(p => p.Add(x => x.Size, XFExtendedSize.Sm));
        Assert.Contains("h-8 w-8", sm.Markup);

        var lg = Render<XFAiAvatar>(p => p.Add(x => x.Size, XFExtendedSize.Lg));
        Assert.Contains("h-14 w-14", lg.Markup);

        var xl = Render<XFAiAvatar>(p => p.Add(x => x.Size, XFExtendedSize.Xl));
        Assert.Contains("h-20 w-20", xl.Markup);
    }

    [Fact]
    public void Avatar_Interactive_ClickFires()
    {
        XFAiAvatarClickEventArgs? captured = null;

        var cut = Render<XFAiAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "Agent");
            parameters.Add(p => p.Interactive, true);
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<XFAiAvatarClickEventArgs>(this, e => captured = e));
        });

        cut.Find(".xf-ai-avatar").Click();

        Assert.NotNull(captured);
        Assert.Equal("Agent", captured!.AgentName);
    }

    #endregion

    #region XFAiAgentPersonality

    [Fact]
    public void Personality_RendersDefault()
    {
        var cut = Render<XFAiAgentPersonality>();

        Assert.Contains("xf-ai-agent-personality", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Agente IA", cut.Markup);
    }

    [Fact]
    public void Personality_ShowsNameAndDescription()
    {
        var personality = new XFAiAgentPersonalityConfig
        {
            Name = "Assistente Fiscal",
            Description = "Especialista em NFe e SPED",
            Tone = XFAiAgentTone.Formal,
            Expertise = XFAiAgentExpertise.Fiscal,
            Greeting = "Olá! Como posso ajudar?",
            SuggestedPrompts = new[] { "Validar NFe", "Gerar SPED" }
        };

        var cut = Render<XFAiAgentPersonality>(p => p.Add(x => x.Personality, personality));

        Assert.Contains("Assistente Fiscal", cut.Markup);
        Assert.Contains("Especialista em NFe e SPED", cut.Markup);
    }

    [Fact]
    public void Personality_ShowsToneAndExpertiseBadges()
    {
        var personality = new XFAiAgentPersonalityConfig
        {
            Name = "Assistente Fiscal",
            Description = "Especialista em NFe e SPED",
            Tone = XFAiAgentTone.Formal,
            Expertise = XFAiAgentExpertise.Fiscal,
            Greeting = "Olá! Como posso ajudar?",
            SuggestedPrompts = new[] { "Validar NFe", "Gerar SPED" }
        };

        var cut = Render<XFAiAgentPersonality>(p => p.Add(x => x.Personality, personality));

        Assert.Contains("Formal", cut.Markup);
        Assert.Contains("Fiscal", cut.Markup);
    }

    [Fact]
    public void Personality_SuggestedPrompt_ClickFires()
    {
        string? captured = null;
        var personality = new XFAiAgentPersonalityConfig
        {
            Name = "Assistente Fiscal",
            Description = "Especialista em NFe e SPED",
            Tone = XFAiAgentTone.Formal,
            Expertise = XFAiAgentExpertise.Fiscal,
            Greeting = "Olá! Como posso ajudar?",
            SuggestedPrompts = new[] { "Validar NFe", "Gerar SPED" }
        };

        var cut = Render<XFAiAgentPersonality>(parameters =>
        {
            parameters.Add(p => p.Personality, personality);
            parameters.Add(p => p.OnSuggestedPrompt, EventCallback.Factory.Create<string>(this, s => captured = s));
        });

        cut.Find("button:first-child").Click();

        Assert.NotNull(captured);
        Assert.Equal("Validar NFe", captured);
    }

    #endregion

    #region XFAiConversationContext

    [Fact]
    public void Context_RendersDefault()
    {
        var cut = Render<XFAiConversationContext>();

        Assert.Contains("xf-ai-conversation-context", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Contexto da Conversa", cut.Markup);
    }

    [Fact]
    public void Context_ShowsEntries()
    {
        var entries = new List<XFAiContextEntry>
        {
            new()
            {
                Type = XFAiContextEntryType.Preference,
                Content = "Usuário prefere visualização em tabela",
                Source = "ai"
            }
        };

        var cut = Render<XFAiConversationContext>(p => p.Add(x => x.Entries, entries));

        Assert.Contains("Usuário prefere visualização em tabela", cut.Markup);
    }

    [Fact]
    public void Context_ShowsTypeFilterChips()
    {
        var entries = new List<XFAiContextEntry>
        {
            new()
            {
                Type = XFAiContextEntryType.Preference,
                Content = "Preferência de tema",
                Source = "ai"
            },
            new()
            {
                Type = XFAiContextEntryType.Fact,
                Content = "Fato relevante",
                Source = "system"
            }
        };

        var cut = Render<XFAiConversationContext>(p => p.Add(x => x.Entries, entries));

        Assert.Contains("Todos", cut.Markup);
        Assert.Contains("Preferência", cut.Markup);
        Assert.Contains("Fato", cut.Markup);
    }

    [Fact]
    public void Context_EmptyText_Shown()
    {
        var cut = Render<XFAiConversationContext>();

        Assert.Contains("Nenhum contexto capturado ainda.", cut.Markup);
    }

    #endregion

    #region XFAiProactiveInsight

    [Fact]
    public void Insight_RendersDefault()
    {
        var cut = Render<XFAiProactiveInsight>();

        Assert.Contains("xf-ai-proactive-insight", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Insights Proativos", cut.Markup);
    }

    [Fact]
    public void Insight_ShowsInsights()
    {
        var insights = new List<XFAiProactiveInsightItem>
        {
            new()
            {
                Type = XFAiInsightType.Suggestion,
                Title = "Otimizar consultas",
                Description = "3 queries podem ser indexadas",
                ActionLabel = "Ver detalhes",
                Priority = XFAiInsightPriority.Medium
            }
        };

        var cut = Render<XFAiProactiveInsight>(p => p.Add(x => x.Insights, insights));

        Assert.Contains("Otimizar consultas", cut.Markup);
        Assert.Contains("3 queries podem ser indexadas", cut.Markup);
    }

    [Fact]
    public void Insight_DismissButton_Renders()
    {
        var insights = new List<XFAiProactiveInsightItem>
        {
            new()
            {
                Type = XFAiInsightType.Suggestion,
                Title = "Otimizar consultas",
                Description = "3 queries podem ser indexadas",
                ActionLabel = "Ver detalhes",
                Priority = XFAiInsightPriority.Medium
            }
        };

        var cut = Render<XFAiProactiveInsight>(p => p.Add(x => x.Insights, insights));

        Assert.Contains("aria-label=\"Dispensar insight\"", cut.Markup);
    }

    [Fact]
    public void Insight_EmptyText_Shown()
    {
        var cut = Render<XFAiProactiveInsight>();

        Assert.Contains("Nenhum insight no momento.", cut.Markup);
    }

    #endregion

    #region XFAiOnboardingWizard

    [Fact]
    public void Wizard_RendersDefault()
    {
        var steps = new List<XFAiOnboardingStep>
        {
            new() { Id = "step1", Title = "Bem-vindo", Description = "Configure sua conta" }
        };

        var cut = Render<XFAiOnboardingWizard>(p => p.Add(x => x.Steps, steps));

        Assert.Contains("xf-ai-onboarding-wizard", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Configuração Inicial", cut.Markup);
    }

    [Fact]
    public void Wizard_ShowsSteps()
    {
        var steps = new List<XFAiOnboardingStep>
        {
            new() { Id = "step1", Title = "Bem-vindo", Description = "Configure sua conta" },
            new() { Id = "step2", Title = "Preferências", Description = "Defina suas preferências" }
        };

        var cut = Render<XFAiOnboardingWizard>(p => p.Add(x => x.Steps, steps));

        Assert.Contains("Bem-vindo", cut.Markup);
        Assert.Contains("1 de 2", cut.Markup);
        Assert.Contains("aria-label=\"Bem-vindo\"", cut.Markup);
    }

    [Fact]
    public void Wizard_ShowsNavigationButtons()
    {
        var steps = new List<XFAiOnboardingStep>
        {
            new() { Id = "step1", Title = "Bem-vindo", Description = "Configure sua conta" },
            new() { Id = "step2", Title = "Preferências", Description = "Defina suas preferências" }
        };

        var cut = Render<XFAiOnboardingWizard>(p => p.Add(x => x.Steps, steps));

        Assert.Contains("Próximo", cut.Markup);
        Assert.Contains("Pular", cut.Markup);
    }

    #endregion
}
