using Xunit;
using XForge.DesignSystem.Utilities;

namespace XForge.DesignSystem.Tests.Services;

/// <summary>
/// Tests for <see cref="XFSearchRanker"/> — the reusable text search ranking utility.
/// </summary>
public class XFSearchRankerTest
{
    #region Score — Exact Matches

    [Fact]
    public void Score_ExactLabelMatch_ReturnsHighestScore()
    {
        var score = XFSearchRanker.Score("buttons", "buttons", "buttons");
        Assert.Equal(100, score);
    }

    [Fact]
    public void Score_ExactSlugMatch_Returns95()
    {
        var score = XFSearchRanker.Score("text input", "text-input", "text-input");
        Assert.Equal(95, score);
    }

    #endregion

    #region Score — Prefix Matches

    [Fact]
    public void Score_LabelStartsWithQuery_Returns80()
    {
        var score = XFSearchRanker.Score("buttons", "buttons", "but");
        Assert.Equal(80, score);
    }

    [Fact]
    public void Score_SlugStartsWithQuery_Returns75()
    {
        var score = XFSearchRanker.Score("my component", "my-component", "my-comp");
        Assert.Equal(75, score);
    }

    #endregion

    #region Score — Contains Matches

    [Fact]
    public void Score_LabelContainsQueryAsWholeWord_Returns60()
    {
        var score = XFSearchRanker.Score("split button", "split-button", "button");
        Assert.Equal(60, score);
    }

    [Fact]
    public void Score_LabelContainsHyphenatedQuery_Returns60()
    {
        var score = XFSearchRanker.Score("drop-down menu", "dropdown-menu", "down");
        Assert.Equal(60, score);
    }

    [Fact]
    public void Score_LabelContainsQuery_Returns50()
    {
        var score = XFSearchRanker.Score("autocomplete", "autocomplete", "compl");
        Assert.Equal(50, score);
    }

    [Fact]
    public void Score_SlugContainsQuery_Returns45()
    {
        var score = XFSearchRanker.Score("my component", "my-long-component-slug", "long-comp");
        Assert.Equal(45, score);
    }

    #endregion

    #region Score — Word-Prefix Match

    [Fact]
    public void Score_WordPrefixMatch_Returns40()
    {
        // "split menu" should match "split button menu" because "split" starts with "split" and "menu" starts with "menu"
        // "split menu" does NOT StartsWith match on the label, and the label contains "split" as whole word (60)
        // Actually "split button menu" contains " split" → 60; so let's use a case where contains doesn't match:
        // "button menu" on "split-button-menu" → slug contains "button-menu"? yes → 45
        // Let's use "two words" on "three-word-item" → slug "three-word-item" contains? No.
        // "two wor" on "two words" → starts with "two wor" → 80. 
        // Use non-contiguous words: "but men" on "button menu" → label "button menu" contains " but"? No (starts with but).
        // Actually "button menu" starts with "but" → 80. 
        // Let's use "input text" on "text input component" → label contains " input"? yes → 60.
        // "comp text" on "text input component" → word-prefix: "comp" matches "component"? yes. "text" matches "text"? yes. → 40
        var score = XFSearchRanker.Score("text input component", "text-input-component", "comp text");
        Assert.Equal(40, score);
    }

    [Fact]
    public void Score_WordPrefixMatch_SingleWordQuery_Returns0()
    {
        // Single-word query should not trigger word-prefix match (it would be caught by other matchers anyway)
        var score = XFSearchRanker.Score("no match here", "no-match", "zzz");
        Assert.Equal(0, score);
    }

    #endregion

    #region Score — Fuzzy Match

    [Fact]
    public void Score_FuzzyMatch_Returns30()
    {
        // "btn" matches "button" as a fuzzy subsequence: b-u-t-t-o-n contains b,t,n in order
        var score = XFSearchRanker.Score("button", "button", "btn");
        Assert.Equal(30, score);
    }

    [Fact]
    public void Score_FuzzyMatch_NoMatch_Returns0()
    {
        // "xyz" does not appear in order in "button"
        var score = XFSearchRanker.Score("button", "button", "xyz");
        Assert.Equal(0, score);
    }

    #endregion

    #region Score — No Match

    [Fact]
    public void Score_EmptyQuery_Returns0()
    {
        var score = XFSearchRanker.Score("buttons", "buttons", "");
        Assert.Equal(0, score);
    }

    [Fact]
    public void Score_NullQuery_Returns0()
    {
        var score = XFSearchRanker.Score("buttons", "buttons", null!);
        Assert.Equal(0, score);
    }

    [Fact]
    public void Score_CompletelyUnrelated_Returns0()
    {
        var score = XFSearchRanker.Score("buttons", "buttons", "zzzzzzzz");
        Assert.Equal(0, score);
    }

    #endregion

    #region Score — Case Insensitivity (inputs are pre-normalized)

    [Fact]
    public void Score_CaseInsensitive_ExactMatch()
    {
        // Caller normalizes to lowercase before calling Score
        var score = XFSearchRanker.Score("buttons", "buttons", "buttons");
        Assert.Equal(100, score);
    }

    #endregion

    #region IsFuzzyMatch

    [Fact]
    public void IsFuzzyMatch_MatchingSubsequence_ReturnsTrue()
    {
        Assert.True(XFSearchRanker.IsFuzzyMatch("button", "btn"));
    }

    [Fact]
    public void IsFuzzyMatch_NonMatchingSubsequence_ReturnsFalse()
    {
        Assert.False(XFSearchRanker.IsFuzzyMatch("button", "bnx"));
    }

    [Fact]
    public void IsFuzzyMatch_EmptyQuery_ReturnsTrue()
    {
        Assert.True(XFSearchRanker.IsFuzzyMatch("anything", ""));
    }

    [Fact]
    public void IsFuzzyMatch_QueryLongerThanText_ReturnsFalse()
    {
        Assert.False(XFSearchRanker.IsFuzzyMatch("ab", "abc"));
    }

    #endregion

    #region IsWordPrefixMatch

    [Fact]
    public void IsWordPrefixMatch_AllWordsMatch_ReturnsTrue()
    {
        Assert.True(XFSearchRanker.IsWordPrefixMatch("text input component", "text inp comp"));
    }

    [Fact]
    public void IsWordPrefixMatch_OneWordDoesNotMatch_ReturnsFalse()
    {
        Assert.False(XFSearchRanker.IsWordPrefixMatch("text input component", "text inp xyz"));
    }

    [Fact]
    public void IsWordPrefixMatch_SingleQueryWord_ReturnsFalse()
    {
        // Single-word queries should not trigger word-prefix match
        Assert.False(XFSearchRanker.IsWordPrefixMatch("text input", "text"));
    }

    [Fact]
    public void IsWordPrefixMatch_HyphenatedLabel_Matches()
    {
        Assert.True(XFSearchRanker.IsWordPrefixMatch("split-button-menu", "split but men"));
    }

    #endregion

    #region Score — Priority Ordering

    [Fact]
    public void Score_ExactBeatsPrefix()
    {
        var exactScore = XFSearchRanker.Score("tabs", "tabs", "tabs");
        var prefixScore = XFSearchRanker.Score("table", "table", "tab");
        Assert.True(exactScore > prefixScore);
    }

    [Fact]
    public void Score_PrefixBeatsContains()
    {
        var prefixScore = XFSearchRanker.Score("button", "button", "but");
        var containsScore = XFSearchRanker.Score("rebutton", "rebutton", "but");
        Assert.True(prefixScore > containsScore);
    }

    [Fact]
    public void Score_ContainsBeatsFuzzy()
    {
        var containsScore = XFSearchRanker.Score("autocomplete", "autocomplete", "compl");
        var fuzzyScore = XFSearchRanker.Score("autocomplete", "autocomplete", "acl");
        Assert.True(containsScore > fuzzyScore);
    }

    #endregion

    #region Rank (generic)

    [Fact]
    public void Rank_ReturnsRankedResults()
    {
        var entries = new[]
        {
            new { Label = "Autocomplete", Slug = "autocomplete" },
            new { Label = "Button", Slug = "button" },
            new { Label = "Split Button", Slug = "split-button" },
            new { Label = "Tabs", Slug = "tabs" },
        };

        var results = XFSearchRanker.Rank(entries, "but", e => e.Label, e => e.Slug);

        Assert.Equal(2, results.Count);
        Assert.Equal("Button", results[0].Entry.Label);
        Assert.Equal("Split Button", results[1].Entry.Label);
    }

    [Fact]
    public void Rank_EmptyQuery_ReturnsEmpty()
    {
        var entries = new[] { new { Label = "Button", Slug = "button" } };
        var results = XFSearchRanker.Rank(entries, "", e => e.Label, e => e.Slug);
        Assert.Empty(results);
    }

    [Fact]
    public void Rank_NullQuery_ReturnsEmpty()
    {
        var entries = new[] { new { Label = "Button", Slug = "button" } };
        var results = XFSearchRanker.Rank(entries, null, e => e.Label, e => e.Slug);
        Assert.Empty(results);
    }

    [Fact]
    public void Rank_RespectsMaxResults()
    {
        var entries = Enumerable.Range(1, 100)
            .Select(i => new { Label = $"Item {i}", Slug = $"item-{i}" })
            .ToArray();

        var results = XFSearchRanker.Rank(entries, "item", e => e.Label, e => e.Slug, maxResults: 5);
        Assert.Equal(5, results.Count);
    }

    #endregion
}
