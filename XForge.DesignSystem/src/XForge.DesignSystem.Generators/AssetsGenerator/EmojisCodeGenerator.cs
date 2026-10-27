using System.Text;

namespace XForge.AssetsGenerator;

/// <summary>
/// Generates one .cs file per (Group, Style) combination for emojis.
/// Aligns with FluentUI.AssetsGenerator.EmojisCodeGenerator.
/// </summary>
public static class EmojisCodeGenerator
{
    public static int Generate(GeneratorSettings settings)
    {
        if (!System.IO.Directory.Exists(settings.Assets))
        {
            Console.Error.WriteLine($"ERROR: assets path not found: {settings.Assets}");
            return 1;
        }

        System.IO.Directory.CreateDirectory(settings.Target!);
        var svgFiles = System.IO.Directory.EnumerateFiles(settings.Assets, "*.svg", System.IO.SearchOption.AllDirectories);
        var grouped = svgFiles.Select(ParseEmoji).Where(e => e != null).Cast<Emoji>()
            .GroupBy(e => $"{e.Group}_{e.Style}");

        var count = 0;
        foreach (var group in grouped)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {settings.Namespace};");
            sb.AppendLine();
            sb.AppendLine($"// Auto-generated emoji group - {group.Count()} entries");
            foreach (var emoji in group)
            {
                var svg = System.IO.File.ReadAllText(emoji.SvgPath);
                sb.AppendLine($"public static class {emoji.Name}_{emoji.Skintone} {{ public const string Svg = @\"{svg}\"; }}");
                count++;
            }
            var path = System.IO.Path.Combine(settings.Target!, $"{group.Key}.cs");
            System.IO.File.WriteAllText(path, sb.ToString());
        }
        Console.WriteLine($"Generated {count} emojis in {grouped.Count()} files");
        return 0;
    }

    private static Emoji? ParseEmoji(string svgPath)
    {
        var file = System.IO.Path.GetFileNameWithoutExtension(svgPath);
        var parts = file.Split('_');
        if (parts.Length < 3) return null;
        return new Emoji
        {
            Name = parts[0],
            Group = parts[1],
            Style = parts[2],
            Skintone = parts.Length > 3 ? parts[3] : "Default",
            SvgPath = svgPath,
        };
    }
}