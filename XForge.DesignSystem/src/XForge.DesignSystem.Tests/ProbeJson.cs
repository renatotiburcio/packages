using System.Text.Json;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class ProbeJson
{
    [Fact]
    public void Parse()
    {
        var path = System.IO.Path.Combine(AppContext.BaseDirectory, "pt-BR.json");
        var txt = System.IO.File.ReadAllText(path);
        using var doc = JsonDocument.Parse(txt);
        Assert.True(doc.RootElement.EnumerateObject().Count() > 300);
    }
}
