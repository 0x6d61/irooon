namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// インデックス代入式を表すASTノード。
/// 例: arr[0] = value
/// </summary>
public class IndexAssignExpr : Expression
{
    /// <summary>
    /// 対象の式（配列またはハッシュ）
    /// </summary>
    public Expression Target { get; }

    /// <summary>
    /// インデックス式
    /// </summary>
    public Expression Index { get; }

    /// <summary>
    /// 代入する値の式
    /// </summary>
    public Expression Value { get; }

    /// <summary>
    /// IndexAssignExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="target">対象の式</param>
    /// <param name="index">インデックス式</param>
    /// <param name="value">代入する値の式</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public IndexAssignExpr(Expression target, Expression index, Expression value, int line, int column)
        : base(line, column)
    {
        Target = target;
        Index = index;
        Value = value;
    }
}
