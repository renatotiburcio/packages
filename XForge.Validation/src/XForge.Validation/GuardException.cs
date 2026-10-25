using XForge.Validation.Internal;

namespace XForge.Validation;

/// <summary>
/// Lançada pela Ensure API quando um argumento viola uma regra.
/// </summary>
public class GuardException : Exception
{
    /// <summary>Nome do parâmetro que falhou (quando aplicável).</summary>
    public string? ParameterName { get; }

    /// <summary>Cria a exceção.</summary>
    public GuardException(string message, string? parameterName = null)
        : base(message)
    {
        ParameterName = parameterName;
    }
}
