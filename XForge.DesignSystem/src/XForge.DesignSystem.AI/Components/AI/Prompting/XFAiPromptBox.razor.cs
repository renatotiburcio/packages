using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Prompting;

/// <summary>
/// Advanced AI prompt editor with templates, variables, and token estimation.
/// Supports Enter to send, Shift+Enter for newline, template selection with dropdown,
/// and real-time token/character counting.
/// </summary>
public partial class XFAiPromptBox : XForgeComponentBase
{
    private static readonly Regex VariablePattern = new(@"\{\{(\w+)\}\}", RegexOptions.Compiled);
    private const int CharsPerToken = 4;

    private bool _templatesOpen;
    private int _highlightedTemplateIndex = -1;
    private bool _preventKeyDefault;
    private ElementReference _textareaRef;

    #region Parameters

    /// <summary>Current prompt text (two-way bindable).</summary>
    [Parameter] public string Value { get; set; } = string.Empty;

    /// <summary>Fired when the prompt text changes.</summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>Placeholder text for the textarea.</summary>
    [Parameter] public string Placeholder { get; set; } = "Escreva seu prompt aqui...";

    /// <summary>Available prompt templates.</summary>
    [Parameter] public IReadOnlyList<XFAiPromptTemplate> Templates { get; set; } = Array.Empty<XFAiPromptTemplate>();

    /// <summary>Show the template selector button.</summary>
    [Parameter] public bool ShowTemplates { get; set; } = true;

    /// <summary>Show token estimation in the footer.</summary>
    [Parameter] public bool ShowTokenEstimate { get; set; } = true;

    /// <summary>Show character count in the footer.</summary>
    [Parameter] public bool ShowCharCount { get; set; } = true;

    /// <summary>Maximum allowed tokens. Shows warning when exceeded.</summary>
    [Parameter] public int? MaxTokens { get; set; }

    /// <summary>Maximum character length.</summary>
    [Parameter] public int? MaxLength { get; set; }

    /// <summary>Number of rows for the textarea.</summary>
    [Parameter] public int Rows { get; set; } = 4;

    /// <summary>Label for the templates button.</summary>
    [Parameter] public string TemplatesLabel { get; set; } = "Templates";

    /// <summary>Label for the token count display.</summary>
    [Parameter] public string TokenEstimateLabel { get; set; } = "Tokens estimados";

    /// <summary>Send button label.</summary>
    [Parameter] public string SendLabel { get; set; } = "Enviar";

    /// <summary>Read-only mode.</summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>Show inline send button.</summary>
    [Parameter] public bool ShowSendButton { get; set; } = true;

    /// <summary>Custom template rendering fragment.</summary>
    [Parameter] public RenderFragment<XFAiPromptTemplate>? TemplateContent { get; set; }

    /// <summary>Fired when the user sends the prompt (Enter or button click).</summary>
    [Parameter] public EventCallback<string> OnSend { get; set; }

    /// <summary>Fired when a template is selected.</summary>
    [Parameter] public EventCallback<XFAiPromptTemplate> OnTemplateSelect { get; set; }

    #endregion

    #region Computed Properties

    private int EstimatedTokens => string.IsNullOrEmpty(Value) ? 0 : (int)Math.Ceiling((double)Value.Length / CharsPerToken);

    private int CharCount => Value?.Length ?? 0;

    private bool IsMaxTokensExceeded => MaxTokens.HasValue && EstimatedTokens > MaxTokens.Value;

    private bool IsMaxLengthExceeded => MaxLength.HasValue && CharCount > MaxLength.Value;

    private bool CanSend => !Disabled && !ReadOnly && !string.IsNullOrWhiteSpace(Value) && !IsMaxTokensExceeded && !IsMaxLengthExceeded;

    private bool HasTemplates => Templates is { Count: > 0 };

    private bool ShowFooter => ShowTokenEstimate || ShowCharCount || MaxTokens.HasValue;

    private string AriaBusy => "false";

    #endregion

    #region CSS Classes

    private string RootClass =>
        Join("xf-ai-prompt-box flex flex-col rounded-lg border bg-bg-surface shadow-sm transition-colors duration-150",
            IsMaxTokensExceeded || IsMaxLengthExceeded
                ? "border-error-400 focus-within:ring-error-200"
                : "border-border-light focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]",
            Class);

    private string TextareaClass =>
        Join(
            "w-full resize-none bg-transparent px-3 py-2.5 text-sm leading-6 text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:opacity-60",
            ReadOnly ? "cursor-default" : "");

    private string FooterClass =>
        "flex items-center justify-between gap-3 border-t border-border-light px-3 py-2 text-xs";

    private string TokenCountClass =>
        Join("inline-flex items-center gap-1.5",
            IsMaxTokensExceeded ? "font-medium text-error-600" : "text-text-muted");

    private string CharCountClass =>
        Join("",
            IsMaxLengthExceeded ? "font-medium text-error-600" : "text-text-muted");

    private string TemplatesButtonClass =>
        "inline-flex items-center gap-1.5 rounded-md border border-border-light bg-bg-surface px-2.5 py-1.5 text-xs font-medium text-text-secondary outline-none transition-colors duration-150 hover:bg-surface-50 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-1 disabled:cursor-not-allowed disabled:opacity-50";

    private string SendButtonClass =>
        "inline-flex h-8 min-w-8 items-center justify-center rounded-md bg-primary px-2.5 text-xs font-semibold text-primary-contrast outline-none transition-all duration-150 hover:bg-primary-hover focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-1 disabled:cursor-not-allowed disabled:opacity-50";

    private string DropdownClass =>
        "absolute bottom-full left-0 z-50 mb-1 max-h-64 w-72 overflow-y-auto rounded-lg border border-border-light bg-bg-surface py-1 shadow-lg";

    private string DropdownItemClass(int index) =>
        Join("flex cursor-pointer flex-col gap-0.5 px-3 py-2 text-sm transition-colors duration-75",
            index == _highlightedTemplateIndex
                ? "bg-primary/10 text-primary"
                : "text-text-primary hover:bg-surface-50");

    #endregion

    #region Event Handlers

    private async Task HandleInputAsync(ChangeEventArgs args)
    {
        var next = args.Value?.ToString() ?? string.Empty;

        if (MaxLength.HasValue && next.Length > MaxLength.Value)
        {
            next = next[..MaxLength.Value];
        }

        Value = next;

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        _preventKeyDefault = false;

        if (_templatesOpen)
        {
            _preventKeyDefault = args.Key is "ArrowDown" or "ArrowUp" or "Enter" or "Escape";
            await HandleDropdownKeyDownAsync(args);
            return;
        }

        if (args.Key == "Enter" && !args.ShiftKey)
        {
            _preventKeyDefault = true;

            if (CanSend)
            {
                await SendAsync();
            }
        }
    }

    private async Task HandleDropdownKeyDownAsync(KeyboardEventArgs args)
    {
        switch (args.Key)
        {
            case "ArrowDown":
                _highlightedTemplateIndex = Math.Min(_highlightedTemplateIndex + 1, Templates.Count - 1);
                break;

            case "ArrowUp":
                _highlightedTemplateIndex = Math.Max(_highlightedTemplateIndex - 1, 0);
                break;

            case "Enter":
                if (_highlightedTemplateIndex >= 0 && _highlightedTemplateIndex < Templates.Count)
                {
                    await SelectTemplateAsync(Templates[_highlightedTemplateIndex]);
                }
                break;

            case "Escape":
                CloseTemplates();
                break;
        }
    }

    private async Task SendAsync()
    {
        if (!CanSend)
        {
            return;
        }

        if (OnSend.HasDelegate)
        {
            await OnSend.InvokeAsync(Value!.Trim());
        }
    }

    private void ToggleTemplates()
    {
        if (_templatesOpen)
        {
            CloseTemplates();
        }
        else
        {
            _templatesOpen = true;
            _highlightedTemplateIndex = Templates.Count > 0 ? 0 : -1;
        }
    }

    private void CloseTemplates()
    {
        _templatesOpen = false;
        _highlightedTemplateIndex = -1;
    }

    private async Task SelectTemplateAsync(XFAiPromptTemplate template)
    {
        Value = template.Content;

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }

        if (OnTemplateSelect.HasDelegate)
        {
            await OnTemplateSelect.InvokeAsync(template);
        }

        CloseTemplates();
    }

    #endregion

    #region Helpers

    private static string Join(params string?[] values)
    {
        return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    /// <summary>
    /// Groups templates by their <see cref="XFAiPromptTemplate.Category"/> property.
    /// Templates without a category are placed in the "Outros" group.
    /// </summary>
    private IEnumerable<IGrouping<string, XFAiPromptTemplate>> GroupedTemplates =>
        Templates
            .GroupBy(t => string.IsNullOrWhiteSpace(t.Category) ? "Outros" : t.Category!)
            .OrderBy(g => g.Key);

    #endregion
}