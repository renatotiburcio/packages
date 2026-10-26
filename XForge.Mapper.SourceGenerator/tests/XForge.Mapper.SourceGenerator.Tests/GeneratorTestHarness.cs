using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace XForge.Mapper.SourceGenerator.Tests;

/// <summary>
/// Harness Roslyn: roda o generator sobre um codigo de amostra, emite os assemblies
/// (compilacao + codigo gerado) e carrega o assembly resultante para testes de comportamento real.
/// </summary>
internal static class GeneratorTestHarness
{
    public static RunResult Run(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        // referencias da plataforma via TPA (cobre List&lt;&gt;, Task, etc.)
        var tpa = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);
        var references = tpa
            .Where(path => path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            .Where(path => !path.Contains("Native", StringComparison.OrdinalIgnoreCase))
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToList();

        // assemblies do proprio workspace (o atributo [XForgeMap] mora no pacote lib)
        references.Add(MetadataReference.CreateFromFile(
            typeof(XForge.Mapper.Generators.XForgeMapAttribute).Assembly.Location));

        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithUsings(new[]
            {
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "System.Threading.Tasks",
            })
            .WithNullableContextOptions(NullableContextOptions.Enable);

        var compilation = CSharpCompilation.Create(
            "XForgeGeneratedAssembly_" + Guid.NewGuid().ToString("N"), // nome unico: evita colisao no AssemblyLoadContext
            new[] { syntaxTree },
            references.Distinct(),
            compilationOptions);

        var generator = new XForge.Mapper.Generators.MapperGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation, out var generatedCompilation, out _);

        var outputCompilation = (CSharpCompilation)generatedCompilation;

        var diagnostics = outputCompilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToList();

        var generatedFiles = driver.GetRunResult().Results
            .SelectMany(result => result.GeneratedSources)
            .ToDictionary(source => source.HintName, source => source.SourceText.ToString());

        var assembly = Emit(outputCompilation);

        return new RunResult(
            assembly,
            generatedFiles,
            diagnostics.Select(diagnostic => diagnostic.ToString()).ToList(),
            driver.GetRunResult().Diagnostics);
    }

    private static Assembly? Emit(CSharpCompilation compilation)
    {
        using var stream = new MemoryStream();

        var emitResult = compilation.Emit(stream);

        if (!emitResult.Success)
        {
            return null;
        }

        stream.Position = 0;
        return AssemblyLoadContext.Default.LoadFromStream(stream);
    }

    public sealed class RunResult
    {
        public RunResult(Assembly? assembly, Dictionary<string, string> generatedFiles,
            List<string> compileErrors, ImmutableArray<Diagnostic> generatorDiagnostics)
        {
            Assembly = assembly;
            GeneratedFiles = generatedFiles;
            CompileErrors = compileErrors;
            GeneratorDiagnostics = generatorDiagnostics;
        }

        public Assembly? Assembly { get; }
        public Dictionary<string, string> GeneratedFiles { get; }
        public List<string> CompileErrors { get; }
        public ImmutableArray<Diagnostic> GeneratorDiagnostics { get; }
    }
}
