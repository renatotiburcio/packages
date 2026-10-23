namespace XForge.Mapper;

/// <summary>
/// Thrown when a mapping fails at runtime (converter error, null destination member, depth exceeded, etc.).
/// </summary>
public class MappingException : Exception
{
    /// <summary>The source value being mapped when the failure occurred, when known.</summary>
    public object? SourceValue { get; }

    /// <summary>The mapping path (member chain) where the failure occurred, when known (e.g. "Pessoa.Endereco.Cidade").</summary>
    public string? MappingPath { get; }

    /// <summary>Creates the exception.</summary>
    public MappingException(string message, Exception? innerException = null,
        object? sourceValue = null, string? mappingPath = null)
        : base(message, innerException)
    {
        SourceValue = sourceValue;
        MappingPath = mappingPath;
    }
}

/// <summary>
/// Thrown by <see cref="MapperConfiguration.Validate"/> when the configuration has problems
/// (unmapped destination members with <c>RequireDestinationMemberSource</c>, missing constructors, etc.).
/// </summary>
public class MappingValidationException : Exception
{
    /// <summary>The individual validation problems, one per line item.</summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>Creates the exception.</summary>
    public MappingValidationException(IReadOnlyList<string> errors)
        : base(BuildMessage(errors))
    {
        Errors = errors;
    }

    private static string BuildMessage(IReadOnlyList<string> errors)
    {
        return $"Mapping configuration has {errors.Count} validation error(s):" +
               Environment.NewLine + string.Join(Environment.NewLine, errors.Select(e => " - " + e));
    }
}
