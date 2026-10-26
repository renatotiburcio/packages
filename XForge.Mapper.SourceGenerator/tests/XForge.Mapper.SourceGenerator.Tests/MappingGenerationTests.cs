using System.Reflection;
using Xunit;

namespace XForge.Mapper.SourceGenerator.Tests;

public class MappingGenerationTests
{
    [Fact]
    public void Create_and_fill_and_list_methods_are_generated()
    {
        var result = GeneratorTestHarness.Run("""
            using XForge.Mapper.Generators;
            using System.Collections.Generic;

            namespace TestApp;

            [XForgeMap]
            public partial class PedidoMappings
            {
                public partial PedidoDto Map(Pedido source);
                public partial void Map(Pedido source, PedidoDto destination);
                public partial List<PedidoDto> Map(List<Pedido> source);
            }

            public class Pedido
            {
                public int Id { get; set; }
                public string Numero { get; set; } = string.Empty;
                public Cliente Cliente { get; set; }
                public List<Item> Itens { get; set; } = new();
            }

            public class PedidoDto
            {
                public int Id { get; set; }
                public string Numero { get; set; } = string.Empty;
                public ClienteDto? Cliente { get; set; }
                public List<ItemDto>? Itens { get; set; }
            }

            public class Cliente
            {
                public string Nome { get; set; } = string.Empty;
            }

            public class ClienteDto
            {
                public string Nome { get; set; } = string.Empty;
            }

            public class Item
            {
                public string Produto { get; set; } = string.Empty;
                public int Quantidade { get; set; }
            }

            public class ItemDto
            {
                public string Produto { get; set; } = string.Empty;
                public int Quantidade { get; set; }
            }
            """);

        Assert.Empty(result.CompileErrors);

        var assembly = result.Assembly!;
        var pedido = Create(assembly, "TestApp.Pedido", ("Id", 7), ("Numero", "1001"));
        var cliente = Create(assembly, "TestApp.Cliente", ("Nome", "acme"));
        Prop(assembly, "TestApp.Pedido", "Cliente").SetValue(pedido, cliente);

        var itensList = (System.Collections.IList)Activator.CreateInstance(
            typeof(List<>).MakeGenericType(assembly.GetType("TestApp.Item")!))!;
        Prop(assembly, "TestApp.Pedido", "Itens").SetValue(pedido, itensList);

        // Create
        var dto = Invoke(assembly, "TestApp.PedidoMappings", "Map", pedido);
        Assert.Equal(7, Prop(assembly, "TestApp.PedidoDto", "Id").GetValue(dto));
        Assert.Equal("1001", Prop(assembly, "TestApp.PedidoDto", "Numero").GetValue(dto));
        Assert.NotNull(Prop(assembly, "TestApp.PedidoDto", "Cliente").GetValue(dto));

        // Fill (merge)
        var target = Activator.CreateInstance(assembly.GetType("TestApp.PedidoDto")!);
        Invoke(assembly, "TestApp.PedidoMappings", "Map", pedido, target);
        Assert.Equal(7, Prop(assembly, "TestApp.PedidoDto", "Id").GetValue(target));

        // List
        var list = (System.Collections.IList)Activator.CreateInstance(
            typeof(List<>).MakeGenericType(assembly.GetType("TestApp.Pedido")!))!;
        list.Add(pedido);
        var listResult = InvokeOn(assembly, "TestApp.PedidoMappings", "Map", list, list.GetType());
        Assert.Equal(1, ((System.Collections.ICollection)listResult!).Count);
    }

    [Fact]
    public void Nested_null_becomes_null_and_collections_materialize()
    {
        var result = GeneratorTestHarness.Run("""
            using XForge.Mapper.Generators;
            using System.Collections.Generic;

            namespace TestApp;

            [XForgeMap]
            public partial class Mappings
            {
                public partial Dto Map(Fonte source);
            }

            public class Fonte
            {
                public Fonte Filho { get; set; }
                public List<int>? Numeros { get; set; }
            }

            public class Dto
            {
                public Dto Filho { get; set; }
                public List<int>? Numeros { get; set; }
            }
            """);

        Assert.Empty(result.CompileErrors);

        var assembly = result.Assembly!;
        var fonte = Create(assembly, "TestApp.Fonte"); // Filho nulo por default
        Prop(assembly, "TestApp.Fonte", "Numeros").SetValue(fonte, new List<int> { 1, 2, 3 });

        var dto = Invoke(assembly, "TestApp.Mappings", "Map", fonte);

        Assert.Null(Prop(assembly, "TestApp.Dto", "Filho").GetValue(dto));
        Assert.Equal(new[] { 1, 2, 3 }, (System.Collections.IEnumerable)Prop(assembly, "TestApp.Dto", "Numeros").GetValue(dto)!);
    }

    [Fact]
    public void Records_bind_constructor_by_name()
    {
        var result = GeneratorTestHarness.Run("""
            using XForge.Mapper.Generators;
            using System.Collections.Generic;

            namespace TestApp;

            [XForgeMap]
            public partial class Mappings
            {
                public partial Recorde Map(FonteRecorde source);
            }

            public class FonteRecorde
            {
                public int Numero { get; set; }
                public string Cliente { get; set; } = string.Empty;
            }

            public record Recorde(int Numero, string Cliente);
            """);

        Assert.Empty(result.CompileErrors);

        var assembly = result.Assembly!;
        var fonte = Create(assembly, "TestApp.FonteRecorde", ("Numero", 42), ("Cliente", "acme"));
        var dto = Invoke(assembly, "TestApp.Mappings", "Map", fonte);

        Assert.Equal("acme", Prop(assembly, "TestApp.Recorde", "Cliente").GetValue(dto));
        Assert.Equal(42, Prop(assembly, "TestApp.Recorde", "Numero").GetValue(dto));
    }

    [Fact]
    public void Enum_to_string_is_generated()
    {
        var result = GeneratorTestHarness.Run("""
            using XForge.Mapper.Generators;
            using System.Collections.Generic;

            namespace TestApp;

            [XForgeMap]
            public partial class Mappings
            {
                public partial Dto Map(Fonte source);
            }

            public enum Status { Ativo, Inativo }

            public class Fonte
            {
                public Status Status { get; set; }
                public string? Apelido { get; set; }
            }

            public class Dto
            {
                public string Status { get; set; } = string.Empty;
                public string? Apelido { get; set; }
            }
            """);

        Assert.Empty(result.CompileErrors);

        var assembly = result.Assembly!;
        var statusType = assembly.GetType("TestApp.Status")!;
        var fonte = Activator.CreateInstance(assembly.GetType("TestApp.Fonte")!);
        Prop(assembly, "TestApp.Fonte", "Status").SetValue(fonte, Enum.Parse(statusType, "Ativo"));
        Prop(assembly, "TestApp.Fonte", "Apelido").SetValue(fonte, "ap");

        var dto = Invoke(assembly, "TestApp.Mappings", "Map", fonte);

        Assert.Equal("Ativo", Prop(assembly, "TestApp.Dto", "Status").GetValue(dto));
        Assert.Equal("ap", Prop(assembly, "TestApp.Dto", "Apelido").GetValue(dto));
    }

    [Fact]
    public void Unmapped_and_incompatible_members_are_reported_and_stay_default()
    {
        var result = GeneratorTestHarness.Run("""
            using XForge.Mapper.Generators;
            using System.Collections.Generic;

            namespace TestApp;

            [XForgeMap]
            public partial class Mappings
            {
                public partial Dto Map(Fonte source);
            }

            public class Fonte
            {
                public int Id { get; set; }
            }

            public class Dto
            {
                public int Id { get; set; }
                public string SemOrigem { get; set; } = "mantido";
            }
            """);

        Assert.Empty(result.CompileErrors);
        Assert.Contains(result.GeneratorDiagnostics, diagnostic => diagnostic.Id == "XFG003");

        var assembly = result.Assembly!;
        var fonte = Activator.CreateInstance(assembly.GetType("TestApp.Fonte")!);
        var dto = Invoke(assembly, "TestApp.Mappings", "Map", fonte);
        Assert.Equal("mantido", Prop(assembly, "TestApp.Dto", "SemOrigem").GetValue(dto));
    }

    [Fact]
    public void Non_partial_class_reports_XFG001()
    {
        var result = GeneratorTestHarness.Run("""
            using XForge.Mapper.Generators;
            using System.Collections.Generic;

            namespace TestApp;

            [XForgeMap]
            public class Mappings
            {
                public partial Dto Map(Fonte source);
            }

            public class Fonte { public int Id { get; set; } }
            public class Dto { public int Id { get; set; } }
            """);

        Assert.Contains(result.GeneratorDiagnostics, diagnostic => diagnostic.Id == "XFG001");
    }

    // ---------- helpers de reflexÃ£o ----------

    private static object Create(Assembly assembly, string typeName, params (string Property, object Value)[] values)
    {
        var instance = Activator.CreateInstance(assembly.GetType(typeName)!)!;

        foreach (var (property, value) in values)
        {
            assembly.GetType(typeName)!.GetProperty(property)!.SetValue(instance, value);
        }

        return instance;
    }

    private static PropertyInfo Prop(Assembly assembly, string typeName, string propertyName) =>
        assembly.GetType(typeName)!.GetProperty(propertyName)!;

    private static object? Invoke(Assembly assembly, string typeName, string methodName, params object?[] arguments)
    {
        var instance = Activator.CreateInstance(assembly.GetType(typeName)!)!;
        var argumentTypes = arguments.Select(argument => argument!.GetType()).ToArray();
        var method = assembly.GetType(typeName)!.GetMethod(methodName, argumentTypes)
                      ?? assembly.GetType(typeName)!.GetMethods().First(candidate => candidate.Name == methodName);
        return method.Invoke(instance, arguments);
    }

    private static object? InvokeOn(Assembly assembly, string typeName, string methodName,
        object argument, Type argumentType)
    {
        var instance = Activator.CreateInstance(assembly.GetType(typeName)!)!;
        var method = assembly.GetType(typeName)!.GetMethod(methodName, new[] { argumentType })!;
        return method.Invoke(instance, new[] { argument });
    }
}


