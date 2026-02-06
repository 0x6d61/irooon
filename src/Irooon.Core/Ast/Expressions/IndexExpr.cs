namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// インデックスアクセス式を表します（例: arr[0]）。
/// </summary>
public class IndexExpr : Expression
{
    /// <summary>
    /// ターゲット式
    /// </summary>
    public Expression Target { get; }

    /// <summary>
    /// インデックス式
    /// </summary>
    public Expression Index { get; }

    /// <summary>
    /// IndexExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="target">ターゲット式</param>
    /// <param name="index">インデックス式</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public IndexExpr(Expression target, Expression index, int line, int column) : base(line, column)
    {
        Target = target;
        Index = index;
    }
}
