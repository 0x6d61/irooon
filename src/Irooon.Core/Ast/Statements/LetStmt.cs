namespace Irooon.Core.Ast.Statements;

/// <summary>
/// let文を表します（再代入不可の変数宣言）。
/// </summary>
public class LetStmt : Statement
{
    /// <summary>
    /// 変数名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 配列スロットインデックス（-1 = Dictionary使用）
    /// </summary>
    public int ResolvedSlot { get; set; } = -1;

    /// <summary>
    /// 初期化式
    /// </summary>
    public Expression Initializer { get; }

    /// <summary>
    /// LetStmtの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">変数名</param>
    /// <param name="initializer">初期化式</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public LetStmt(string name, Expression initializer, int line, int column) : base(line, column)
    {
        Name = name;
        Initializer = initializer;
    }
}
