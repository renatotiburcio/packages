using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace XForge.Validation;

/// <summary>Validador fortemente tipado para <typeparamref name="T"/>.</summary>
/// <typeparam name="T">O tipo validado.</typeparam>
public interface IValidator<in T> where T : notnull
{
    /// <summary>Valida a instância (regras do ruleset default).</summary>
    ValidationResult Validate(T instance);

    /// <summary>Valida a instância executando também as regras async.</summary>
    ValueTask<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);

    /// <summary>Valida a instância para os rulesets informados (separados por vírgula; <c>*</c> = todos).</summary>
    ValidationResult Validate(T instance, string ruleSet);

    /// <summary>Valida a instância para os rulesets informados executando também as regras async.</summary>
    ValueTask<ValidationResult> ValidateAsync(T instance, string ruleSet, CancellationToken cancellationToken = default);

    /// <summary><see langword="true"/> quando este validador sabe validar instâncias do tipo informado.</summary>
    bool CanValidateInstancesOfType(Type type);
}

/// <summary>Versão não genérica (para DI/resolução dinâmica).</summary>
public interface IValidator
{
    /// <summary>Valida a instância (boxed).</summary>
    ValidationResult Validate(object instance);

    /// <summary>Valida a instância (boxed) executando também as regras async.</summary>
    ValueTask<ValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default);

    /// <summary><see langword="true"/> quando este validador sabe validar instâncias do tipo informado.</summary>
    bool CanValidateInstancesOfType(Type type);
}

/// <summary>Builder inicial de uma cadeia de regras para um membro. Já expõe todas as opções (uso: RuleFor(...).NotNull()).</summary>
/// <typeparam name="T">O tipo validado.</typeparam>
/// <typeparam name="TProperty">O tipo do membro.</typeparam>
public interface IRuleBuilderInitial<T, TProperty> : IRuleBuilderOptions<T, TProperty>
    where T : notnull
{
}

/// <summary>Builder de opções de uma cadeia de regras.</summary>
/// <typeparam name="T">O tipo validado.</typeparam>
/// <typeparam name="TProperty">O tipo do membro.</typeparam>
public interface IRuleBuilderOptions<T, TProperty> where T : notnull
{
    /// <summary>O membro não pode ser null.</summary>
    IRuleBuilderOptions<T, TProperty> NotNull(string? message = null);

    /// <summary>O membro deve ser null.</summary>
    IRuleBuilderOptions<T, TProperty> Null(string? message = null);

    /// <summary>O membro não pode ser null, string vazia ou coleção vazia.</summary>
    IRuleBuilderOptions<T, TProperty> NotEmpty(string? message = null);

    /// <summary>O membro deve ser null, string vazia ou coleção vazia.</summary>
    IRuleBuilderOptions<T, TProperty> Empty(string? message = null);

    /// <summary>Comprimento de string/coleção entre min e max (inclusivos).</summary>
    IRuleBuilderOptions<T, TProperty> Length(int min, int max, string? message = null);

    /// <summary>Comprimento máximo de string/coleção.</summary>
    IRuleBuilderOptions<T, TProperty> MaxLength(int max, string? message = null);

    /// <summary>Comprimento mínimo de string/coleção.</summary>
    IRuleBuilderOptions<T, TProperty> MinLength(int min, string? message = null);

    /// <summary>Comprimento exato de string/coleção.</summary>
    IRuleBuilderOptions<T, TProperty> ExactLength(int length, string? message = null);

    /// <summary>E-mail válido (System.Net.Mail.MailAddress).</summary>
    IRuleBuilderOptions<T, TProperty> EmailAddress(string? message = null);

    /// <summary>Corresponde à expressão regular.</summary>
    IRuleBuilderOptions<T, TProperty> Matches(string pattern, string? message = null);

    /// <summary>Corresponde à expressão regular com opções.</summary>
    IRuleBuilderOptions<T, TProperty> Matches(string pattern, RegexOptions options, string? message = null);

    /// <summary>Igual ao valor informado.</summary>
    IRuleBuilderOptions<T, TProperty> Equal(TProperty value, string? message = null);

    /// <summary>Diferente do valor informado.</summary>
    IRuleBuilderOptions<T, TProperty> NotEqual(TProperty value, string? message = null);

    /// <summary>Igual ao membro da própria instância.</summary>
    IRuleBuilderOptions<T, TProperty> Equal(Expression<Func<T, TProperty>> otherMember, string? message = null);

    /// <summary>Diferente do membro da própria instância.</summary>
    IRuleBuilderOptions<T, TProperty> NotEqual(Expression<Func<T, TProperty>> otherMember, string? message = null);

    /// <summary>Predicado customizado sobre o valor do membro.</summary>
    IRuleBuilderOptions<T, TProperty> Must(Func<TProperty, bool> predicate, string? message = null);

    /// <summary>Predicado customizado recebendo a instância inteira.</summary>
    IRuleBuilderOptions<T, TProperty> Must(Func<T, TProperty, bool> predicate, string? message = null);

    /// <summary>Predicado customizado assíncrono (executado apenas em ValidateAsync).</summary>
    IRuleBuilderOptions<T, TProperty> MustAsync(Func<TProperty, CancellationToken, ValueTask<bool>> predicate, string? message = null);

    /// <summary>Predicado customizado assíncrono recebendo a instância inteira.</summary>
    IRuleBuilderOptions<T, TProperty> MustAsync(Func<T, TProperty, CancellationToken, ValueTask<bool>> predicate, string? message = null);

    /// <summary>Menor que o valor informado (IComparable).</summary>
    IRuleBuilderOptions<T, TProperty> LessThan(TProperty value, string? message = null);

    /// <summary>Menor ou igual ao valor informado.</summary>
    IRuleBuilderOptions<T, TProperty> LessThanOrEqualTo(TProperty value, string? message = null);

    /// <summary>Maior que o valor informado.</summary>
    IRuleBuilderOptions<T, TProperty> GreaterThan(TProperty value, string? message = null);

    /// <summary>Maior ou igual ao valor informado.</summary>
    IRuleBuilderOptions<T, TProperty> GreaterThanOrEqualTo(TProperty value, string? message = null);

    /// <summary>Valor no intervalo inclusivo.</summary>
    IRuleBuilderOptions<T, TProperty> InclusiveBetween(TProperty from, TProperty to, string? message = null);

    /// <summary>Valor no intervalo exclusivo.</summary>
    IRuleBuilderOptions<T, TProperty> ExclusiveBetween(TProperty from, TProperty to, string? message = null);

    /// <summary>Valor está entre as opções informadas.</summary>
    IRuleBuilderOptions<T, TProperty> IsIn(params TProperty[] validValues);

    /// <summary>Valor é membro do enum informado.</summary>
    IRuleBuilderOptions<T, TProperty> IsInEnum(string? message = null);

    /// <summary>Mensagem customizada com placeholders: {PropertyName}, {PropertyValue}, {MinLength}, {MaxLength}, {TotalLength}, {ComparisonValue}, {From}, {To}.</summary>
    IRuleBuilderOptions<T, TProperty> WithMessage(string message);

    /// <summary>Código de erro customizado (default: código padronizado do validador, ex.: NOT_NULL).</summary>
    IRuleBuilderOptions<T, TProperty> WithErrorCode(string errorCode);

    /// <summary>Severidade customizada.</summary>
    IRuleBuilderOptions<T, TProperty> WithSeverity(Severity severity);

    /// <summary>Severidade calculada em runtime.</summary>
    IRuleBuilderOptions<T, TProperty> WithSeverity(Func<T, TProperty, Severity> severityFactory);

    /// <summary>Condição de execução para o validador atual (default) ou toda a cadeia.</summary>
    IRuleBuilderOptions<T, TProperty> When(Func<T, bool> predicate, ApplyConditionTo applyTo = ApplyConditionTo.CurrentValidator);

    /// <summary>Inverso da condição: executa quando o predicado for falso.</summary>
    IRuleBuilderOptions<T, TProperty> Unless(Func<T, bool> predicate, ApplyConditionTo applyTo = ApplyConditionTo.CurrentValidator);

    /// <summary>Modo de cascata desta cadeia (default: StopOnFirstFailure).</summary>
    IRuleBuilderOptions<T, TProperty> Cascade(CascadeMode cascadeMode);

    /// <summary>Atribui um validador aninhado para o membro (objetos complexos).</summary>
    IRuleBuilderOptions<T, TProperty> SetValidator(IValidator<TProperty> validator);

    /// <summary>Atribui um validador aninhado apenas quando a condição for verdadeira.</summary>
    IRuleBuilderOptions<T, TProperty> SetValidator(IValidator<TProperty> validator, Func<T, bool> when);
}

/// <summary>Builder de regras para coleções (RuleForEach).</summary>
/// <typeparam name="T">O tipo validado.</typeparam>
/// <typeparam name="TItem">O tipo dos itens da coleção.</typeparam>
public interface ICollectionRuleBuilderInitial<T, TItem> where T : notnull
{
    /// <summary>Aplica um validador completo a cada item da coleção.</summary>
    void SetValidator(IValidator<TItem> validator);

    /// <summary>Aplica regras inline a cada item da coleção.</summary>
    void ChildRules(Action<Validator<TItem>> childRules);

    /// <summary>Aplica regras inline a cada item apenas quando a condição for verdadeira para a instância raiz.</summary>
    void ChildRulesWhen(Func<T, bool> condition, Action<Validator<TItem>> childRules);
}
