namespace XForge.Validation;

/// <summary>
/// Resultado de uma validação: lista imutável de falhas + helpers de acesso.
/// </summary>
public sealed class ValidationResult
{
    /// <summary>O resultado de sucesso (sem falhas) — instância compartilhada.</summary>
    public static ValidationResult Success { get; } = new(Array.Empty<ValidationFailure>());

    /// <summary>Cria o resultado com as falhas informadas.</summary>
    public ValidationResult(IEnumerable<ValidationFailure> errors)
    {
        Errors = (errors ?? throw new ArgumentNullException(nameof(errors))).ToArray();
    }

    /// <summary><see langword="true"/> quando não há falhas com severidade <see cref="Severity.Error"/>.</summary>
    public bool IsValid => !HasErrors();

    /// <summary>Todas as falhas (todas as severidades).</summary>
    public IReadOnlyList<ValidationFailure> Errors { get; }

    /// <summary>Falhas com severidade <see cref="Severity.Error"/> (as bloqueantes).</summary>
    public IEnumerable<ValidationFailure> ErrorsWithSeverity(Severity severity)
        => Errors.Where(e => e.Severity == severity);

    /// <summary>Falhas de um membro específico.</summary>
    public IEnumerable<ValidationFailure> ErrorsFor(string propertyName)
        => Errors.Where(e => string.Equals(e.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase));

    /// <summary>Obtém a primeira mensagem de erro de um membro (ou <see langword="null"/>).</summary>
    public string? GetError(string propertyName)
        => ErrorsFor(propertyName).FirstOrDefault()?.ErrorMessage;

    /// <summary>Dicionário membro → mensagens (formato binds de UI).</summary>
    public Dictionary<string, string[]> ToDictionary()
    {
        var dictionary = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var failure in Errors)
        {
            if (dictionary.TryGetValue(failure.PropertyName, out var messages))
            {
                var list = messages.ToList();
                list.Add(failure.ErrorMessage);
                dictionary[failure.PropertyName] = list.ToArray();
            }
            else
            {
                dictionary[failure.PropertyName] = new[] { failure.ErrorMessage };
            }
        }

        return dictionary;
    }

    private bool HasErrors()
    {
        foreach (var failure in Errors)
        {
            if (failure.Severity == Severity.Error)
            {
                return true;
            }
        }

        return false;
    }
}
