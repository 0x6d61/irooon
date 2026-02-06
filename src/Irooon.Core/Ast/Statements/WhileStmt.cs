namespace Irooon.Core.Ast.Statements;

/// <summary>
/// while文を表します。
/// </summary>
public class WhileStmt : Statement
{
    /// <summary>
    /// 条件式
    /// </summary>
    public Expression Condition { get; }

    /// <summary>
    /// ループ本体
    /// </summary>
    public Statement Body { get; }

    /// <summary>
    /// WhileStmtの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="condition">条件式</param>
    /// <param name="body">ループ本体</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public WhileStmt(Expression condition, Statement body, int line, int column) : base(line, column)
    {
        Condition = condition;
        Body = body;
    }
}
