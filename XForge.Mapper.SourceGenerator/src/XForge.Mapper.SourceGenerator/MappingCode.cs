using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XForge.Mapper.Generators;

/// <summary>
/// Emissão recursiva de código: convenções do motor de runtime traduzidas em C# gerado.
/// Mesmo tipo → acesso direto; enum→string → ToString(); enum→enum → cast;
/// classe→classe → object initializer com null-guard (recursivo);
/// coleção → função local pré-dimensionada (ICollection.Count) com skip de nulos.
/// Membros sem contraparte (XFG003) ou de tipos incompatíveis (XFG002) são registrados
/// e ignorados — paridade com o motor de runtime.
/// </summary>
internal sealed partial class MappingPlanner
{
    private const int MaxDepth = 8;

    /// <summary>Estado da geração atual: caminho de pares (detecção de ciclos) + problemas acumulados.</summary>
    internal sealed class GenerationContext
    {
        public HashSet<(ITypeSymbol Source, ITypeSymbol Destination)> Path { get; } = new();
        public List<LocalFunctionStatementSyntax> Helpers { get; } = new();
        public List<MemberIssue> Issues { get; } = new();
    }

    /// <summary>Emite as atribuições <c>destination.Membro = ...</c> para todos os membros suportados.</summary>
    public List<StatementSyntax> EmitMemberAssignments(
        string accessorRoot, string destinationAccessor, GenerationContext context)
    {
        var statements = new List<StatementSyntax>();

        // membros já vinculados pelo construtor (records/positional) ficam init-only — não reatribuir
        var constructorBound = new HashSet<string>(StringComparer.Ordinal);

        var constructorArguments = TryBindConstructor(accessorRoot);

        if (constructorArguments is not null)
        {
            foreach (var argument in constructorArguments)
            {
                constructorBound.Add(argument.ToString().Split('.').Last());
            }
        }

        foreach (var destinationMember in DestinationMembers())
        {
            if (constructorBound.Contains(destinationMember.Name))
            {
                continue;
            }

            var sourceMember = FindSourceMember(destinationMember.Name);

            if (sourceMember is null)
            {
                context.Issues.Add(new MemberIssue(
                    destinationMember.Name,
                    $"'{destinationMember.Name}' não encontrado em '{Source.ToDisplayString()}'",
                    "XFG003"));
                continue;
            }

            var memberSourceType = MemberType(sourceMember);
            var memberDestinationType = MemberType(destinationMember);

            if (!IsSupportedPair(memberSourceType, memberDestinationType, out var reason))
            {
                context.Issues.Add(new MemberIssue(
                    destinationMember.Name, reason ?? "tipos incompatíveis", "XFG002"));
                continue;
            }

            var value = EmitValue(
                accessorRoot, sourceMember, memberSourceType, memberDestinationType, context, depth: 0);

            if (value is null)
            {
                continue;
            }

            var assignment = SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(destinationAccessor + "." + destinationMember.Name),
                value);

            statements.Add(SyntaxFactory.ExpressionStatement(assignment));
        }

        return statements;
    }

    /// <summary>Expressão de valor de um membro (acesso/conversão/aninhado/coleção).</summary>
    private ExpressionSyntax? EmitValue(
        string accessorRoot, ISymbol sourceMember, ITypeSymbol sourceType, ITypeSymbol destinationType,
        GenerationContext context, int depth)
    {
        if (depth > MaxDepth)
        {
            return null;
        }

        var access = MemberAccess(accessorRoot, sourceMember.Name);

        if (SymbolEqualityComparer.Default.Equals(sourceType, destinationType))
        {
            return access;
        }

        if (sourceType.TypeKind == TypeKind.Enum)
        {
            return destinationType.SpecialType == SpecialType.System_String
                ? SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            access,
                            SyntaxFactory.IdentifierName("ToString")))
                    .WithArgumentList(SyntaxFactory.ArgumentList())
                : SyntaxFactory.CastExpression(Parse(destinationType), access);
        }

        if (TryGetElementType(sourceType) is { } sourceElement &&
            TryGetDestinationElement(destinationType) is { } destinationElement)
        {
            return EmitCollection(access, sourceMember.Name, sourceElement, destinationElement, context, depth);
        }

        if (IsMappableClass(sourceType) && IsMappableClass(destinationType))
        {
            return EmitNested(access, sourceType, destinationType, context, depth + 1);
        }

        return null;
    }

    /// <summary>
    /// <c>access == null ? null : new Dest { ... }</c> — recursivo.
    /// Ciclos (par já no caminho) são detectados e o membro é ignorado (XFG005):
    /// o motor de runtime resolve ciclos com cache de instâncias; o generator
    /// não tem estado de execução — o membro fica default com aviso.
    /// </summary>
    private ExpressionSyntax? EmitNested(
        ExpressionSyntax access, ITypeSymbol sourceType, ITypeSymbol destinationType,
        GenerationContext context, int depth)
    {
        var pair = (sourceType, destinationType);

        if (!context.Path.Add(pair))
        {
            context.Issues.Add(new MemberIssue(
                destinationType.Name, $"ciclo detectado em '{sourceType.Name}' → '{destinationType.Name}'", "XFG005"));
            return null;
        }

        try
        {
            var nested = new MappingPlanner(sourceType, destinationType);
            var bindings = new List<ExpressionSyntax>();
            var accessorRoot = access.ToString();

            foreach (var destinationMember in nested.DestinationMembers())
            {
                var sourceMember = nested.FindSourceMember(destinationMember.Name);

                if (sourceMember is null)
                {
                    continue;
                }

                var memberSourceType = MemberType(sourceMember);
                var memberDestinationType = MemberType(destinationMember);

                if (!IsSupportedPair(memberSourceType, memberDestinationType, out _))
                {
                    continue;
                }

                var value = nested.EmitValue(
                    accessorRoot, sourceMember, memberSourceType, memberDestinationType, context, depth);

                if (value is not null)
                {
                    bindings.Add(SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName(destinationMember.Name),
                        value));
                }
            }

            var creation = ObjectCreation(destinationType, bindings);

            return SyntaxFactory.ConditionalExpression(
                SyntaxFactory.BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    access,
                    SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression),
                creation);
        }
        finally
        {
            context.Path.Remove(pair);
        }
    }

    /// <summary>
    /// Função local que materializa a coleção com pré-dimensionamento:
    /// <code>
    /// List&lt;Dest&gt; XForgeMap_Itens_0(IEnumerable&lt;Src&gt; source)
    /// {
    ///     if (source is null) return null;
    ///     var list = new List&lt;Dest&gt;(source is ICollection&lt;Src&gt; c ? c.Count : 0);
    ///     foreach (var item in source)
    ///     {
    ///         if (item is null) continue;
    ///         list.Add(new Dest { A = item.A });
    ///     }
    ///     return list;
    /// }
    /// </code>
    /// </summary>
    private ExpressionSyntax EmitCollection(
        ExpressionSyntax sourceAccess, string nameBase,
        ITypeSymbol sourceElement, ITypeSymbol destinationElement,
        GenerationContext context, int depth)
    {
        var nested = new MappingPlanner(sourceElement, destinationElement);
        var bindings = new List<ExpressionSyntax>();
        var constructorArguments = nested.TryBindConstructor("item");

        if (constructorArguments is null || constructorArguments.Count == 0)
        {
            foreach (var destinationMember in nested.DestinationMembers())
            {
                var memberSource = nested.FindSourceMember(destinationMember.Name);

                if (memberSource is null)
                {
                    continue;
                }

                var memberSourceType = MemberType(memberSource);
                var memberDestinationType = MemberType(destinationMember);

                if (!IsSupportedPair(memberSourceType, memberDestinationType, out _))
                {
                    continue;
                }

                var value = nested.EmitValue(
                    "item", memberSource, memberSourceType, memberDestinationType, context, depth + 1);

                if (value is not null)
                {
                    bindings.Add(SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName(destinationMember.Name),
                        value));
                }
            }
        }

        // construção do elemento: ctor por nome (records) ou object initializer
        ExpressionSyntax elementCreation = constructorArguments is not null && constructorArguments.Count > 0
            ? SyntaxFactory.ObjectCreationExpression(ParseForCreation(destinationElement))
                .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                    constructorArguments.Select(static expression => SyntaxFactory.Argument(expression)))))
            : SyntaxFactory.ObjectCreationExpression(ParseForCreation(destinationElement))
                .WithArgumentList(SyntaxFactory.ArgumentList());

        if (bindings.Count > 0)
        {
            var withInitializer = ((ObjectCreationExpressionSyntax)elementCreation).WithInitializer(
                SyntaxFactory.InitializerExpression(
                    SyntaxKind.ObjectInitializerExpression,
                    SyntaxFactory.SeparatedList(bindings)));
            elementCreation = withInitializer;
        }

        // count: source is ICollection<Src> c ? c.Count : 0
        var collectionType = ParseForCreation(
            $"System.Collections.Generic.ICollection<{CreationName(sourceElement)}>");
        var countExpression = SyntaxFactory.ConditionalExpression(
            SyntaxFactory.IsPatternExpression(
                SyntaxFactory.IdentifierName("source"),
                SyntaxFactory.DeclarationPattern(collectionType, SyntaxFactory.SingleVariableDesignation(
                    SyntaxFactory.Identifier("sourceCollection")))),
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("sourceCollection"),
                SyntaxFactory.IdentifierName("Count")),
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)));

        // if (source is null) return null;
        var nullGuard = SyntaxFactory.IfStatement(
            SyntaxFactory.BinaryExpression(
                SyntaxKind.EqualsExpression,
                SyntaxFactory.IdentifierName("source"),
                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
            SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

        // var list = new List<Dest>(count);
        var listType = ParseForCreation(
            $"System.Collections.Generic.List<{CreationName(destinationElement)}>");
        var listDeclaration = SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(listType)
                .WithVariables(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("list"))
                        .WithInitializer(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.ObjectCreationExpression(listType)
                                .WithArgumentList(SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(countExpression)))))))));

        // foreach (var item in source) { if (item is null) continue; list.Add(new Dest { ... }); }
        var skipNull = SyntaxFactory.IfStatement(
            SyntaxFactory.IsPatternExpression(
                SyntaxFactory.IdentifierName("item"),
                SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))),
            SyntaxFactory.ContinueStatement());

        var addToList = SyntaxFactory.ExpressionStatement(
            SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("list"),
                    SyntaxFactory.IdentifierName("Add")))
                .WithArgumentList(SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(elementCreation)))));

        var foreachBody = SyntaxFactory.Block(skipNull, addToList);

        var foreachStatement = (ForEachStatementSyntax)SyntaxFactory.ParseStatement(
            "foreach (var item in source)\n{\n}");
        foreachStatement = foreachStatement.WithStatement(foreachBody);

        var returnList = SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("list"));

        var functionName = "XForgeMap_" + nameBase + "_" + context.Helpers.Count;
        var function = SyntaxFactory.LocalFunctionStatement(listType, SyntaxFactory.Identifier(functionName))
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("source"))
                    .WithType(ParseForCreation(
                        $"System.Collections.Generic.IEnumerable<{CreationName(sourceElement)}>")))))
            .WithBody(SyntaxFactory.Block(nullGuard, listDeclaration, foreachStatement, returnList));

        context.Helpers.Add(function);

        return SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(functionName))
            .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.Argument(sourceAccess))));
    }

    // ------------------------------------------------------------------
    // Construtor e utilitários
    // ------------------------------------------------------------------

    /// <summary>
    /// Emite a função local de materialização do par de ELEMENTO deste plano
    /// (usado pelo CreateList) e devolve a invocação <c>XForgeMap_List_0(source)</c>.
    /// </summary>
    public ExpressionSyntax EmitSelfCollection(
        ExpressionSyntax sourceAccess, GenerationContext context)
    {
        return EmitCollection(sourceAccess, "List", Source, Destination, context, depth: 0);
    }

    /// <summary>Argumentos do construtor vinculáveis por nome (records/imutáveis) ou null.</summary>
    public List<ExpressionSyntax>? TryBindConstructor(string accessorRoot)
    {
        var constructors = Destination.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(static method => method.MethodKind == MethodKind.Constructor &&
                                    method.DeclaredAccessibility == Accessibility.Public)
            .OrderByDescending(static method => method.Parameters.Length)
            .ToList();

        foreach (var constructor in constructors)
        {
            if (constructor.Parameters.Length == 0)
            {
                continue;
            }

            var arguments = new List<ExpressionSyntax>();
            var bound = true;

            foreach (var parameter in constructor.Parameters)
            {
                var sourceMember = FindSourceMember(parameter.Name);

                if (sourceMember is not null &&
                    IsSupportedPair(MemberType(sourceMember), parameter.Type, out _))
                {
                    arguments.Add(MemberAccess(accessorRoot, sourceMember.Name));
                    continue;
                }

                bound = false;
                break;
            }

            if (bound)
            {
                return arguments;
            }
        }

        return null;
    }

    public bool DestinationTypeIsConstructible()
    {
        if (Destination is INamedTypeSymbol { IsAbstract: true })
        {
            return false;
        }

        return Destination.GetMembers()
            .OfType<IMethodSymbol>()
            .Any(static method => method.MethodKind == MethodKind.Constructor &&
                                  method.DeclaredAccessibility == Accessibility.Public);
    }

    private static ObjectCreationExpressionSyntax ObjectCreation(
        ITypeSymbol type, List<ExpressionSyntax> bindings)
    {
        var creation = SyntaxFactory.ObjectCreationExpression(ParseForCreation(type))
            .WithArgumentList(SyntaxFactory.ArgumentList());

        if (bindings.Count > 0)
        {
            creation = creation.WithInitializer(SyntaxFactory.InitializerExpression(
                SyntaxKind.ObjectInitializerExpression,
                SyntaxFactory.SeparatedList(bindings)));
        }

        return creation;
    }

    private static ExpressionSyntax MemberAccess(string root, string member) =>
        SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName(root),
            SyntaxFactory.IdentifierName(member));

    /// <summary>Nome de tipo para DECLARACOES (mantem <c>?</c> de nullable).</summary>
    private static TypeSyntax Parse(ITypeSymbol type) => Parse(NameOf(type));

    private static TypeSyntax Parse(string typeName) => SyntaxFactory.ParseTypeName(typeName);

    /// <summary>Nome de tipo para CRIAÇÃO de instancias — sem a anotacao <c>?</c> (new T?() e invalido).</summary>
    private static TypeSyntax ParseForCreation(ITypeSymbol type) =>
        ParseForCreation(NameOf(type));

    private static TypeSyntax ParseForCreation(string typeName) =>
        SyntaxFactory.ParseTypeName(typeName.TrimEnd('?'));

    private static string NameOf(ITypeSymbol type) =>
        type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

    /// <summary>Nome de tipo sem anotacao nullable (para criacao/padroes).</summary>
    private static string CreationName(ITypeSymbol type) => NameOf(type).TrimEnd('?');
}
