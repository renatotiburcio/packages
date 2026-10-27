using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Navigation.Steps
{
    public sealed record XFStepContext
    {
        public required XFStepItemModel Item { get; init; }

        public required int Index { get; init; }

        public required XFStepStatus Status { get; init; }

        public required int Total { get; init; }

        public required bool IsCurrent { get; init; }

        public required bool IsComplete { get; init; }

        public required bool IsError { get; init; }

        public required bool IsDisabled { get; init; }

        public required bool IsPending { get; init; }
    }
}
