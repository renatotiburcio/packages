using Microsoft.Extensions.Configuration;

namespace XForge.AssetsGenerator;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
        {
            PrintHelp();
            return 0;
        }

        var config = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

        var settings = new GeneratorSettings();
        config.Bind(settings);

        if (string.IsNullOrEmpty(settings.Assets))
        {
            Console.Error.WriteLine("ERROR: --assets path is required");
            return 1;
        }

        return settings.Library?.ToLowerInvariant() switch
        {
            "icon" => IconsCodeGenerator.Generate(settings),
            "emoji" => EmojisCodeGenerator.Generate(settings),
            _ => PrintUnknownLibrary(settings.Library),
        };
    }

    private static void PrintHelp()
    {
        Console.WriteLine("XForge Assets Generator (T080)");
        Console.WriteLine("Usage: xforge-assets-gen --assets <path> --target <path> --library icon|emoji [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --assets <path>      Source directory containing SVG files");
        Console.WriteLine("  --target <path>      Destination directory for generated .cs files");
        Console.WriteLine("  --namespace <ns>     C# namespace (default: XForge.DesignSystem.Components.Icon)");
        Console.WriteLine("  --sizes <list>       Comma-separated size filter (e.g. 16,20,24)");
        Console.WriteLine("  --names <list>       Comma-separated icon name filter");
        Console.WriteLine("  --library <name>     icon | emoji");
    }

    private static int PrintUnknownLibrary(string? library)
    {
        Console.Error.WriteLine($"ERROR: unknown library '{library}'. Use icon or emoji.");
        return 1;
    }
}