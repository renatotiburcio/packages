using XForge.Validation;

namespace XForge.Validation.TestHelper;

/// <summary>
/// Helpers para obter o <see cref="ValidationResult"/> de um validador dentro de testes —
/// encadeia diretamente com as assertions de <see cref="ValidationResultAssertions"/>.
/// </summary>
public static class ValidatorTestExtensions
{
    /// <summary>Valida a instância e devolve o resultado para encadear assertions.</summary>
    public static ValidationResult TestValidationResult<T>(this IValidator<T> validator, T instance)
        where T : notnull
        => validator.Validate(instance);

    /// <summary>Valida a instância (assíncrono) e devolve o resultado para encadear assertions.</summary>
    public static async Task<ValidationResult> TestValidationResultAsync<T>(
        this IValidator<T> validator, T instance, CancellationToken cancellationToken = default)
        where T : notnull
        => await validator.ValidateAsync(instance, cancellationToken);

    /// <summary>Valida com um RuleSet e devolve o resultado para encadear assertions.</summary>
    public static ValidationResult TestValidationResult<T>(
        this IValidator<T> validator, T instance, string ruleSet)
        where T : notnull
        => validator.Validate(instance, ruleSet);

    /// <summary>
    /// Atalho: valida e já assertiona que não há nenhuma falha.
    /// Equivalente a <c>validator.TestValidationResult(x).ShouldNotHaveAnyValidationErrors()</c>.
    /// </summary>
    public static void ShouldValidateSuccessfully<T>(this IValidator<T> validator, T instance)
        where T : notnull
        => validator.TestValidationResult(instance).ShouldNotHaveAnyValidationErrors();

    /// <summary>Versão assíncrona de <see cref="ShouldValidateSuccessfully{T}"/>.</summary>
    public static async Task ShouldValidateSuccessfullyAsync<T>(
        this IValidator<T> validator, T instance, CancellationToken cancellationToken = default)
        where T : notnull
        => (await validator.TestValidationResultAsync(instance, cancellationToken))
            .ShouldNotHaveAnyValidationErrors();
}
