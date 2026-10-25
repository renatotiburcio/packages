using System.Text.RegularExpressions;

namespace XForge.Validation;

/// <summary>Validações encadeadas sobre um argumento (retornado por <see cref="Ensure.Arg{T}"/>).</summary>
public readonly struct EnsureArg<T>
{
    private readonly T _value;
    private readonly string _parameterName;

    internal EnsureArg(T value, string parameterName)
    {
        _value = value;
        _parameterName = parameterName;
    }

    private GuardException Fail(string rule) =>
        new($"Parameter '{_parameterName}' failed validation: {rule}.", _parameterName);

    /// <summary>O argumento não pode ser null.</summary>
    public EnsureArg<T> NotNull()
    {
        if (_value is null)
        {
            throw Fail("must not be null (NOT_NULL)");
        }

        return this;
    }

    /// <summary>O argumento não pode ser null, string vazia ou coleção vazia.</summary>
    public EnsureArg<T> NotEmpty()
    {
        if (IsEmpty(_value))
        {
            throw Fail("must not be empty (NOT_EMPTY)");
        }

        return this;
    }

    /// <summary>O argumento (string) não pode ser null, vazio ou apenas espaços.</summary>
    public EnsureArg<T> NotNullOrWhiteSpace()
    {
        if (_value is string text && string.IsNullOrWhiteSpace(text))
        {
            throw Fail("must not be null or whitespace (NOT_WHITESPACE)");
        }

        if (_value is null)
        {
            throw Fail("must not be null (NOT_NULL)");
        }

        return this;
    }

    /// <summary>Comprimento de string/coleção entre min e max (inclusivos).</summary>
    public EnsureArg<T> Length(int min, int max)
    {
        if (TryGetLength(_value, out var length) && (length < min || length > max))
        {
            throw Fail($"length must be between {min} and {max}, but was {length} (LENGTH)");
        }

        return this;
    }

    /// <summary>Valor maior que o limite informado (IComparable).</summary>
    public EnsureArg<T> GreaterThan(T limit)
    {
        var result = Compare(_value, limit);

        if (result is null || result <= 0)
        {
            throw Fail($"must be greater than {limit} (GREATER_THAN)");
        }

        return this;
    }

    /// <summary>Valor maior ou igual ao limite informado.</summary>
    public EnsureArg<T> GreaterThanOrEqualTo(T limit)
    {
        var result = Compare(_value, limit);

        if (result is null || result < 0)
        {
            throw Fail($"must be greater than or equal to {limit} (GREATER_THAN_OR_EQUAL)");
        }

        return this;
    }

    /// <summary>Valor menor que o limite informado.</summary>
    public EnsureArg<T> LessThan(T limit)
    {
        var result = Compare(_value, limit);

        if (result is null || result >= 0)
        {
            throw Fail($"must be less than {limit} (LESS_THAN)");
        }

        return this;
    }

    /// <summary>Valor no intervalo inclusivo [from, to].</summary>
    public EnsureArg<T> InRange(T from, T to)
    {
        var fromResult = Compare(_value, from);
        var toResult = Compare(_value, to);

        if (fromResult is null || fromResult < 0 || toResult is null || toResult > 0)
        {
            throw Fail($"must be between {from} and {to} (IN_RANGE)");
        }

        return this;
    }

    /// <summary>A string corresponde à expressão regular.</summary>
    public EnsureArg<T> Matches(string pattern)
    {
        if (_value is string text && !Regex.IsMatch(text, pattern, RegexOptions.Compiled))
        {
            throw Fail("does not match the required format (REGEX)");
        }

        return this;
    }

    /// <summary>Valor está entre as opções informadas.</summary>
    public EnsureArg<T> IsIn(params T[] validValues)
    {
        if (_value is null)
        {
            throw Fail("must not be null (IS_IN)");
        }

        if (Array.IndexOf(validValues, _value) < 0)
        {
            throw Fail("must be one of the allowed values (IS_IN)");
        }

        return this;
    }

    private static bool IsEmpty(T value)
    {
        return value switch
        {
            null => true,
            string text => text.Length == 0,
            System.Collections.ICollection collection => collection.Count == 0,
            _ => false,
        };
    }

    private static bool TryGetLength(T value, out int length)
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

    private static int? Compare(T left, T right)
    {
        if (left is null || right is null)
        {
            return null;
        }

        if (left is IComparable comparable && comparable.GetType() == typeof(T))
        {
            return comparable.CompareTo(right);
        }

        return null;
    }
}
