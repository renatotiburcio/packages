using System.Linq.Expressions;

namespace XForge.Mapper.Projections.Internal;

/// <summary>Substitui um parâmetro da árvore por outra expressão (rebinding de lambdas do usuário).</summary>
internal sealed class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _from;
    private readonly Expression _to;

    private ParameterReplacer(ParameterExpression from, Expression to)
    {
        _from = from;
        _to = to;
    }

    public static Expression Replace(Expression body, ParameterExpression from, Expression to)
    {
        return new ParameterReplacer(from, to).Visit(body);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _from ? _to : base.VisitParameter(node);
    }
}
