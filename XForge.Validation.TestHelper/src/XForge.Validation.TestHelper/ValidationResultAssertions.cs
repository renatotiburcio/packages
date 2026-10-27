using System.Linq.Expressions;
using XForge.Validation;
using Xunit;
using Xunit.Sdk;

namespace XForge.Validation.TestHelper;

/// <summary>
/// Assertions de teste sobre <see cref="ValidationResult"/> — estilo FluentValidation.TestHelper,
/// adaptadas ao modelo do XForge (falhas por nome de membro, códigos padronizados e severidade).
/// </summary>
public static class ValidationResultAssertions
{
    /// <summary>Nenhuma falha (nenhuma severidade) no resultado.</summary>
    public static void ShouldNotHaveAnyValidationErrors(this ValidationResult result)
    {
        if (result.Errors.Count > 0)
        {
            throw new XunitException(BuildMessage(result.Errors, "não deveriam existir falhas"));
        }
    }

    /// <summary>Existe ao menos uma falha no resultado (qualquer membro).</summary>
    public static void ShouldHaveAnyValidationError(this ValidationResult result)
    {
        if (result.Errors.Count == 0)
        {
            throw new XunitException("Esperada ao menos uma falha de validação, mas o resultado não tinha nenhuma.");
        }
    }

    /// <summary>Sem falhas para o membro informado (nome ou expressão de propriedade).</summary>
    public static void ShouldNotHaveValidationErrorFor<T, TProperty>(
        this ValidationResult result, Expression<Func<T, TProperty>> propertyExpression)
        => result.ShouldNotHaveValidationErrorFor(PropertyName(propertyExpression));

    /// <summary>Sem falhas para o membro informado (comparação case-insensitive).</summary>
    public static void ShouldNotHaveValidationErrorFor(this ValidationResult result, string propertyName)
    {
        var failures = result.ErrorsFor(propertyName).ToList();

        if (failures.Count > 0)
        {
            throw new XunitException(BuildMessage(failures, $"não deveriam existir falhas para '{propertyName}'"));
        }
    }

    /// <summary>Existe falha para o membro (mensagem opcional comparada exatamente, case-sensitive).</summary>
    public static void ShouldHaveValidationErrorFor<T, TProperty>(
        this ValidationResult result, Expression<Func<T, TProperty>> propertyExpression, string? expectedMessage = null)
        => result.ShouldHaveValidationErrorFor(PropertyName(propertyExpression), expectedMessage);

    /// <summary>Existe falha para o membro (mensagem opcional comparada exatamente).</summary>
    public static void ShouldHaveValidationErrorFor(this ValidationResult result, string propertyName, string? expectedMessage = null)
    {
        var failures = result.ErrorsFor(propertyName).ToList();

        if (failures.Count == 0)
        {
            throw new XunitException(
                $"Esperada uma falha de validação para '{propertyName}', mas nenhuma foi encontrada.");
        }

        if (expectedMessage is null)
        {
            return;
        }

        if (!failures.Any(failure => string.Equals(failure.ErrorMessage, expectedMessage, StringComparison.Ordinal)))
        {
            throw new XunitException(
                $"Esperada a mensagem '{expectedMessage}' para '{propertyName}', mas as falhas foram: " +
                string.Join("; ", failures.Select(failure => $"'{failure.ErrorMessage}'")) + ".");
        }
    }

    /// <summary>Existe falha com o código padronizado informado para o membro.</summary>
    public static void ShouldHaveValidationErrorWithCode(this ValidationResult result, string propertyName, string errorCode)
    {
        var failures = result.ErrorsFor(propertyName)
            .Where(failure => string.Equals(failure.ErrorCode, errorCode, StringComparison.Ordinal))
            .ToList();

        if (failures.Count == 0)
        {
            var codes = result.ErrorsFor(propertyName).Select(failure => failure.ErrorCode).ToList();

            throw new XunitException(
                $"Esperada uma falha com código '{errorCode}' para '{propertyName}', mas os códigos encontrados foram: " +
                (codes.Count == 0 ? "nenhuma falha para o membro." : string.Join(", ", codes) + "."));
        }
    }

    /// <summary>Nenhuma falha com a severidade informada em nenhum membro (padrão: avisos não bloqueiam).</summary>
    public static void ShouldNotHaveValidationErrorsWithSeverity(this ValidationResult result, Severity severity)
    {
        var failures = result.ErrorsWithSeverity(severity).ToList();

        if (failures.Count > 0)
        {
            throw new XunitException(BuildMessage(failures, $"não deveriam existir falhas com severidade '{severity}'"));
        }
    }

    private static string PropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var body = expression.Body;

        while (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } convert)
        {
            body = convert.Operand;
        }

        return body is MemberExpression member
            ? member.Member.Name
            : throw new ArgumentException(
                "A expressão deve ser um acesso simples a um membro (ex.: x => x.Nome).", nameof(expression));
    }

    private static string BuildMessage(IReadOnlyCollection<ValidationFailure> failures, string expectation)
    {
        return $"Encontradas {failures.Count} falha(s) de validação inesperadas ({expectation}): " +
               string.Join("; ", failures.Select(failure => failure.ToString())) + ".";
    }
}
