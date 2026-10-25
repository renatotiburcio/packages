namespace XForge.Validation;

/// <summary>Severidade de uma falha de validação.</summary>
public enum Severity
{
    /// <summary>Falha bloqueante (default).</summary>
    Error,

    /// <summary>Aviso — não bloqueia, mas é reportado.</summary>
    Warning,

    /// <summary>Informativo.</summary>
    Info,
}
