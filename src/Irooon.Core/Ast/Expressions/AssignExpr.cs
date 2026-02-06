namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// 代入式を表します（例: x = value）。
/// </summary>
public class AssignExpr : Expression
{
    /// <summary>
    /// 代入先の変数名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 代入する値
    /// </summary>
    public Expression Value { get; }

    /// <summary>
    /// AssignExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">代入先の変数名</param>
    /// <param name="value">代入する値</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public AssignExpr(string name, Expression value, int line, int column) : base(line, column)
    {
        Name = name;
        Value = value;
    }
}
