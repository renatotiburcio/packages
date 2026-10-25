namespace XForge.Validation;

/// <summary>Lançada por <c>ValidateAndThrow</c> quando a validação falha. Carrega todas as falhas.</summary>
public class ValidationException : Exception
{
    /// <summary>Cria a exceção com as falhas.</summary>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this(BuildMessage(failures), new ValidationResult(failures))
    {
    }

    /// <summary>Cria a exceção com mensagem e resultado.</summary>
    public ValidationException(string message, ValidationResult? result)
        : base(message)
    {
        Errors = result?.Errors ?? Array.Empty<ValidationFailure>();
    }

    /// <summary>Cria a exceção simples.</summary>
    public ValidationException(string message)
        : base(message)
    {
        Errors = Array.Empty<ValidationFailure>();
    }

    /// <summary>Todas as falhas que causaram a exceção.</summary>
    public IReadOnlyList<ValidationFailure> Errors { get; }

    private static string BuildMessage(IEnumerable<ValidationFailure> failures)
    {
        var list = failures.ToArray();
        return $"Validation failed: {list.Length} error(s). " + string.Join("; ", list.Select(f => f.ToString()));
    }
}
