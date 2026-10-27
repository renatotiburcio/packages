using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Avatars;

/// <summary>
/// AI agent personality configuration component. Displays agent identity,
/// tone, expertise, greeting, and suggested prompts. Integrates with
/// XFAiAvatar for visual representation.
/// </summary>
public partial class XFAiAgentPersonality : XForgeComponentBase, IDisposable
{
    // -- Parameters ------------------------------------------------------

    /// <summary>Agent personality configuration.</summary>
    [Parameter] public XFAiAgentPersonalityConfig? Personality { get; set; }

    /// <summary>Title shown when Personality is null.</summary>
    [Parameter] public string Title { get; set; } = "Agente IA";

    /// <summary>Whether to show the header with avatar and name.</summary>
    [Parameter] public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show tone/expertise detail badges.</summary>
    [Parameter] public bool ShowDetails { get; set; } = true;

    /// <summary>Whether to show the greeting message.</summary>
    [Parameter] public bool ShowGreeting { get; set; } = true;

    /// <summary>Whether to show suggested prompt buttons.</summary>
    [Parameter] public bool ShowSuggestedPrompts { get; set; } = true;

    /// <summary>ARIA label for the component.</summary>
    [Parameter] public string AriaLabel { get; set; } = "Personalidade do agente IA";

    /// <summary>Custom header content.</summary>
    [Parameter] public RenderFragment? HeaderContent { get; set; }

    /// <summary>Custom content rendered below personality details.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Fired when a suggested prompt is clicked.</summary>
    [Parameter] public EventCallback<string> OnSuggestedPrompt { get; set; }

    // -- Computed --------------------------------------------------------

    private string RootClass => Join(
        "xf-ai-agent-personality overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class);

    private string AriaBusyValue => "false";

    // -- Interactions ----------------------------------------------------

    private async Task HandleSuggestedPromptAsync(string prompt)
    {
        if (OnSuggestedPrompt.HasDelegate)
        {
            await OnSuggestedPrompt.InvokeAsync(prompt);
        }
    }

    // -- Helpers ---------------------------------------------------------

    private static string GetToneLabel(XFAiAgentTone tone) => tone switch
    {
        XFAiAgentTone.Formal => "Formal",
        XFAiAgentTone.Friendly => "Amigável",
        XFAiAgentTone.Concise => "Conciso",
        XFAiAgentTone.Detailed => "Detalhado",
        XFAiAgentTone.Empathetic => "Empático",
        XFAiAgentTone.Custom => "Personalizado",
        _ => tone.ToString()
    };

    private static string GetExpertiseLabel(XFAiAgentExpertise expertise) => expertise switch
    {
        XFAiAgentExpertise.General => "Geral",
        XFAiAgentExpertise.Finance => "Financeiro",
        XFAiAgentExpertise.Fiscal => "Fiscal",
        XFAiAgentExpertise.Stock => "Estoque",
        XFAiAgentExpertise.HR => "RH",
        XFAiAgentExpertise.Legal => "Jurídico",
        XFAiAgentExpertise.IT => "TI",
        XFAiAgentExpertise.Sales => "Comercial",
        XFAiAgentExpertise.Custom => "Personalizado",
        _ => expertise.ToString()
    };

    private static string Join(params string?[] values)
    {
        return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
