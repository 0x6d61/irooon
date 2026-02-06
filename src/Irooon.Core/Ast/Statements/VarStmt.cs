namespace Irooon.Core.Ast.Statements;

/// <summary>
/// var文を表します（再代入可能な変数宣言）。
/// </summary>
public class VarStmt : Statement
{
    /// <summary>
    /// 変数名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 初期化式
    /// </summary>
    public Expression Initializer { get; }

    /// <summary>
    /// VarStmtの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">変数名</param>
    /// <param name="initializer">初期化式</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public VarStmt(string name, Expression initializer, int line, int column) : base(line, column)
    {
        Name = name;
        Initializer = initializer;
    }
}
