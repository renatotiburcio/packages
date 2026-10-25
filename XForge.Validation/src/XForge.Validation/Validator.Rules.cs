using System.Text.RegularExpressions;

using System.Linq.Expressions;
using XForge.Validation.Internal;

namespace XForge.Validation;

public partial class Validator<T> where T : notnull
{
    /// <summary>Builder fluente das regras de um membro.</summary>
    internal sealed class RuleBuilder<TProperty> :
        IRuleBuilderInitial<T, TProperty>,
        IRuleBuilderOptions<T, TProperty>
    {
        private readonly ValidationChain<T> _chain;
        private readonly Func<T, TProperty> _getter;
        private ChainEntry<T> _lastEntry = null!;
        private ComponentState _lastState = null!;

        public RuleBuilder(ValidationChain<T> chain, Func<T, TProperty> getter)
        {
            _chain = chain;
            _getter = getter;
        }

        private void AddSyncComponent(
            string errorCode, string defaultTemplate,
            Func<T, TProperty, ComponentState, ValidationFailure?> runner)
        {
            var state = new ComponentState(_chain.PropertyName, defaultTemplate, errorCode);
            _lastState = state;

            _lastEntry = new ChainEntry<T>
            {
                Sync = instance =>
                {
                    var value = _getter(instance);
                    var failure = runner(instance, value, state);

                    if (failure is not null)
                    {
                        return new List<ValidationFailure> { failure };
                    }

                    return null;
                },
            };

            _chain.Entries.Add(_lastEntry);
        }

        private void AddAsyncComponent(
            string errorCode, string defaultTemplate,
            Func<T, TProperty, CancellationToken, ComponentState, ValueTask<ValidationFailure?>> runner)
        {
            var state = new ComponentState(_chain.PropertyName, defaultTemplate, errorCode);
            _lastState = state;
            _chain.HasAsyncEntries = true;

            _lastEntry = new ChainEntry<T>
            {
                Async = async (instance, ct) =>
                {
                    var value = _getter(instance);
                    var failure = await runner(instance, value, ct, state);

                    if (failure is not null)
                    {
                        return new List<ValidationFailure> { failure };
                    }

                    return null;
                },
            };

            _chain.Entries.Add(_lastEntry);
        }

        private ValidationFailure CreateFailure(ComponentState state, TProperty value)
        {
            state.Placeholders["PropertyName"] = state.PropertyName;
            state.Placeholders["PropertyValue"] = value;

            var message = Internal.MessageFormatter.Format(state.MessageTemplate, state.Placeholders);
            return new ValidationFailure(state.PropertyName, message, state.ErrorCode, state.Severity, value);
        }

        private void ApplyCustomMessage(string? message)
        {
            if (message is not null)
            {
                _lastState.MessageTemplate = message;
            }
        }

        public IRuleBuilderOptions<T, TProperty> NotNull(string? message = null)
        {
            AddSyncComponent("NOT_NULL", "'{PropertyName}' must not be empty.", (instance, value, state) =>
                value is null ? CreateFailure(state, value) : null);
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> Null(string? message = null)
        {
            AddSyncComponent("NULL", "'{PropertyName}' must be empty.", (instance, value, state) =>
                value is not null ? CreateFailure(state, value) : null);
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> NotEmpty(string? message = null)
        {
            AddSyncComponent("NOT_EMPTY", "'{PropertyName}' must not be empty.", (instance, value, state) =>
                IsEmpty(value) ? CreateFailure(state, value) : null);
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> Empty(string? message = null)
        {
            AddSyncComponent("EMPTY", "'{PropertyName}' must be empty.", (instance, value, state) =>
                !IsEmpty(value) ? CreateFailure(state, value) : null);
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> Length(int min, int max, string? message = null)
        {
            AddSyncComponent("LENGTH",
                $"'{{PropertyName}}' must be between {min} and {max} characters. You entered {{TotalLength}} characters.",
                (instance, value, state) =>
                {
                    if (value is null || TryGetLength(value, out var length) == false)
                    {
                        return null; // null/não-contável é responsabilidade de NotNull/NotEmpty
                    }

                    state.Min = min;
                    state.Max = max;
                    state.Placeholders["MinLength"] = min;
                    state.Placeholders["MaxLength"] = max;
                    state.Placeholders["TotalLength"] = length;

                    return length >= min && length <= max ? null : CreateFailure(state, value);
                });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> MaxLength(int max, string? message = null)
        {
            AddSyncComponent("MAX_LENGTH",
                $"'{{PropertyName}}' must not exceed {max} characters. You entered {{TotalLength}} characters.",
                (instance, value, state) =>
                {
                    if (value is null || TryGetLength(value, out var length) == false)
                    {
                        return null;
                    }

                    state.Max = max;
                    state.Placeholders["MaxLength"] = max;
                    state.Placeholders["TotalLength"] = length;

                    return length <= max ? null : CreateFailure(state, value);
                });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> MinLength(int min, string? message = null)
        {
            AddSyncComponent("MIN_LENGTH",
                $"'{{PropertyName}}' must be at least {min} characters. You entered {{TotalLength}} characters.",
                (instance, value, state) =>
                {
                    if (value is null || TryGetLength(value, out var length) == false)
                    {
                        return null;
                    }

                    state.Min = min;
                    state.Placeholders["MinLength"] = min;
                    state.Placeholders["TotalLength"] = length;

                    return length >= min ? null : CreateFailure(state, value);
                });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> ExactLength(int length, string? message = null)
        {
            AddSyncComponent("EXACT_LENGTH",
                $"'{{PropertyName}}' must be {length} characters. You entered {{TotalLength}} characters.",
                (instance, value, state) =>
                {
                    if (value is null || TryGetLength(value, out var actual) == false)
                    {
                        return null;
                    }

                    state.Placeholders["TotalLength"] = actual;

                    return actual == length ? null : CreateFailure(state, value);
                });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> EmailAddress(string? message = null)
        {
            AddSyncComponent("EMAIL", "'{PropertyName}' is not a valid email address.", (instance, value, state) =>
            {
                if (value is not string email || string.IsNullOrEmpty(email))
                {
                    return null;
                }

                try
                {
                    _ = new System.Net.Mail.MailAddress(email);
                    return null;
                }
                catch (FormatException)
                {
                    return CreateFailure(state, value);
                }
                catch (ArgumentException)
                {
                    return CreateFailure(state, value);
                }
            });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> Matches(string pattern, string? message = null)
        {
            var regex = new Regex(pattern, RegexOptions.Compiled);

            AddSyncComponent("REGEX", "'{PropertyName}' is not in the correct format.", (instance, value, state) =>
            {
                if (value is not string text)
                {
                    return null;
                }

                state.Placeholders["RegularExpression"] = pattern;

                return regex.IsMatch(text) ? null : CreateFailure(state, value);
            });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> Matches(string pattern, RegexOptions options, string? message = null)
        {
            var regex = new Regex(pattern, options | RegexOptions.Compiled);

            AddSyncComponent("REGEX", "'{PropertyName}' is not in the correct format.", (instance, value, state) =>
            {
                if (value is not string text)
                {
                    return null;
                }

                state.Placeholders["RegularExpression"] = pattern;

                return regex.IsMatch(text) ? null : CreateFailure(state, value);
            });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> Equal(TProperty value, string? message = null)
        {
            AddSyncComponent("EQUAL", "'{PropertyName}' must be equal to '{ComparisonValue}'.", (instance, current, state) =>
            {
                state.ComparisonValue = value;
                state.Placeholders["ComparisonValue"] = value;

                return EqualityComparer<TProperty>.Default.Equals(current, value)
                    ? null
                    : CreateFailure(state, current);
            });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> NotEqual(TProperty value, string? message = null)
        {
            AddSyncComponent("NOT_EQUAL", "'{PropertyName}' must not be equal to '{ComparisonValue}'.", (instance, current, state) =>
            {
                state.ComparisonValue = value;
                state.Placeholders["ComparisonValue"] = value;

                return !EqualityComparer<TProperty>.Default.Equals(current, value)
                    ? null
                    : CreateFailure(state, current);
            });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> Equal(Expression<Func<T, TProperty>> otherMember, string? message = null)
        {
            var otherGetter = otherMember.Compile();
            var otherName = MemberName(otherMember);

            AddSyncComponent("EQUAL", $"'{{PropertyName}}' must be equal to '{otherName}'.", (instance, current, state) =>
            {
                var other = otherGetter(instance);
                state.ComparisonValue = other;
                state.Placeholders["ComparisonValue"] = other;

                return EqualityComparer<TProperty>.Default.Equals(current, other)
                    ? null
                    : CreateFailure(state, current);
            });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> NotEqual(Expression<Func<T, TProperty>> otherMember, string? message = null)
        {
            var otherGetter = otherMember.Compile();
            var otherName = MemberName(otherMember);

            AddSyncComponent("NOT_EQUAL", $"'{{PropertyName}}' must not be equal to '{otherName}'.", (instance, current, state) =>
            {
                var other = otherGetter(instance);
                state.ComparisonValue = other;
                state.Placeholders["ComparisonValue"] = other;

                return !EqualityComparer<TProperty>.Default.Equals(current, other)
                    ? null
                    : CreateFailure(state, current);
            });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> Must(Func<TProperty, bool> predicate, string? message = null)
        {
            AddSyncComponent("MUST", "The specified condition was not met for '{PropertyName}'.", (instance, value, state) =>
                predicate(value) ? null : CreateFailure(state, value));
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> Must(Func<T, TProperty, bool> predicate, string? message = null)
        {
            AddSyncComponent("MUST", "The specified condition was not met for '{PropertyName}'.", (instance, value, state) =>
                predicate(instance, value) ? null : CreateFailure(state, value));
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> MustAsync(
            Func<TProperty, CancellationToken, ValueTask<bool>> predicate, string? message = null)
        {
            AddAsyncComponent("MUST", "The specified condition was not met for '{PropertyName}'.",
                async (instance, value, ct, state) =>
                {
                    var ok = await predicate(value, ct);
                    return ok ? null : CreateFailure(state, value);
                });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> MustAsync(
            Func<T, TProperty, CancellationToken, ValueTask<bool>> predicate, string? message = null)
        {
            AddAsyncComponent("MUST", "The specified condition was not met for '{PropertyName}'.",
                async (instance, value, ct, state) =>
                {
                    var ok = await predicate(instance, value, ct);
                    return ok ? null : CreateFailure(state, value);
                });
            ApplyCustomMessage(message);
            return this;
        }

        private void AddComparison(string errorCode, string template, Func<int, bool> passes, TProperty comparisonValue)
        {
            AddSyncComponent(errorCode, template, (instance, value, state) =>
            {
                state.ComparisonValue = comparisonValue;
                state.Placeholders["ComparisonValue"] = comparisonValue;

                if (value is null)
                {
                    return null; // null é responsabilidade de NotNull
                }

                if (value is not IComparable comparable)
                {
                    throw new InvalidOperationException(
                        $"Property '{_chain.PropertyName}' does not implement IComparable and cannot use comparison validators.");
                }

                var result = comparable.CompareTo(comparisonValue);
                return passes(result) ? null : CreateFailure(state, value);
            });
        }

        public IRuleBuilderOptions<T, TProperty> LessThan(TProperty value, string? message = null)
        {
            AddComparison("LESS_THAN", "'{PropertyName}' must be less than '{ComparisonValue}'.",
                result => result < 0, value);
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> LessThanOrEqualTo(TProperty value, string? message = null)
        {
            AddComparison("LESS_THAN_OR_EQUAL", "'{PropertyName}' must be less than or equal to '{ComparisonValue}'.",
                result => result <= 0, value);
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> GreaterThan(TProperty value, string? message = null)
        {
            AddComparison("GREATER_THAN", "'{PropertyName}' must be greater than '{ComparisonValue}'.",
                result => result > 0, value);
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> GreaterThanOrEqualTo(TProperty value, string? message = null)
        {
            AddComparison("GREATER_THAN_OR_EQUAL", "'{PropertyName}' must be greater than or equal to '{ComparisonValue}'.",
                result => result >= 0, value);
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> InclusiveBetween(TProperty from, TProperty to, string? message = null)
        {
            AddSyncComponent("INCLUSIVE_BETWEEN",
                "'{PropertyName}' must be between {From} and {To}. You entered {PropertyValue}.",
                (instance, value, state) =>
                {
                    state.Placeholders["From"] = from;
                    state.Placeholders["To"] = to;

                    if (value is null)
                    {
                        return null;
                    }

                    if (value is not IComparable comparable)
                    {
                        throw new InvalidOperationException(
                            $"Property '{_chain.PropertyName}' does not implement IComparable and cannot use range validators.");
                    }

                    return comparable.CompareTo(from) >= 0 && comparable.CompareTo(to) <= 0
                        ? null
                        : CreateFailure(state, value);
                });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> ExclusiveBetween(TProperty from, TProperty to, string? message = null)
        {
            AddSyncComponent("EXCLUSIVE_BETWEEN",
                "'{PropertyName}' must be between {From} and {To} (exclusive). You entered {PropertyValue}.",
                (instance, value, state) =>
                {
                    state.Placeholders["From"] = from;
                    state.Placeholders["To"] = to;

                    if (value is null)
                    {
                        return null;
                    }

                    if (value is not IComparable comparable)
                    {
                        throw new InvalidOperationException(
                            $"Property '{_chain.PropertyName}' does not implement IComparable and cannot use range validators.");
                    }

                    return comparable.CompareTo(from) > 0 && comparable.CompareTo(to) < 0
                        ? null
                        : CreateFailure(state, value);
                });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> IsIn(params TProperty[] validValues)
        {
            var validSet = new HashSet<TProperty>(validValues);

            AddSyncComponent("IS_IN", "'{PropertyName}' must be one of the allowed values.", (instance, value, state) =>
                value is null || validSet.Contains(value) ? null : CreateFailure(state, value));
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> IsInEnum(string? message = null)
        {
            AddSyncComponent("IS_IN_ENUM", "'{PropertyName}' has a range of values which does not include '{PropertyValue}'.",
                (instance, value, state) =>
                {
                    if (value is null)
                    {
                        return null;
                    }

                    if (typeof(TProperty).IsEnum == false)
                    {
                        throw new InvalidOperationException(
                            $"IsInEnum requires '{typeof(TProperty)}' to be an enum type.");
                    }

                    return Enum.IsDefined(typeof(TProperty), value) ? null : CreateFailure(state, value);
                });
            ApplyCustomMessage(message);
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> WithMessage(string message)
        {
            _lastState.MessageTemplate = message ?? throw new ArgumentNullException(nameof(message));
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> WithErrorCode(string errorCode)
        {
            _lastState.ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> WithSeverity(Severity severity)
        {
            _lastState.Severity = severity;
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> WithSeverity(Func<T, TProperty, Severity> severityFactory)
        {
            var factory = severityFactory ?? throw new ArgumentNullException(nameof(severityFactory));
            var previous = _lastEntry.Sync;
            var getter = _getter;

            _lastEntry.Sync = instance =>
            {
                var failures = previous?.Invoke(instance);

                if (failures is null || failures.Count == 0)
                {
                    return failures;
                }

                var value = getter(instance);
                var severity = factory(instance, value);

                return failures
                    .Select(failure => new ValidationFailure(
                        failure.PropertyName, failure.ErrorMessage, failure.ErrorCode, severity, failure.AttemptedValue))
                    .ToList();
            };

            return this;
        }

        public IRuleBuilderOptions<T, TProperty> When(Func<T, bool> predicate, ApplyConditionTo applyTo = ApplyConditionTo.CurrentValidator)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (applyTo == ApplyConditionTo.CurrentValidator)
            {
                _lastEntry.Condition = predicate;
            }
            else
            {
                var previous = _chain.Condition;
                _chain.Condition = previous is null
                    ? predicate
                    : instance => previous(instance) && predicate(instance);
            }

            return this;
        }

        public IRuleBuilderOptions<T, TProperty> Unless(Func<T, bool> predicate, ApplyConditionTo applyTo = ApplyConditionTo.CurrentValidator)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return When(instance => !predicate(instance), applyTo);
        }

        public IRuleBuilderOptions<T, TProperty> Cascade(CascadeMode cascadeMode)
        {
            _chain.Cascade = cascadeMode;
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> SetValidator(IValidator<TProperty> validator)
        {
            if (validator is null) throw new ArgumentNullException(nameof(validator));

            _chain.Entries.Remove(_lastEntry);

            var entry = new ChainEntry<T>
            {
                Sync = instance =>
                {
                    var value = _getter(instance);

                    if (value is null)
                    {
                        return null; // null é responsabilidade de NotNull
                    }

                    var result = validator.Validate(value);

                    if (result.IsValid)
                    {
                        return null;
                    }

                    // prefixa o caminho do membro aninhado: "Endereco.Cidade"
                    var failures = new List<ValidationFailure>(result.Errors.Count);

                    foreach (var failure in result.Errors)
                    {
                        failures.Add(new ValidationFailure(
                            $"{_chain.PropertyName}.{failure.PropertyName}",
                            failure.ErrorMessage,
                            failure.ErrorCode,
                            failure.Severity,
                            failure.AttemptedValue));
                    }

                    return failures;
                },
            };

            _chain.Entries.Add(entry);
            _lastEntry = entry;
            return this;
        }

        public IRuleBuilderOptions<T, TProperty> SetValidator(IValidator<TProperty> validator, Func<T, bool> when)
        {
            if (validator is null) throw new ArgumentNullException(nameof(validator));
            if (when is null) throw new ArgumentNullException(nameof(when));

            SetValidator(validator);
            _lastEntry.Condition = when;
            return this;
        }

        private static string MemberName(Expression<Func<T, TProperty>> expression)
        {
            var body = expression.Body;
            while (body.NodeType is ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.Quote)
            {
                body = ((UnaryExpression)body).Operand;
            }

            return body is MemberExpression member ? member.Member.Name : string.Empty;
        }

        private static bool IsEmpty(TProperty value)
        {
            return value switch
            {
                null => true,
                string text => text.Length == 0,
                System.Collections.ICollection collection => collection.Count == 0,
                _ => false,
            };
        }

        private static bool TryGetLength(TProperty value, out int length)
        {
            switch (value)
            {
                case string text:
                    length = text.Length;
                    return true;
                case System.Collections.ICollection collection:
                    length = collection.Count;
                    return true;
                default:
                    length = 0;
                    return false;
            }
        }
    }
}
