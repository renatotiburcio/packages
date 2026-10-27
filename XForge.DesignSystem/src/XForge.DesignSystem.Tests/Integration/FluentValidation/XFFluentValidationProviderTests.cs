// T048: Tests for XFFluentValidationProvider.
using Xunit;
using XForge.DesignSystem.Integration.FluentValidation;

namespace XForge.DesignSystem.Tests.Integration.FluentValidation;

public class XFNullFluentValidationProviderTests
{
    private readonly IXFFluentValidationProvider p = new XFNullFluentValidationProvider();

    [Fact] public void ResolveValidator_ReturnsNull() => Assert.Null(p.ResolveValidator<string>());

    [Fact] public void HasValidator_ReturnsFalse() => Assert.False(p.HasValidator<int>());
}

public class XFValidationResultTests
{
    [Fact] public void Valid_IsValidAndEmpty()
    {
        Assert.True(XFValidationResult.Valid.IsValid);
        Assert.Empty(XFValidationResult.Valid.Errors);
    }

    [Fact] public void Invalid_StoresErrors()
    {
        var r = XFValidationResult.Invalid("a", "b");
        Assert.False(r.IsValid);
        Assert.Equal(2, r.Errors.Count);
    }
}