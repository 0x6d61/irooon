namespace Irooon.Core.Ast.Statements;

/// <summary>
/// foreach文を表します。
/// </summary>
public class ForeachStmt : Statement
{
    /// <summary>
    /// ループ変数名
    /// </summary>
    public string Variable { get; }

    /// <summary>
    /// 反復対象のコレクション式
    /// </summary>
    public Expression Collection { get; }

    /// <summary>
    /// ループ本体
    /// </summary>
    public Statement Body { get; }

    /// <summary>
    /// イテレータ変数の配列スロットインデックス（-1 = Dictionary使用）
    /// </summary>
    public int IteratorResolvedSlot { get; set; } = -1;

    /// <summary>
    /// ForeachStmtの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="variable">ループ変数名</param>
    /// <param name="collection">反復対象のコレクション式</param>
    /// <param name="body">ループ本体</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public ForeachStmt(string variable, Expression collection, Statement body, int line, int column) : base(line, column)
    {
        Variable = variable;
        Collection = collection;
        Body = body;
    }
}
