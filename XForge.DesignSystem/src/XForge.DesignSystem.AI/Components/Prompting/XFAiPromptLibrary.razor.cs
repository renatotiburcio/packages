using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.AI.Components;

/// <summary>
/// Browseable library of AI prompt templates with search, filtering by tags,
/// and category organization. Users can select a prompt to use in their workflow.
/// </summary>
public partial class XFAiPromptLibrary : XForgeComponentBase
{
    private string Filter { get; set; } = "";

    /// <summary>Available prompt templates.</summary>
    [Parameter] public IReadOnlyList<PromptTemplate> Prompts { get; set; } = new List<PromptTemplate>();

    /// <summary>Fires when a prompt template is selected.</summary>
    [Parameter] public EventCallback<PromptTemplate> OnSelect { get; set; }

    private IEnumerable<PromptTemplate> Filtered => string.IsNullOrEmpty(Filter)
        ? Prompts
        : Prompts.Where(p => p.Title.Contains(Filter, StringComparison.OrdinalIgnoreCase)
            || p.Tags.Any(t => t.Contains(Filter, StringComparison.OrdinalIgnoreCase)));
}
