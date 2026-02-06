namespace Irooon.Core.Ast.Statements;

/// <summary>
/// 式文を表します（式を文として評価）。
/// </summary>
public class ExprStmt : Statement
{
    /// <summary>
    /// 式
    /// </summary>
    public Expression Expression { get; }

    /// <summary>
    /// ExprStmtの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="expression">式</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public ExprStmt(Expression expression, int line, int column) : base(line, column)
    {
        Expression = expression;
    }
}
