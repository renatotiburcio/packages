using Xunit;
using XForge.DesignSystem.Components.AI.DataGrid;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for AI DataGrid types at 0% coverage: XFAiGridQuery, XFAiGridFilter, XFAiGridSort,
/// XFAiGridInsight, XFAiGridInsightType.
/// </summary>
public sealed class XFAiGridTypesTest
{
    #region XFAiGridInsightType

    [Fact]
    public void XFAiGridInsightType_HasAllValues()
    {
        Assert.Equal(6, Enum.GetValues<XFAiGridInsightType>().Length);
        Assert.True(Enum.IsDefined<XFAiGridInsightType>(XFAiGridInsightType.Pattern));
        Assert.True(Enum.IsDefined<XFAiGridInsightType>(XFAiGridInsightType.Anomaly));
        Assert.True(Enum.IsDefined<XFAiGridInsightType>(XFAiGridInsightType.Kpi));
        Assert.True(Enum.IsDefined<XFAiGridInsightType>(XFAiGridInsightType.Trend));
        Assert.True(Enum.IsDefined<XFAiGridInsightType>(XFAiGridInsightType.Summary));
        Assert.True(Enum.IsDefined<XFAiGridInsightType>(XFAiGridInsightType.Recommendation));
    }

    #endregion

    #region XFAiGridQuery

    [Fact]
    public void XFAiGridQuery_Defaults_AreCorrect()
    {
        var query = new XFAiGridQuery();
        Assert.Equal(string.Empty, query.NaturalLanguageQuery);
        Assert.Null(query.GeneratedSql);
        Assert.Empty(query.Filters);
        Assert.Empty(query.Sorts);
        Assert.Null(query.GroupBy);
        Assert.Null(query.Summary);
        Assert.Equal(0, query.Confidence);
    }

    [Fact]
    public void XFAiGridQuery_WithValues_AssignsCorrectly()
    {
        var filters = new List<XFAiGridFilter>
        {
            new() { Column = "amount", Operator = "gt", Value = "100" }
        };
        var sorts = new List<XFAiGridSort>
        {
            new() { Column = "date", Descending = true }
        };
        var query = new XFAiGridQuery
        {
            NaturalLanguageQuery = "Show large orders",
            GeneratedSql = "SELECT * FROM orders WHERE amount > 100",
            Filters = filters,
            Sorts = sorts,
            GroupBy = "customer",
            Summary = "Orders over 100 sorted by date",
            Confidence = 0.92
        };
        Assert.Equal("Show large orders", query.NaturalLanguageQuery);
        Assert.Single(query.Filters);
        Assert.Equal("amount", query.Filters[0].Column);
        Assert.Single(query.Sorts);
        Assert.True(query.Sorts[0].Descending);
        Assert.Equal("customer", query.GroupBy);
        Assert.Equal(0.92, query.Confidence);
    }

    #endregion

    #region XFAiGridFilter

    [Fact]
    public void XFAiGridFilter_Defaults_AreCorrect()
    {
        var filter = new XFAiGridFilter();
        Assert.Equal(string.Empty, filter.Column);
        Assert.Equal(string.Empty, filter.Operator);
        Assert.Null(filter.Value);
    }

    [Fact]
    public void XFAiGridFilter_WithValues_AssignsCorrectly()
    {
        var filter = new XFAiGridFilter
        {
            Column = "status",
            Operator = "equals",
            Value = "active"
        };
        Assert.Equal("status", filter.Column);
        Assert.Equal("equals", filter.Operator);
        Assert.Equal("active", filter.Value);
    }

    #endregion

    #region XFAiGridSort

    [Fact]
    public void XFAiGridSort_Defaults_AreCorrect()
    {
        var sort = new XFAiGridSort();
        Assert.Equal(string.Empty, sort.Column);
        Assert.False(sort.Descending);
    }

    [Fact]
    public void XFAiGridSort_DescendingTrue_AssignsCorrectly()
    {
        var sort = new XFAiGridSort { Column = "priority", Descending = true };
        Assert.Equal("priority", sort.Column);
        Assert.True(sort.Descending);
    }

    #endregion

    #region XFAiGridInsight

    [Fact]
    public void XFAiGridInsight_Defaults_AreCorrect()
    {
        var insight = new XFAiGridInsight();
        Assert.NotEmpty(insight.Id);
        Assert.Equal(8, insight.Id.Length);
        Assert.Equal(string.Empty, insight.Title);
        Assert.Equal(string.Empty, insight.Description);
        Assert.Equal(XFAiGridInsightType.Pattern, insight.Type); // default enum = 0
        Assert.Null(insight.Confidence);
        Assert.Null(insight.ActionLabel);
        Assert.Null(insight.Metadata);
    }

    [Fact]
    public void XFAiGridInsight_WithValues_AssignsCorrectly()
    {
        var insight = new XFAiGridInsight
        {
            Title = "Revenue spike",
            Description = "Revenue increased 45% in Q4",
            Type = XFAiGridInsightType.Anomaly,
            Confidence = 0.88,
            ActionLabel = "Investigate",
            Metadata = new Dictionary<string, string> { { "quarter", "Q4" } }
        };
        Assert.Equal("Revenue spike", insight.Title);
        Assert.Equal(XFAiGridInsightType.Anomaly, insight.Type);
        Assert.Equal(0.88, insight.Confidence);
        Assert.Equal("Investigate", insight.ActionLabel);
        Assert.Equal("Q4", insight.Metadata!["quarter"]);
    }

    [Fact]
    public void XFAiGridInsight_Id_IsUnique()
    {
        var a = new XFAiGridInsight();
        var b = new XFAiGridInsight();
        Assert.NotEqual(a.Id, b.Id);
    }

    #endregion
}
