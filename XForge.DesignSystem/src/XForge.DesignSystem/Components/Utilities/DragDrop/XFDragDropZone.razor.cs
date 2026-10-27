using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Utilities.DragDrop
{
    public partial class XFDragDropZone<TPayload> : XForgeComponentBase
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment<XFDragDropState>? StateContent { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string DefaultLabel { get; set; } = "Solte aqui";
        [Parameter] public string? Description { get; set; }
        [Parameter] public IReadOnlyList<string> AcceptTypes { get; set; } = Array.Empty<string>();
        [Parameter] public TPayload? Payload { get; set; }
        [Parameter] public EventCallback<XFDragDropEventArgs> OnDragEnter { get; set; }
        [Parameter] public EventCallback<XFDragDropEventArgs> OnDragLeave { get; set; }
        [Parameter] public EventCallback<XFDragDropEventArgs> OnDragOver { get; set; }
        [Parameter] public EventCallback<XFDragDropEventArgs> OnDrop { get; set; }
        [Parameter] public EventCallback<XFDragDropPayloadEventArgs<TPayload>> PayloadDrop { get; set; }
        [Parameter] public EventCallback<XFDragDropState> StateChanged { get; set; }
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Primary;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool RejectUnknownTypes { get; set; }

        private XFDragDropState State { get; set; }
        private string LabelId => $"{Id}-label";
        private string DescriptionId => $"{Id}-description";
        private string? AriaDescribedBy => string.IsNullOrWhiteSpace(Description) ? null : DescriptionId;
        private string ResolvedLabel => string.IsNullOrWhiteSpace(Label) ? DefaultLabel : Label;

        private string RootClass => Join(
            "xf-drag-drop flex flex-col items-center justify-center rounded-xl border border-dashed text-center outline-none transition-all duration-200 ease-out",
            FullWidth ? "w-full" : "inline-flex",
            SizeClass,
            StateClass,
            Disabled ? "cursor-not-allowed opacity-60" : "cursor-default",
            Class);

        private string SizeClass => Size switch
        {
            XFSize.Sm => "min-h-24 gap-2 p-4",
            XFSize.Lg => "min-h-52 gap-4 p-8",
            _ => "min-h-36 gap-3 p-6"
        };

        private string StateClass => State switch
        {
            XFDragDropState.DragOver => $"{ToneBorderClass} {ToneBackgroundClass} ring-2 {ToneRingClass}",
            XFDragDropState.Rejected => "border-danger bg-[color-mix(in_oklab,var(--color-danger)_6%,transparent)] ring-2 ring-[color-mix(in_oklab,var(--color-danger)_18%,transparent)]",
            _ => "border-border bg-bg-surface hover:border-[color-mix(in_oklab,var(--color-primary)_35%,var(--color-border))] hover:bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)]"
        };

        private string ToneBorderClass => Tone switch
        {
            XFSemanticColor.Success => "border-success",
            XFSemanticColor.Warning => "border-warning",
            XFSemanticColor.Danger => "border-danger",
            XFSemanticColor.Default => "border-border",
            _ => "border-primary"
        };

        private string ToneBackgroundClass => Tone switch
        {
            XFSemanticColor.Success => "bg-[color-mix(in_oklab,var(--color-success)_7%,transparent)]",
            XFSemanticColor.Warning => "bg-[color-mix(in_oklab,var(--color-warning)_8%,transparent)]",
            XFSemanticColor.Danger => "bg-[color-mix(in_oklab,var(--color-danger)_7%,transparent)]",
            XFSemanticColor.Default => "bg-surface-50",
            _ => "bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)]"
        };

        private string ToneRingClass => Tone switch
        {
            XFSemanticColor.Success => "ring-[color-mix(in_oklab,var(--color-success)_18%,transparent)]",
            XFSemanticColor.Warning => "ring-[color-mix(in_oklab,var(--color-warning)_22%,transparent)]",
            XFSemanticColor.Danger => "ring-[color-mix(in_oklab,var(--color-danger)_18%,transparent)]",
            XFSemanticColor.Default => "ring-surface-200/80",
            _ => "ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]"
        };

        private string TitleClass => "text-sm font-semibold text-text-primary";
        private string DescriptionClass => "text-sm text-text-secondary";

        private async Task HandleDragEnterAsync(DragEventArgs args)
        {
            if (Disabled)
            {
                return;
            }

            var accepted = IsAccepted(args);
            await SetStateAsync(accepted ? XFDragDropState.DragOver : XFDragDropState.Rejected);
            await OnDragEnter.InvokeAsync(new XFDragDropEventArgs(args, Id, accepted));
        }

        private async Task HandleDragOverAsync(DragEventArgs args)
        {
            if (Disabled)
            {
                return;
            }

            var accepted = IsAccepted(args);
            await SetStateAsync(accepted ? XFDragDropState.DragOver : XFDragDropState.Rejected);
            await OnDragOver.InvokeAsync(new XFDragDropEventArgs(args, Id, accepted));
        }

        private async Task HandleDragLeaveAsync(DragEventArgs args)
        {
            if (Disabled)
            {
                return;
            }

            await SetStateAsync(XFDragDropState.Idle);
            await OnDragLeave.InvokeAsync(new XFDragDropEventArgs(args, Id, true));
        }

        private async Task HandleDropAsync(DragEventArgs args)
        {
            if (Disabled)
            {
                return;
            }

            var accepted = IsAccepted(args);
            await SetStateAsync(XFDragDropState.Idle);
            var eventArgs = new XFDragDropEventArgs(args, Id, accepted);
            await OnDrop.InvokeAsync(eventArgs);

            if (PayloadDrop.HasDelegate)
            {
                await PayloadDrop.InvokeAsync(new XFDragDropPayloadEventArgs<TPayload>(eventArgs, Payload));
            }
        }

        private async Task SetStateAsync(XFDragDropState state)
        {
            if (State == state)
            {
                return;
            }

            State = state;
            if (StateChanged.HasDelegate)
            {
                await StateChanged.InvokeAsync(state);
            }
        }

        private bool IsAccepted(DragEventArgs args)
        {
            if (AcceptTypes.Count == 0)
            {
                return true;
            }

            var types = args.DataTransfer?.Types ?? Array.Empty<string>();
            if (types.Length == 0)
            {
                return !RejectUnknownTypes;
            }

            return types.Any(type => AcceptTypes.Any(accepted => string.Equals(type, accepted, StringComparison.OrdinalIgnoreCase)));
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
