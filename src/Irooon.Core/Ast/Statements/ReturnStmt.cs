namespace Irooon.Core.Ast.Statements;

/// <summary>
/// return文を表します。
/// </summary>
public class ReturnStmt : Statement
{
    /// <summary>
    /// 戻り値（nullの場合はnullを返す）
    /// </summary>
    public Expression? Value { get; }

    /// <summary>
    /// ReturnStmtの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="value">戻り値</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public ReturnStmt(Expression? value, int line, int column) : base(line, column)
    {
        Value = value;
    }
}
