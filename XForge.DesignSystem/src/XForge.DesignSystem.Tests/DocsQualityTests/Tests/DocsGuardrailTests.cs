using System.Text.Json;
using Xunit;
using XForge.DesignSystem.Docs.QualityTests.Infrastructure;

namespace XForge.DesignSystem.Docs.QualityTests.Tests;

public class DocsGuardrailTests
{
    private static readonly string SnapshotPath = Path.Combine(PathResolver.SnapshotDir, "docs-api-contract.snapshot.json");

    [Fact]
    public void Public_Api_Changes_Should_Require_Docs_Update()
    {
        var current = BuildSnapshot();
        if (!File.Exists(SnapshotPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SnapshotPath)!);
            File.WriteAllText(SnapshotPath, JsonSerializer.Serialize(current, new JsonSerializerOptions { WriteIndented = true }));
            return;
        }
        var saved = JsonSerializer.Deserialize<Dictionary<string, Snap>>(File.ReadAllText(SnapshotPath))!;
        var changes = new List<string>();
        foreach (var (n, c) in current)
        {
            if (!saved.TryGetValue(n, out var s)) { changes.Add($"Novo: {n}"); continue; }
            var np = c.Params.Except(s.Params).ToList();
            var ne = c.Events.Except(s.Events).ToList();
            if (np.Any()) changes.Add($"{n}: novos params - {string.Join(", ", np)}");
            if (ne.Any()) changes.Add($"{n}: novos eventos - {string.Join(", ", ne)}");
        }
        if (changes.Any())
            File.WriteAllText(SnapshotPath, JsonSerializer.Serialize(current, new JsonSerializerOptions { WriteIndented = true }));
        Assert.True(changes.Count == 0, $"API alterada:\n{string.Join("\n", changes)}");
    }

    private static Dictionary<string, Snap> BuildSnapshot()
    {
        var r = new Dictionary<string, Snap>();
        foreach (var c in ComponentApiCatalog.All)
            r[c.Name] = new Snap { Params = c.Parameters.Select(p => p.Name).ToList(), Events = c.EventCallbacks.Select(e => e.Name).ToList() };
        return r;
    }

    private sealed class Snap { public List<string> Params { get; set; } = new(); public List<string> Events { get; set; } = new(); }
}
