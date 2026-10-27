// Stub: full Roslyn ISourceGenerator implementation is deferred.
// This file documents the generator entry point contract.
using Microsoft.CodeAnalysis;

namespace XForge.Generators.DocsCompiler;

[Generator]
public class XFDocsCompilerGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }
    public void Execute(GeneratorExecutionContext context)
    {
        // Deferred: would scan syntax trees for ComponentBase subclasses and emit *Docs.razor.
    }
}