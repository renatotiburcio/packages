// T048: FluentValidation provider abstraction (Blazorise.FluentValidation pattern).
// P1, 16h, no deps.
namespace XForge.DesignSystem.Integration.FluentValidation;

/// <summary>
/// Resolves a FluentValidation <c>IValidator&lt;T&gt;</c> for a given model type.
/// Implementation is registered via DI when FluentValidation is in the project.
/// </summary>
public interface IXFFluentValidationProvider
{
    /// <summary>Try to resolve the validator for <typeparamref name="T"/>.</summary>
    object? ResolveValidator<T>();
    /// <summary>Indicates whether a validator exists for <typeparamref name="T"/>.</summary>
    bool HasValidator<T>();
}

/// <summary>Validation result returned to the form engine.</summary>
public sealed record XFValidationResult(bool IsValid, IReadOnlyList<string> Errors)
{
    public static XFValidationResult Valid { get; } = new(true, Array.Empty<string>());
    public static XFValidationResult Invalid(params string[] errors) => new(false, errors);
}