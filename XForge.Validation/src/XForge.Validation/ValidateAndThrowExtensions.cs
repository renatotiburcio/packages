namespace XForge.Validation;

/// <summary>Extensões ValidateAndThrow.</summary>
public static class ValidateAndThrowExtensions
{
    /// <summary>Valida e lança <see cref="ValidationException"/> quando houver falhas.</summary>
    public static ValidationResult ValidateAndThrow<T>(this IValidator<T> validator, T instance)
        where T : notnull
    {
        var result = validator.Validate(instance);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        return result;
    }

    /// <summary>Valida (async) e lança <see cref="ValidationException"/> quando houver falhas.</summary>
    public static async ValueTask<ValidationResult> ValidateAndThrowAsync<T>(this IValidator<T> validator,
        T instance, CancellationToken cancellationToken = default) where T : notnull
    {
        var result = await validator.ValidateAsync(instance, cancellationToken);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        return result;
    }
}
