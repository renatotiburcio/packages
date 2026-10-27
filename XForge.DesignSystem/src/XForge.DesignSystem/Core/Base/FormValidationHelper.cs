using System.Linq;
using Microsoft.AspNetCore.Components.Forms;

namespace XForge.DesignSystem.Core.Base;

/// <summary>
/// Shared helper for form validation message resolution.
/// Eliminates repeated null-forgiving patterns across 20+ form components.
/// </summary>
public static class FormValidationHelper
{
    /// <summary>
    /// Returns true if the field has any validation errors.
    /// </summary>
    public static bool HasErrors(EditContext? editContext, FieldIdentifier? fieldIdentifier)
    {
        return editContext is not null
            && fieldIdentifier.HasValue
            && editContext.GetValidationMessages(fieldIdentifier.Value).Any();
    }

    /// <summary>
    /// Returns the first validation message for the field, or null if valid.
    /// </summary>
    public static string? GetFirstMessage(EditContext? editContext, FieldIdentifier? fieldIdentifier)
    {
        if (!HasErrors(editContext, fieldIdentifier))
            return null;

        return editContext!.GetValidationMessages(fieldIdentifier!.Value).FirstOrDefault();
    }

    /// <summary>
    /// Returns all validation messages for the field.
    /// </summary>
    public static string[] GetMessages(EditContext? editContext, FieldIdentifier? fieldIdentifier)
    {
        if (!HasErrors(editContext, fieldIdentifier))
            return System.Array.Empty<string>();

        return editContext!.GetValidationMessages(fieldIdentifier!.Value).ToArray();
    }

    /// <summary>
    /// Returns the aria-invalid attribute value.
    /// </summary>
    public static string AriaInvalid(EditContext? editContext, FieldIdentifier? fieldIdentifier)
    {
        return HasErrors(editContext, fieldIdentifier) ? "true" : "false";
    }
}
