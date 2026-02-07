namespace Irooon.Core.Ast.Statements;

/// <summary>
/// break文を表します。
/// </summary>
public class BreakStmt : Statement
{
    /// <summary>
    /// BreakStmtの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public BreakStmt(int line, int column) : base(line, column)
    {
    }
}
