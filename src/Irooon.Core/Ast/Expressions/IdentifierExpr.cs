namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// 識別子式を表します（変数名や関数名）。
/// </summary>
public class IdentifierExpr : Expression
{
    /// <summary>
    /// 識別子名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 配列スロットインデックス（-1 = Dictionary使用）
    /// </summary>
    public int ResolvedSlot { get; set; } = -1;

    /// <summary>
    /// IdentifierExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">識別子名</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public IdentifierExpr(string name, int line, int column) : base(line, column)
    {
        Name = name;
    }
}
