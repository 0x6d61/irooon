namespace Irooon.Core.Ast.Statements;

/// <summary>
/// continue文を表します。
/// </summary>
public class ContinueStmt : Statement
{
    /// <summary>
    /// ContinueStmtの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public ContinueStmt(int line, int column) : base(line, column)
    {
    }
}
