# T021 - Docs Compiler (Roslyn generator)

## Status: SCAFFOLDED (deferred to follow-up sprint)

## What was created
- `IXFDocsCompiler` interface (testable, no Roslyn)
- `XFDocsCompiler` implementation (in-memory analysis)
- `XFFComponentSymbol` record (component metadata)
- `XFFDocsCompileResult` record (compile result)
- Empty `XFDocsCompilerGenerator` placeholder (ISourceGenerator)

## What is deferred
- Full `ISourceGenerator` integration with Roslyn
- SyntaxTree traversal
- Auto-emit `*Docs.razor` files at build time
- bUnit auto-test generation

## Why deferred
- Requires 40h of focused generator work
- Better to ship T001, T002, T010 first
- Testable in-memory version is sufficient for the unit tests

## Reference
- T021 in antd-blazor-comparison BACKLOG
- Pattern inspired by MudBlazor's docs generator