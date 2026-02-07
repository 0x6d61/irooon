namespace Irooon.Core.Ast.Statements;

/// <summary>
/// throw 文を表すASTノード。
/// </summary>
public class ThrowStmt : Statement
{
    /// <summary>
    /// スローする値を表す式。
    /// </summary>
    public Expression Value { get; }

    /// <summary>
    /// ThrowStmtの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="value">スローする値</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public ThrowStmt(Expression value, int line, int column)
        : base(line, column)
    {
        Value = value;
    }
}
