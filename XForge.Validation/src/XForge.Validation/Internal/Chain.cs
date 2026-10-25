namespace XForge.Validation.Internal;

/// <summary>Estado mutável de um componente de regra (permite WithMessage/WithErrorCode/WithSeverity pós-criação).</summary>
internal sealed class ComponentState
{
    public ComponentState(string propertyName, string messageTemplate, string errorCode)
    {
        PropertyName = propertyName;
        MessageTemplate = messageTemplate;
        ErrorCode = errorCode;
    }

    public string PropertyName { get; }
    public string MessageTemplate { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public Severity Severity { get; set; } = Severity.Error;
    public object? ComparisonValue { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    public Dictionary<string, object?> Placeholders { get; } = new(StringComparer.OrdinalIgnoreCase);
}

/// <summary>Entrada executável dentro de uma cadeia de regras.</summary>
internal sealed class ChainEntry<T>
{
    /// <summary>Condição específica deste componente (When CurrentValidator).</summary>
    public Func<T, bool>? Condition;

    /// <summary>Runner síncrono: retorna falhas ou null quando passa.</summary>
    public Func<T, List<ValidationFailure>?>? Sync;

    /// <summary>Runner assíncrono (quando a cadeia possui regras async).</summary>
    public Func<T, CancellationToken, ValueTask<List<ValidationFailure>?>>? Async;
}

/// <summary>Cadeia de regras para um membro.</summary>
internal sealed class ValidationChain<T>
{
    public ValidationChain(string propertyName, string[] ruleSets, Func<T, bool>? condition, CascadeMode cascade)
    {
        PropertyName = propertyName;
        RuleSets = ruleSets;
        Condition = condition;
        Cascade = cascade;
    }

    public string PropertyName { get; }
    public string[] RuleSets { get; }
    public Func<T, bool>? Condition { get; set; }
    public CascadeMode Cascade { get; set; }
    public List<ChainEntry<T>> Entries { get; } = new();
    public bool HasAsyncEntries { get; set; }

    /// <summary>Executa a cadeia (somente entradas síncronas). Retorna null quando passa.</summary>
    public List<ValidationFailure>? RunSync(T instance)
    {
        if (Condition is not null && !Condition(instance))
        {
            return null;
        }

        List<ValidationFailure>? failures = null;

        foreach (var entry in Entries)
        {
            if (entry.Sync is null)
            {
                continue;
            }

            if (entry.Condition is not null && !entry.Condition(instance))
            {
                continue;
            }

            var entryFailures = entry.Sync(instance);

            if (entryFailures is not null && entryFailures.Count > 0)
            {
                failures ??= new List<ValidationFailure>();
                failures.AddRange(entryFailures);

                if (Cascade == CascadeMode.StopOnFirstFailure)
                {
                    break;
                }
            }
        }

        return failures;
    }
}
