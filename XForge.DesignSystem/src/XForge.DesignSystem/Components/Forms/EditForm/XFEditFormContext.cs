using Microsoft.AspNetCore.Components.Forms;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.EditForm
{
    public sealed class XFEditFormContext
    {
        public XFEditFormContext(
            EditContext editContext,
            bool disabled,
            bool readOnly,
            bool submitting,
            bool hasValidationErrors,
            bool actionsBlocked,
            XFEditFormLayout layout,
            XFDensity density)
            : this(
                editContext,
                disabled,
                readOnly,
                submitting,
                hasValidationErrors,
                actionsBlocked,
                trackChanges: true,
                confirmUnsavedChanges: false,
                isModified: false,
                layout,
                density,
                _ => Task.CompletedTask,
                () => Task.CompletedTask,
                () => Task.FromResult(!hasValidationErrors))
        {
        }

        public XFEditFormContext(
            EditContext editContext,
            bool disabled,
            bool readOnly,
            bool submitting,
            bool hasValidationErrors,
            bool actionsBlocked,
            bool trackChanges,
            bool confirmUnsavedChanges,
            bool isModified,
            XFEditFormLayout layout,
            XFDensity density,
            Func<XFEditFormCloseIntent, Task> requestCloseAsync,
            Func<Task> markSavedAsync,
            Func<Task<bool>> validateAsync)
        {
            EditContext = editContext;
            Disabled = disabled;
            ReadOnly = readOnly;
            Submitting = submitting;
            HasValidationErrors = hasValidationErrors;
            ActionsBlocked = actionsBlocked;
            TrackChanges = trackChanges;
            ConfirmUnsavedChanges = confirmUnsavedChanges;
            IsModified = isModified;
            Layout = layout;
            Density = density;
            RequestCloseAsync = requestCloseAsync;
            MarkSavedAsync = markSavedAsync;
            ValidateAsync = validateAsync;
        }

        public EditContext EditContext { get; }
        public bool Disabled { get; }
        public bool ReadOnly { get; }
        public bool Submitting { get; }
        public bool HasValidationErrors { get; }
        public bool ActionsBlocked { get; }
        public bool TrackChanges { get; }
        public bool ConfirmUnsavedChanges { get; }
        public bool IsModified { get; }
        public XFEditFormLayout Layout { get; }
        public XFDensity Density { get; }
        public Func<XFEditFormCloseIntent, Task> RequestCloseAsync { get; }
        public Func<Task> MarkSavedAsync { get; }
        public Func<Task<bool>> ValidateAsync { get; }
    }
}
