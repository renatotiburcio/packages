using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace XForge.Mediator.SourceGenerator.Tests;

/// <summary>
/// Harness Roslyn: roda o generator sobre um codigo de amostra, emite o assembly
/// (compilacao + codigo gerado) e carrega o resultado para testes de comportamento real.
/// </summary>
internal static class GeneratorTestHarness
{
    public static RunResult Run(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = new List<MetadataReference>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void AddPath(string path)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (seen.Add(name))
            {
                references.Add(MetadataReference.CreateFromFile(path));
            }
        }

        // assemblies do workspace primeiro (mediator + DI) — precedência sobre facades
        AddPath(typeof(global::XForge.Mediator.ISender).Assembly.Location);
        AddPath(typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection).Assembly.Location);
        AddPath(typeof(Microsoft.Extensions.DependencyInjection.ServiceProvider).Assembly.Location);

        // TPA sem facades duplicadas (netstandard) para evitar identidade de tipos duplicada
        var tpa = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);

        foreach (var path in tpa)
        {
            if (!path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("Native", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("netstandard", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            AddPath(path);
        }

        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithNullableContextOptions(NullableContextOptions.Enable);

        var compilation = CSharpCompilation.Create(
            "XForgeGeneratedMediator_" + Guid.NewGuid().ToString("N"),
            new[] { syntaxTree },
            references,
            compilationOptions);

        var generator = new XForge.Mediator.Generators.MediatorGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation, out var generatedCompilation, out _);

        var outputCompilation = (CSharpCompilation)generatedCompilation;

        var compileErrors = outputCompilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .Select(diagnostic => diagnostic.ToString())
            .ToList();

        var generatedFiles = driver.GetRunResult().Results
            .SelectMany(result => result.GeneratedSources)
            .ToDictionary(source => source.HintName, source => source.SourceText.ToString());

        var assembly = Emit(outputCompilation);

        return new RunResult(assembly, generatedFiles, compileErrors, driver.GetRunResult().Diagnostics);
    }

    private static Assembly? Emit(CSharpCompilation compilation)
    {
        using var stream = new MemoryStream();

        if (!compilation.Emit(stream).Success)
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
