using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.Validation
{
    /// <summary>
    /// Base class for XForge field validators. Integrates with Blazor EditContext
    /// via ValidationMessageStore. Subclasses override Validate() to provide logic.
    /// </summary>
    public abstract class XFValidatorBase : ComponentBase, IDisposable
    {
        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        /// <summary>An expression targeting the field to validate.</summary>
        [Parameter] public Expression<Func<object?>>? For { get; set; }

        /// <summary>The error message displayed when validation fails.</summary>
        [Parameter] public string Text { get; set; } = "Validation error";

        protected EditContext? EditContext { get; private set; }
        protected FieldIdentifier? FieldIdentifier { get; private set; }
        protected bool IsValid { get; private set; } = true;

        private ValidationMessageStore? messageStore;
        private EditContext? previousEditContext;

        protected override void OnParametersSet()
        {
            if (CascadedEditContext != previousEditContext)
            {
                DetachEditContext();
                EditContext = CascadedEditContext;
                previousEditContext = EditContext;

                if (EditContext is not null)
                {
                    messageStore = new ValidationMessageStore(EditContext);
                    EditContext.OnValidationRequested += OnValidationRequested;
                    EditContext.OnFieldChanged += OnFieldChanged;
                }
            }

            if (For is not null && EditContext is not null)
            {
                FieldIdentifier = global::Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(For);
            }
        }

        private async void OnValidationRequested(object? sender, ValidationRequestedEventArgs args)
        {
            if (EditContext is null || FieldIdentifier is null) return;

            var value = GetFieldValue();
            var valid = Validate(value);

            if (valid != IsValid)
            {
                IsValid = valid;
                if (!IsValid)
                {
                    messageStore!.Add(FieldIdentifier.Value, Text);
                }
                else
                {
                    messageStore!.Clear(FieldIdentifier.Value);
                }

                EditContext.NotifyValidationStateChanged();
                await InvokeAsync(StateHasChanged);
            }
            else if (!IsValid)
            {
                // Re-add message if still invalid
                messageStore!.Clear(FieldIdentifier.Value);
                messageStore.Add(FieldIdentifier.Value, Text);
                EditContext.NotifyValidationStateChanged();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async void OnFieldChanged(object? sender, FieldChangedEventArgs args)
        {
            if (FieldIdentifier is null || !args.FieldIdentifier.Equals(FieldIdentifier.Value)) return;

            var value = GetFieldValue();
            var valid = Validate(value);

            if (valid != IsValid)
            {
                IsValid = valid;
                if (!IsValid)
                {
                    messageStore!.Add(FieldIdentifier.Value, Text);
                }
                else
                {
                    messageStore!.Clear(FieldIdentifier.Value);
                }

                EditContext?.NotifyValidationStateChanged();
                await InvokeAsync(StateHasChanged);
            }
        }

        private object? GetFieldValue()
        {
            if (For is null || FieldIdentifier is null) return null;
            return For.Compile().Invoke();
        }

        /// <summary>
        /// Validates the field value. Returns true if valid, false if invalid.
        /// </summary>
        protected abstract bool Validate(object? value);

        private void DetachEditContext()
        {
            if (previousEditContext is not null)
            {
                previousEditContext.OnValidationRequested -= OnValidationRequested;
                previousEditContext.OnFieldChanged -= OnFieldChanged;
            }
        }

        public void Dispose()
        {
            DetachEditContext();
            GC.SuppressFinalize(this);
        }
    }
}
