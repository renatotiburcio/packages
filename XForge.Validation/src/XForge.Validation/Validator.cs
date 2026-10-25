using System.Linq.Expressions;
using System.Reflection;
using XForge.Validation.Internal;

namespace XForge.Validation;

/// <summary>
/// Validador fluente para <typeparamref name="T"/>: construa as regras no construtor com
/// <c>RuleFor</c>/<c>RuleForEach</c> e valide com <c>Validate</c>/<c>ValidateAsync</c>.
/// Thread-safe após a configuração.
/// </summary>
public partial class Validator<T> : IValidator<T>, IValidator where T : notnull
{
    private readonly List<ValidationChain<T>> _chains = new();
    private readonly List<Func<T, bool>> _conditionScope = new();
    private readonly List<string[]> _ruleSetScope = new();
    private readonly List<IValidator<T>> _includedValidators = new();
    private readonly CascadeMode _defaultCascade;

    /// <summary>Cria o validador.</summary>
    /// <param name="cascade">Modo de cascata default das cadeias (default: StopOnFirstFailure).</param>
    public Validator(CascadeMode cascade = CascadeMode.StopOnFirstFailure)
    {
        _defaultCascade = cascade;
    }

    /// <inheritdoc />
    public bool CanValidateInstancesOfType(Type type)
    {
        return typeof(T).IsAssignableFrom(type);
    }

    /// <summary>Inicia uma cadeia de regras para um membro.</summary>
    public IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var (propertyName, getter) = CompileAccessor(expression);
        var chain = new ValidationChain<T>(propertyName, CurrentRuleSets(), CombinedCondition(), _defaultCascade);
        _chains.Add(chain);

        return (IRuleBuilderInitial<T, TProperty>)new Validator<T>.RuleBuilder<TProperty>(chain, getter);
    }

    /// <summary>Inicia regras para uma coleção de itens.</summary>
    public ICollectionRuleBuilderInitial<T, TItem> RuleForEach<TItem>(Expression<Func<T, IEnumerable<TItem>>> expression)
    {
        var (propertyName, getter) = CompileCollectionAccessor(expression);
        var chain = new ValidationChain<T>(propertyName, CurrentRuleSets(), CombinedCondition(), _defaultCascade);
        _chains.Add(chain);

        return new CollectionRuleBuilder<T, TItem>(chain, getter);
    }

    /// <summary>Agrupa as regras criadas dentro de action nos rulesets informados (vírgula para vários).</summary>
    public void RuleSet(string ruleSets, Action<Validator<T>> action)
    {
        if (string.IsNullOrWhiteSpace(ruleSets))
        {
            throw new ArgumentException("RuleSet name cannot be null or empty.", nameof(ruleSets));
        }

        var sets = ruleSets.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();
        _ruleSetScope.Add(sets);
        action(this);
        _ruleSetScope.RemoveAt(_ruleSetScope.Count - 1);
    }

    /// <summary>Condição aplicada a todas as regras criadas dentro de action.</summary>
    public void When(Func<T, bool> predicate, Action<Validator<T>> action)
    {
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));
        _conditionScope.Add(predicate);
        action(this);
        _conditionScope.RemoveAt(_conditionScope.Count - 1);
    }

    /// <summary>Inverso de When.</summary>
    public void Unless(Func<T, bool> predicate, Action<Validator<T>> action)
    {
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));
        When(instance => !predicate(instance), action);
    }

    /// <summary>Inclui as regras de outro validador do mesmo tipo.</summary>
    public void Include(IValidator<T> otherValidator)
    {
        if (otherValidator is null) throw new ArgumentNullException(nameof(otherValidator));
        _includedValidators.Add(otherValidator);
    }

    /// <summary>Inclui as regras de um validador de tipo base.</summary>
    public void Include<TOtherValidator>(TOtherValidator otherValidator)
        where TOtherValidator : IValidator<T>
    {
        if (otherValidator is null) throw new ArgumentNullException(nameof(otherValidator));
        _includedValidators.Add(otherValidator);
    }

    /// <inheritdoc />
    public ValidationResult Validate(T instance)
    {
        if (instance is null) throw new ArgumentNullException(nameof(instance));

        if (_chains.Any(chain => chain.HasAsyncEntries))
        {
            throw new InvalidOperationException(
                "This validator contains async rules. Use ValidateAsync instead of Validate.");
        }

        var failures = RunCore(instance, "default", CancellationToken.None).GetAwaiter().GetResult();
        return new ValidationResult(failures);
    }

    /// <inheritdoc />
    public ValueTask<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        if (instance is null) throw new ArgumentNullException(nameof(instance));
        return ValidateAsync(instance, "default", cancellationToken);
    }

    /// <inheritdoc />
    public ValidationResult Validate(T instance, string ruleSet)
    {
        if (instance is null) throw new ArgumentNullException(nameof(instance));

        if (_chains.Any(chain => chain.HasAsyncEntries))
        {
            throw new InvalidOperationException(
                "This validator contains async rules. Use ValidateAsync instead of Validate.");
        }

        var failures = RunCore(instance, ruleSet, CancellationToken.None).GetAwaiter().GetResult();
        return new ValidationResult(failures);
    }

    /// <inheritdoc />
    public async ValueTask<ValidationResult> ValidateAsync(T instance, string ruleSet, CancellationToken cancellationToken = default)
    {
        if (instance is null) throw new ArgumentNullException(nameof(instance));
        var failures = await RunCore(instance, ruleSet, cancellationToken);
        return new ValidationResult(failures);
    }

    /// <inheritdoc />
    ValidationResult IValidator.Validate(object instance)
    {
        if (instance is null) throw new ArgumentNullException(nameof(instance));
        return Validate((T)instance);
    }

    /// <inheritdoc />
    async ValueTask<ValidationResult> IValidator.ValidateAsync(object instance, CancellationToken cancellationToken)
    {
        if (instance is null) throw new ArgumentNullException(nameof(instance));
        return await ValidateAsync((T)instance, cancellationToken);
    }

    private async ValueTask<List<ValidationFailure>> RunCore(T instance, string ruleSet, CancellationToken cancellationToken)
    {
        var activeRuleSets = ParseRuleSets(ruleSet);
        var failures = new List<ValidationFailure>();

        foreach (var included in _includedValidators)
        {
            failures.AddRange((await included.ValidateAsync(instance, ruleSet, cancellationToken)).Errors);
        }

        foreach (var chain in _chains)
        {
            if (!ShouldRunChain(chain, activeRuleSets))
            {
                continue;
            }

            if (chain.Condition is not null && !chain.Condition(instance))
            {
                continue;
            }

            foreach (var entry in chain.Entries)
            {
                if (entry.Condition is not null && !entry.Condition(instance))
                {
                    continue;
                }

                if (entry.Sync is not null)
                {
                    var entryFailures = entry.Sync(instance);

                    if (entryFailures is not null && entryFailures.Count > 0)
                    {
                        failures.AddRange(entryFailures);

                        if (chain.Cascade == CascadeMode.StopOnFirstFailure)
                        {
                            break;
                        }
                    }
                }
                else if (entry.Async is not null)
                {
                    var entryFailures = await entry.Async(instance, cancellationToken);

                    if (entryFailures is not null && entryFailures.Count > 0)
                    {
                        failures.AddRange(entryFailures);

                        if (chain.Cascade == CascadeMode.StopOnFirstFailure)
                        {
                            break;
                        }
                    }
                }
            }
        }

        return failures;
    }

    private static HashSet<string> ParseRuleSets(string ruleSet)
    {
        return new HashSet<string>(
            ruleSet.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0),
            StringComparer.OrdinalIgnoreCase);
    }

    private static bool ShouldRunChain(ValidationChain<T> chain, HashSet<string> activeRuleSets)
    {
        return activeRuleSets.Contains("*") || chain.RuleSets.Any(activeRuleSets.Contains);
    }

    private string[] CurrentRuleSets()
    {
        return _ruleSetScope.Count > 0 ? _ruleSetScope[_ruleSetScope.Count - 1] : new[] { "default" };
    }

    private Func<T, bool>? CombinedCondition()
    {
        if (_conditionScope.Count == 0)
        {
            return null;
        }

        if (_conditionScope.Count == 1)
        {
            return _conditionScope[0];
        }

        var conditions = _conditionScope.ToArray();
        return instance =>
        {
            foreach (var condition in conditions)
            {
                if (!condition(instance))
                {
                    return false;
                }
            }

            return true;
        };
    }

    private static (string PropertyName, Func<T, TProperty> Getter) CompileAccessor<TProperty>(
        Expression<Func<T, TProperty>> expression)
    {
        var member = UnwrapMember(expression.Body)
            ?? throw new ArgumentException($"Expression must be a member access: '{expression}'.", nameof(expression));

        return (member.Name, expression.Compile());
    }

    private static (string PropertyName, Func<T, IEnumerable<TItem>> Getter) CompileCollectionAccessor<TItem>(
        Expression<Func<T, IEnumerable<TItem>>> expression)
    {
        var member = UnwrapMember(expression.Body)
            ?? throw new ArgumentException($"Expression must be a member access: '{expression}'.", nameof(expression));

        return (member.Name, expression.Compile());
    }

    private static MemberInfo? UnwrapMember(Expression body)
    {
        while (body.NodeType is ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.Quote)
        {
            body = ((UnaryExpression)body).Operand;
        }

        return body is MemberExpression member ? member.Member : null;
    }
}
