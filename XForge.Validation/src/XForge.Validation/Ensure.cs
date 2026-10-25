namespace XForge.Validation;

/// <summary>
/// Guard clauses fluentes para argumentos de métodos e pré-condições.
/// Lançam <see cref="GuardException"/> imediatamente na primeira violação (fail-fast).
/// Complementa o <see cref="Validator{T}"/>: use o Validator para DTOs/modelos e o Ensure para
/// argumentos de métodos e invariantes.
/// </summary>
public static class Ensure
{
    /// <summary>Inicia a validação de um argumento: <c>Ensure.Arg(value, nameof(value)).NotNull();</c></summary>
    public static EnsureArg<T> Arg<T>(T value, string parameterName) => new(value, parameterName);

    /// <summary>Pré-condição: lança <see cref="GuardException"/> quando a condição for falsa.</summary>
    public static void That(bool condition, string message, string? parameterName = null)
    {
        if (!condition)
        {
            throw new GuardException(message, parameterName);
        }
    }

    /// <summary>Pré-condição inversa: lança <see cref="GuardException"/> quando a condição for verdadeira.</summary>
    public static void Against(bool condition, string message, string? parameterName = null)
    {
        if (condition)
        {
            throw new GuardException(message, parameterName);
        }
    }
}
