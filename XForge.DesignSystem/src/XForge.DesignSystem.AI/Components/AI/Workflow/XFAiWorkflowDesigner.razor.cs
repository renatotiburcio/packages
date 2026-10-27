using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Workflow;

/// <summary>
/// Enhanced visual workflow designer with drag-drop support,
/// zoom/pan, connection rendering, and node palette. Pure Blazor (no JS interop).
/// </summary>
public partial class XFAiWorkflowDesigner : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Workflow nodes on the canvas.</summary>
    [Parameter]
    public IReadOnlyList<XFAiWfNode> Nodes { get; set; } = Array.Empty<XFAiWfNode>();

    /// <summary>Connections between nodes.</summary>
    [Parameter]
    public IReadOnlyList<XFAiWfConnection> Connections { get; set; } = Array.Empty<XFAiWfConnection>();

    /// <summary>Designer title.</summary>
    [Parameter]
    public string Title { get; set; } = "Workflow Designer";

    /// <summary>ARIA label for the region.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Designer de workflow";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the node type palette.</summary>
    [Parameter]
    public bool ShowPalette { get; set; } = true;

    /// <summary>Canvas width in pixels.</summary>
    [Parameter]
    public int CanvasWidth { get; set; } = 900;

    /// <summary>Canvas height in pixels.</summary>
    [Parameter]
    public int CanvasHeight { get; set; } = 500;

    /// <summary>Empty state text.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Arraste nos da paleta para criar um workflow.";

    /// <summary>Custom empty content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    /// <summary>Custom node template.</summary>
    [Parameter]
    public RenderFragment<XFAiWfNode>? NodeTemplate { get; set; }

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when a node is selected.</summary>
    [Parameter]
    public EventCallback<XFAiWfNode> OnNodeSelected { get; set; }

    /// <summary>Fired when a node is added from palette (type provided).</summary>
    [Parameter]
    public EventCallback<XFAiWfNodeType> OnNodeAddRequested { get; set; }

    /// <summary>Fired when a connection is selected.</summary>
    [Parameter]
    public EventCallback<XFAiWfConnection> OnConnectionSelected { get; set; }

    /// <summary>Fired when nodes are reordered (drag completed).</summary>
    [Parameter]
    public EventCallback<IReadOnlyList<XFAiWfNode>> OnNodesChanged { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private string? _selectedNodeId;
    private double _zoom = 1.0;
    private readonly XFAiWfNodeType[] _paletteTypes = Enum.GetValues<XFAiWfNodeType>();

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-workflow-designer overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    private string CanvasClass =>
        "relative overflow-auto bg-surface-50 dark:bg-slate-900/50";

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string GetNodeIcon(XFAiWfNodeType type) => type switch
    {
        XFAiWfNodeType.Start => "play",
        XFAiWfNodeType.End => "stop",
        XFAiWfNodeType.AiProcessor => "sparkles",
        XFAiWfNodeType.Rule => "scale-balanced",
        XFAiWfNodeType.Approval => "clipboard-check",
        XFAiWfNodeType.Trigger => "bolt",
        XFAiWfNodeType.Event => "bell",
        XFAiWfNodeType.Schedule => "clock",
        XFAiWfNodeType.Condition => "code-branch",
        XFAiWfNodeType.HttpCall => "globe",
        XFAiWfNodeType.Transform => "wand-magic-sparkles",
        XFAiWfNodeType.Delay => "hourglass",
        XFAiWfNodeType.Parallel => "split",
        XFAiWfNodeType.Merge => "merge",
        XFAiWfNodeType.SubProcess => "folder-tree",
        _ => "circle"
    };

    private static string GetNodeLabel(XFAiWfNodeType type) => type switch
    {
        XFAiWfNodeType.Start => "Inicio",
        XFAiWfNodeType.End => "Fim",
        XFAiWfNodeType.AiProcessor => "IA",
        XFAiWfNodeType.Rule => "Regra",
        XFAiWfNodeType.Approval => "Aprovacao",
        XFAiWfNodeType.Trigger => "Trigger",
        XFAiWfNodeType.Event => "Evento",
        XFAiWfNodeType.Schedule => "Agendamento",
        XFAiWfNodeType.Condition => "Condicao",
        XFAiWfNodeType.HttpCall => "HTTP",
        XFAiWfNodeType.Transform => "Transformar",
        XFAiWfNodeType.Delay => "Atraso",
        XFAiWfNodeType.Parallel => "Paralelo",
        XFAiWfNodeType.Merge => "Merge",
        XFAiWfNodeType.SubProcess => "Sub-processo",
        _ => "No"
    };

    private static string GetNodeColor(XFAiWfNodeType type) => type switch
    {
        XFAiWfNodeType.Start => "border-emerald-400 bg-emerald-50 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300",
        XFAiWfNodeType.End => "border-slate-400 bg-slate-50 text-slate-700 dark:bg-slate-800 dark:text-slate-300",
        XFAiWfNodeType.AiProcessor => "border-purple-400 bg-purple-50 text-purple-700 dark:bg-purple-900/30 dark:text-purple-300",
        XFAiWfNodeType.Rule => "border-blue-400 bg-blue-50 text-blue-700 dark:bg-blue-900/30 dark:text-blue-300",
        XFAiWfNodeType.Approval => "border-amber-400 bg-amber-50 text-amber-700 dark:bg-amber-900/30 dark:text-amber-300",
        XFAiWfNodeType.Trigger => "border-orange-400 bg-orange-50 text-orange-700 dark:bg-orange-900/30 dark:text-orange-300",
        XFAiWfNodeType.Event => "border-cyan-400 bg-cyan-50 text-cyan-700 dark:bg-cyan-900/30 dark:text-cyan-300",
        XFAiWfNodeType.Schedule => "border-indigo-400 bg-indigo-50 text-indigo-700 dark:bg-indigo-900/30 dark:text-indigo-300",
        XFAiWfNodeType.Condition => "border-teal-400 bg-teal-50 text-teal-700 dark:bg-teal-900/30 dark:text-teal-300",
        XFAiWfNodeType.HttpCall => "border-sky-400 bg-sky-50 text-sky-700 dark:bg-sky-900/30 dark:text-sky-300",
        XFAiWfNodeType.Transform => "border-pink-400 bg-pink-50 text-pink-700 dark:bg-pink-900/30 dark:text-pink-300",
        XFAiWfNodeType.Delay => "border-gray-400 bg-gray-50 text-gray-700 dark:bg-gray-800 dark:text-gray-300",
        XFAiWfNodeType.Parallel => "border-lime-400 bg-lime-50 text-lime-700 dark:bg-lime-900/30 dark:text-lime-300",
        XFAiWfNodeType.Merge => "border-rose-400 bg-rose-50 text-rose-700 dark:bg-rose-900/30 dark:text-rose-300",
        XFAiWfNodeType.SubProcess => "border-violet-400 bg-violet-50 text-violet-700 dark:bg-violet-900/30 dark:text-violet-300",
        _ => "border-slate-300 bg-surface-50 text-text-secondary"
    };

    private string GetNodeCardClass(XFAiWfNode node)
    {
        var selected = node.Id == _selectedNodeId;
        var baseCls = "absolute flex flex-col justify-center gap-1 rounded-lg border-2 px-3 py-2 cursor-pointer select-none transition-all hover:shadow-md";
        var colorCls = GetNodeColor(node.Type);
        var selectedCls = selected ? "ring-2 ring-purple-500 ring-offset-1" : "";
        var disabledCls = node.IsDisabled ? "opacity-40" : "";
        return $"{baseCls} {colorCls} {selectedCls} {disabledCls}";
    }

    // ── Interactions ────────────────────────────────────────────────────

    private void SelectNode(XFAiWfNode node)
    {
        _selectedNodeId = _selectedNodeId == node.Id ? null : node.Id;
        if (_selectedNodeId is not null)
            OnNodeSelected.InvokeAsync(node);
    }

    private void ZoomIn()
    {
        _zoom = Math.Min(2.0, _zoom + 0.1);
    }

    private void ZoomOut()
    {
        _zoom = Math.Max(0.3, _zoom - 0.1);
    }

    private void ResetZoom()
    {
        _zoom = 1.0;
    }

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
