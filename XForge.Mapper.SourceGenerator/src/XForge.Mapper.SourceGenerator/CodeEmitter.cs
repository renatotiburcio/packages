using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XForge.Mapper.Generators;

/// <summary>
/// Emite os corpos dos metodos parciais como codigo C# puro - mesma semantica do motor de runtime:
/// Create: var destination = new Dest(...); atribuicoes; return destination;
/// (records vinculam construtor por nome; aninhados com null-guard; colecoes via funcao local).
/// Fill: as mesmas atribuicoes contra destination (merge: so membros mapeados).
/// CreateList: funcao local pre-dimensionada que ignora itens nulos.
/// Membros sem contraparte ou de tipos incompativeis ficam default + diagnostico (XFG002/XFG003)
/// - paridade com o motor de runtime, que os ignora silenciosamente.
/// </summary>
internal sealed class CodeEmitter
{
    private readonly List<MapMethodModel> _methods;
    private readonly MappingPlanner.GenerationContext _context = new();

    public CodeEmitter(List<MapMethodModel> methods)
    {
        _methods = methods;
    }

    public IReadOnlyList<MemberIssue> Issues => _context.Issues;

    public CompilationUnitSyntax? EmitClass(INamedTypeSymbol symbol, ClassDeclarationSyntax declaration)
    {
        var classMembers = new List<MemberDeclarationSyntax>();

        foreach (var model in _methods)
        {
            var methodDeclaration = model.Declaration;

            var statements = model.Kind switch
            {
                MapMethodKind.Create => EmitCreate(model),
                MapMethodKind.Fill => EmitFill(model),
                MapMethodKind.CreateList => EmitCreateList(model),
                _ => throw new ArgumentOutOfRangeException(nameof(model.Kind)),
            };

            // preserva os modificadores e o tipo de retorno do metodo parcial original
            var modifiers = methodDeclaration.Modifiers.Any(token =>
                    token.IsKind(SyntaxKind.PartialKeyword))
                ? methodDeclaration.Modifiers
                : methodDeclaration.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            var returnType = model.Kind == MapMethodKind.Fill
                ? methodDeclaration.ReturnType // Fill: void preservado
                : SyntaxFactory.ParseTypeName(ModelTypeName(model));

            classMembers.Add(methodDeclaration
                .WithReturnType(returnType)
                .WithBody(SyntaxFactory.Block(statements))
                .WithSemicolonToken(default)
                .WithoutTrivia()
                .WithModifiers(modifiers));
        }

        if (classMembers.Count == 0)
        {
            return null;
        }

        var classDeclaration = SyntaxFactory.ClassDeclaration(symbol.Name)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
            .WithMembers(SyntaxFactory.List(classMembers));

        var usings = SyntaxFactory.List(new[]
        {
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
        });

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : symbol.ContainingNamespace.ToDisplayString();

        var compilationUnit = SyntaxFactory.CompilationUnit().WithUsings(usings);

        return namespaceName is null
            ? compilationUnit.WithMembers(
                SyntaxFactory.SingletonList<MemberDeclarationSyntax>(classDeclaration))
            : compilationUnit.WithMembers(
                SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                    SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(namespaceName))
                        .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(classDeclaration))));
    }

    // ------------------------------------------------------------------
    // Corpos
    // ------------------------------------------------------------------

    private List<StatementSyntax> EmitCreate(MapMethodModel model)
    {
        _helpersBefore = _context.Helpers.Count;

        var planner = new MappingPlanner(model.SourceType, model.DestinationType);

        if (!planner.DestinationTypeIsConstructible())
        {
            return ThrowUnsupported(model.DestinationType);
        }

        var statements = new List<StatementSyntax>();
        var constructorArguments = planner.TryBindConstructor("source");
        var destinationTypeName = CreationName(model.DestinationType);

        ExpressionSyntax creation = constructorArguments is not null && constructorArguments.Count > 0
            ? SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(destinationTypeName))
                .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                    constructorArguments.Select(expression => SyntaxFactory.Argument(expression)))))
            : SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(destinationTypeName))
                .WithArgumentList(SyntaxFactory.ArgumentList());

        statements.Add(SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName(destinationTypeName))
                .WithVariables(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("destination"))
                        .WithInitializer(SyntaxFactory.EqualsValueClause(creation))))));

        statements.AddRange(planner.EmitMemberAssignments("source", "destination", _context));
        AppendNewHelpers(statements);
        statements.Add(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("destination")));
        return statements;

        void AppendNewHelpers(List<StatementSyntax> target)
        {
            for (var i = _helpersBefore; i < _context.Helpers.Count; i++)
            {
                target.Add(_context.Helpers[i]);
            }
        }
    }

    private List<StatementSyntax> EmitFill(MapMethodModel model)
    {
        _helpersBefore = _context.Helpers.Count;

        var planner = new MappingPlanner(model.SourceType, model.DestinationType);
        var statements = planner.EmitMemberAssignments("source", "destination", _context);

        for (var i = _helpersBefore; i < _context.Helpers.Count; i++)
        {
            statements.Add(_context.Helpers[i]);
        }

        return statements;
    }

    private List<StatementSyntax> EmitCreateList(MapMethodModel model)
    {
        _helpersBefore = _context.Helpers.Count;

        // plano do par de elemento; a funcao local e anexada ao corpo do metodo
        var elementPlanner = new MappingPlanner(model.SourceType, model.DestinationType);

        var invocation = elementPlanner.EmitSelfCollection(
            SyntaxFactory.IdentifierName("source"), _context);

        var statements = new List<StatementSyntax>
        {
            SyntaxFactory.ReturnStatement(invocation),
        };

        for (var i = _helpersBefore; i < _context.Helpers.Count; i++)
        {
            statements.Add(_context.Helpers[i]);
        }

        return statements;
    }

    private int _helpersBefore;

    // ------------------------------------------------------------------
    // Utilitarios
    // ------------------------------------------------------------------

    private static List<StatementSyntax> ThrowUnsupported(ITypeSymbol destinationType)
    {
        return new List<StatementSyntax>
        {
            SyntaxFactory.ThrowStatement(
                SyntaxFactory.ObjectCreationExpression(
                        SyntaxFactory.ParseTypeName("System.InvalidOperationException"))
                    .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal(
                                "Destino '" + destinationType.ToDisplayString() +
                                "' nao possui construtor acessivel."))))))),
        };
    }

    private static string ModelTypeName(MapMethodModel model)
    {
        return model.Kind == MapMethodKind.CreateList
            ? "System.Collections.Generic.List<" + model.DestinationType
                .ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) + ">"
            : model.DestinationType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
    }

    /// <summary>Nome do tipo para criacao de instancia (sem anotacao nullable).</summary>
    private static string CreationName(ITypeSymbol type) =>
        type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).TrimEnd('?');
}

/// <summary>Problema de membro encontrado durante a emissao (diagnostico XFG002/XFG003/XFG005).</summary>
internal sealed class MemberIssue
{
    public MemberIssue(string memberName, string reason, string diagnosticId)
    {
        MemberName = memberName;
        Reason = reason;
        DiagnosticId = diagnosticId;
    }

    public string MemberName { get; }
    public string Reason { get; }
    public string DiagnosticId { get; }
}
