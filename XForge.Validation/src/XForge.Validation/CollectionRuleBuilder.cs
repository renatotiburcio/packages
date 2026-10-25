using System.Linq.Expressions;
using XForge.Validation.Internal;

namespace XForge.Validation;

/// <summary>Builder de regras para coleções (RuleForEach).</summary>
internal sealed class CollectionRuleBuilder<T, TItem> : ICollectionRuleBuilderInitial<T, TItem>
    where T : notnull
{
    private readonly ValidationChain<T> _chain;
    private readonly Func<T, IEnumerable<TItem>> _getter;

    public CollectionRuleBuilder(ValidationChain<T> chain, Func<T, IEnumerable<TItem>> getter)
    {
        _chain = chain;
        _getter = getter;
    }

    /// <inheritdoc />
    public void SetValidator(IValidator<TItem> validator)
    {
        if (validator is null) throw new ArgumentNullException(nameof(validator));

        _chain.Entries.Add(new ChainEntry<T>
        {
            Sync = instance =>
            {
                var collection = _getter(instance);

                if (collection is null)
                {
                    return null;
                }

                var failures = new List<ValidationFailure>();
                var index = 0;

                foreach (var item in collection)
                {
                    var result = validator.Validate(item);

                    foreach (var failure in result.Errors)
                    {
                        failures.Add(new ValidationFailure(
                            $"{_chain.PropertyName}[{index}].{failure.PropertyName}",
                            failure.ErrorMessage,
                            failure.ErrorCode,
                            failure.Severity,
                            failure.AttemptedValue));
                    }

                    index++;
                }

                return failures.Count > 0 ? failures : null;
            },
        });
    }

    /// <inheritdoc />
    public void ChildRules(Action<Validator<TItem>> childRules)
    {
        ChildRulesWhen(static _ => true, childRules);
    }

    /// <inheritdoc />
    public void ChildRulesWhen(Func<T, bool> condition, Action<Validator<TItem>> childRules)
    {
        if (condition is null) throw new ArgumentNullException(nameof(condition));
        if (childRules is null) throw new ArgumentNullException(nameof(childRules));

        var childValidator = new Validator<TItem>();
        childRules(childValidator);

        SetValidatorWhen(condition, childValidator);
    }

    private void SetValidatorWhen(Func<T, bool> condition, IValidator<TItem> validator)
    {
        _chain.Entries.Add(new ChainEntry<T>
        {
            Condition = condition,
            Sync = instance =>
            {
                var collection = _getter(instance);

                if (collection is null)
                {
                    return null;
                }

                var failures = new List<ValidationFailure>();
                var index = 0;

                foreach (var item in collection)
                {
                    var result = validator.Validate(item);

                    foreach (var failure in result.Errors)
                    {
                        failures.Add(new ValidationFailure(
                            $"{_chain.PropertyName}[{index}].{failure.PropertyName}",
                            failure.ErrorMessage,
                            failure.ErrorCode,
                            failure.Severity,
                            failure.AttemptedValue));
                    }

                    index++;
                }

                return failures.Count > 0 ? failures : null;
            },
        });
    }
}
