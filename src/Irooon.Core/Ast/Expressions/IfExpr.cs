namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// if式を表します（例: if (cond) { expr } else { expr }）。
/// irooon言語では、ifは式であり値を返します。
/// </summary>
public class IfExpr : Expression
{
    /// <summary>
    /// 条件式
    /// </summary>
    public Expression Condition { get; }

    /// <summary>
    /// 条件が真の場合に評価される式
    /// </summary>
    public Expression ThenBranch { get; }

    /// <summary>
    /// 条件が偽の場合に評価される式
    /// </summary>
    public Expression ElseBranch { get; }

    /// <summary>
    /// IfExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="condition">条件式</param>
    /// <param name="thenBranch">then節の式</param>
    /// <param name="elseBranch">else節の式</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public IfExpr(Expression condition, Expression thenBranch, Expression elseBranch, int line, int column)
        : base(line, column)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }
}
