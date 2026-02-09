namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// スプレッド式を表します（...expr）
/// </summary>
public class SpreadExpr : Expression
{
    public Expression Operand { get; }

    public SpreadExpr(Expression operand, int line, int column) : base(line, column)
    {
        Operand = operand;
    }
}
