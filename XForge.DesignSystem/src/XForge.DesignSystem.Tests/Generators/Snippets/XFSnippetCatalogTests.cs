// T046: Tests for XFSnippetCatalog + XFDefaultSnippetsGenerator.
using Xunit;
using XForge.DesignSystem.Generators.Snippets;

namespace XForge.DesignSystem.Tests.Generators.Snippets;

public class XFSnippetCatalogTests
{
    [Fact] public void All_HasSnippets() => Assert.NotEmpty(XFSnippetCatalog.All);

    [Fact] public void ForComponent_ReturnsMatches()
    {
        var xs = XFSnippetCatalog.ForComponent("XButton").ToArray();
        Assert.Equal(2, xs.Length);
        Assert.All(xs, s => Assert.Equal("XButton", s.Component));
    }

    [Fact] public void ToMarkdown_HasFencedBlock()
    {
        var md = XFSnippetCatalog.All[0].ToMarkdown();
        Assert.Contains("```razor", md);
        Assert.Contains("<XButton>", md);
    }
}

public class XFDefaultSnippetsGeneratorTests
{
    [Fact] public void Count_MatchesCatalog() => Assert.Equal(XFSnippetCatalog.All.Count, new XFDefaultSnippetsGenerator().Count);

    [Fact] public void Generate_HasNamespace() => Assert.Contains("namespace XForge.DesignSystem.Generated", new XFDefaultSnippetsGenerator().Generate());

    [Fact] public void Generate_HasStaticClass() => Assert.Contains("public static class Snippets", new XFDefaultSnippetsGenerator().Generate());
}