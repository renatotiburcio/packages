using System;

namespace XForge.Mapper.Generators;

/// <summary>
/// Marca uma classe parcial como um "gerador de mapeamentos" do XForge.Mapper:
/// declare métodos parciais com a assinatura do mapeamento e o source generator
/// emite o corpo em compile-time com as mesmas convenções do motor de runtime.
/// </summary>
/// <example>
/// <code>
/// [XForgeMap]
/// public partial class PedidoMappings
/// {
///     public partial PedidoDto Map(Pedido source);
///     public partial void Map(Pedido source, PedidoDto destination);
///     public partial List&lt;PedidoDto&gt; Map(List&lt;Pedido&gt; source);
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class XForgeMapAttribute : Attribute
{
}
