namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// ブロック式を表します（例: { stmt* expr? }）。
/// ブロックの最後の式が値になります。
/// </summary>
public class BlockExpr : Expression
{
    /// <summary>
    /// ブロック内の文のリスト
    /// </summary>
    public List<Statement> Statements { get; }

    /// <summary>
    /// ブロックの最後の式（nullの場合はnullを返す）
    /// </summary>
    public Expression? Expression { get; }

    /// <summary>
    /// BlockExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="statements">文のリスト</param>
    /// <param name="expression">最後の式</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public BlockExpr(List<Statement> statements, Expression? expression, int line, int column)
        : base(line, column)
    {
        Statements = statements;
        Expression = expression;
    }
}
