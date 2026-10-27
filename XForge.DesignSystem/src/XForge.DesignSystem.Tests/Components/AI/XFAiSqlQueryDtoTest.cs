using Xunit;
using XForge.DesignSystem.Components.AI.Analysis;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for AI SQL Query Builder DTOs at 0% coverage: XFAiSqlQuery, XFAiSqlValidationResult.
/// </summary>
public sealed class XFAiSqlQueryDtoTest
{
    #region XFAiSqlQuery

    [Fact]
    public void XFAiSqlQuery_Defaults_AreCorrect()
    {
        var query = new XFAiSqlQuery();
        Assert.NotEmpty(query.Id);
        Assert.Equal(8, query.Id.Length);
        Assert.Equal(string.Empty, query.NaturalLanguageQuery);
        Assert.Equal(string.Empty, query.GeneratedSql);
        Assert.Null(query.Explanation);
        Assert.Null(query.Optimization);
        Assert.NotNull(query.ValidationResult);
        Assert.True(query.IsReadOnly);
        Assert.False(query.RequiresApproval);
        Assert.Equal(0, query.Confidence);
        Assert.Empty(query.TouchedTables);
        Assert.Null(query.Parameters);
    }

    [Fact]
    public void XFAiSqlQuery_WithValues_AssignsCorrectly()
    {
        var validation = new XFAiSqlValidationResult { IsValid = false, HasInjectionRisk = true };
        var query = new XFAiSqlQuery
        {
            NaturalLanguageQuery = "Show me all orders",
            GeneratedSql = "SELECT * FROM orders",
            Explanation = "Selects all columns from orders table",
            Optimization = "Add index on created_at",
            ValidationResult = validation,
            IsReadOnly = true,
            RequiresApproval = false,
            Confidence = 0.85,
            TouchedTables = new[] { "orders" },
            Parameters = new Dictionary<string, string> { { "@limit", "100" } }
        };
        Assert.Equal("Show me all orders", query.NaturalLanguageQuery);
        Assert.Equal("SELECT * FROM orders", query.GeneratedSql);
        Assert.Equal("Add index on created_at", query.Optimization);
        Assert.False(query.ValidationResult.IsValid);
        Assert.True(query.ValidationResult.HasInjectionRisk);
        Assert.Equal(0.85, query.Confidence);
        Assert.Single(query.TouchedTables);
        Assert.Equal("orders", query.TouchedTables[0]);
        Assert.Single(query.Parameters);
        Assert.Equal("100", query.Parameters["@limit"]);
    }

    [Fact]
    public void XFAiSqlQuery_Id_IsUnique()
    {
        var q1 = new XFAiSqlQuery();
        var q2 = new XFAiSqlQuery();
        Assert.NotEqual(q1.Id, q2.Id);
    }

    [Fact]
    public void XFAiSqlQuery_TouchedTables_CanBeReplaced()
    {
        var query = new XFAiSqlQuery
        {
            TouchedTables = new[] { "users", "orders", "products" }
        };
        Assert.Equal(3, query.TouchedTables.Count);
    }

    #endregion

    #region XFAiSqlValidationResult

    [Fact]
    public void XFAiSqlValidationResult_Defaults_AreCorrect()
    {
        var result = new XFAiSqlValidationResult();
        Assert.True(result.IsValid);
        Assert.True(result.IsReadOnlyQuery);
        Assert.False(result.HasInjectionRisk);
        Assert.False(result.HasBlockedOperation);
        Assert.Null(result.ErrorMessage);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void XFAiSqlValidationResult_WithErrors_HasCorrectState()
    {
        var result = new XFAiSqlValidationResult
        {
            IsValid = false,
            IsReadOnlyQuery = false,
            HasBlockedOperation = true,
            ErrorMessage = "DELETE is blocked"
        };
        Assert.False(result.IsValid);
        Assert.False(result.IsReadOnlyQuery);
        Assert.True(result.HasBlockedOperation);
        Assert.Equal("DELETE is blocked", result.ErrorMessage);
    }

    [Fact]
    public void XFAiSqlValidationResult_WithInjectionRisk_HasWarnings()
    {
        var result = new XFAiSqlValidationResult
        {
            IsValid = false,
            HasInjectionRisk = true,
            ErrorMessage = "Injection risk detected",
            Warnings = new[] { "Pattern: ; detected" }
        };
        Assert.True(result.HasInjectionRisk);
        Assert.Single(result.Warnings);
        Assert.Equal("Pattern: ; detected", result.Warnings[0]);
    }

    [Fact]
    public void XFAiSqlValidationResult_AllValid_IsCorrect()
    {
        var result = new XFAiSqlValidationResult
        {
            IsValid = true,
            IsReadOnlyQuery = true,
            HasInjectionRisk = false,
            HasBlockedOperation = false
        };
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    #endregion
}
