namespace XForge.Validation;

/// <summary>Modo de cascata: o que acontece após a primeira falha de uma cadeia de regras.</summary>
public enum CascadeMode
{
    /// <summary>Continua executando as próximas regras da cadeia.</summary>
    Continue,

    /// <summary>Interrompe a cadeia na primeira falha (regras seguintes do mesmo membro não rodam). Default.</summary>
    StopOnFirstFailure,
}

/// <summary>Para quem a condição When/Unless se aplica.</summary>
public enum ApplyConditionTo
{
    /// <summary>Apenas o validador atual da cadeia.</summary>
    CurrentValidator,

    /// <summary>Todos os validadores da cadeia.</summary>
    AllValidators,
}
