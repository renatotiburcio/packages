using XForge.AssetsGenerator;
using Xunit;

namespace XForge.AssetsGenerator.Tests;

public class SvgOptimizerTests
{
    [Fact]
    public void Optimize_Removes_Comments()
    {
        var input = "<svg><!-- comment --><path/></svg>";
        Assert.DoesNotContain("<!--", SvgOptimizer.Optimize(input));
    }

    [Fact]
    public void Optimize_Reduces_Whitespace()
    {
        var input = "<svg>  <path/>  </svg>";
        var result = SvgOptimizer.Optimize(input);
        Assert.DoesNotContain(">  <", result);
    }

    [Fact]
    public void Optimize_ReducesPrecision()
    {
        var input = "<path d=\"M1.123456789L2\"/>";
        var result = SvgOptimizer.Optimize(input);
        Assert.Contains("1.1234", result);
    }

    [Fact]
    public void HasExternalLicenseTraces_Finds_FontAwesome()
    {
        Assert.True(SvgOptimizer.HasExternalLicenseTraces("<!-- Font Awesome -->"));
    }

    [Fact]
    public void HasExternalLicenseTraces_CleanSvg_ReturnsFalse()
    {
        Assert.False(SvgOptimizer.HasExternalLicenseTraces("<svg><path/></svg>"));
    }
}