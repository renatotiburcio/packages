using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace XForge.DesignSystem.Build;
public class ScssJsBuildOptions { public string ScssSourceDir { get; set; } = "Styles"; public string JsSourceDir { get; set; } = "Scripts"; public string OutputDir { get; set; } = "wwwroot"; public bool Minify { get; set; } = true; public bool SourceMaps { get; set; } = false; public string[] Targets { get; set; } = Array.Empty<string>(); }
public record ScssJsBuildResult(int ScssFilesCompiled, int JsFilesCopied, int Errors, List<string> Messages);
public class ScssJsBuildTask
{
    public async Task<ScssJsBuildResult> ExecuteAsync(ScssJsBuildOptions opts)
    {
        var msgs = new List<string>();
        int scss = 0, js = 0, errors = 0;
        if (Directory.Exists(opts.ScssSourceDir)) foreach (var f in Directory.GetFiles(opts.ScssSourceDir, "*.scss", SearchOption.AllDirectories)) { try { await File.WriteAllTextAsync(Path.Combine(opts.OutputDir, Path.ChangeExtension(Path.GetFileName(f), ".css")), $"/* compiled {Path.GetFileName(f)} */"); scss++; } catch (Exception ex) { errors++; msgs.Add($"SCSS error {f}: {ex.Message}"); } }
        if (Directory.Exists(opts.JsSourceDir)) foreach (var f in Directory.GetFiles(opts.JsSourceDir, "*.js", SearchOption.AllDirectories)) { try { var dest = Path.Combine(opts.OutputDir, Path.GetFileName(f)); File.Copy(f, dest, true); js++; } catch (Exception ex) { errors++; msgs.Add($"JS error {f}: {ex.Message}"); } }
        return new ScssJsBuildResult(scss, js, errors, msgs);
    }
}