namespace XForge.Validation;

/// <summary>
/// Uma falha de validação: membro, mensagem, código padronizado, severidade e valor tentado.
/// </summary>
public sealed class ValidationFailure
{
    /// <summary>Cria a falha.</summary>
    public ValidationFailure(string propertyName, string errorMessage,
        string errorCode = "CUSTOM", Severity severity = Severity.Error, object? attemptedValue = null)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Severity = severity;
        AttemptedValue = attemptedValue;
    }

    /// <summary>Nome do membro validado (para regras de coleção: <c>Itens[0].Produto</c>).</summary>
    public string PropertyName { get; }

    /// <summary>Mensagem de erro formatada.</summary>
    public string ErrorMessage { get; }

    /// <summary>Código padronizado e estável (ex.: <c>NOT_NULL</c>, <c>LENGTH</c>, <c>EMAIL</c>).</summary>
    public string ErrorCode { get; }

    /// <summary>Severidade da falha.</summary>
    public Severity Severity { get; }

    /// <summary>O valor que falhou na validação (quando capturável sem custo).</summary>
    public object? AttemptedValue { get; }

    /// <inheritdoc />
    public override string ToString() => $"{PropertyName}: {ErrorMessage}";
}
