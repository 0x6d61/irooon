namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// 範囲リテラル式 (例: 1..10, 1...10)
/// </summary>
public class RangeExpr : Expression
{
    /// <summary>
    /// 範囲の開始
    /// </summary>
    public Expression Start { get; }

    /// <summary>
    /// 範囲の終端
    /// </summary>
    public Expression End { get; }

    /// <summary>
    /// 終端を含むかどうか（...の場合true、..の場合false）
    /// </summary>
    public bool Inclusive { get; }

    /// <summary>
    /// RangeExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="start">範囲の開始</param>
    /// <param name="end">範囲の終端</param>
    /// <param name="inclusive">終端を含むかどうか</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public RangeExpr(Expression start, Expression end, bool inclusive, int line, int column)
        : base(line, column)
    {
        Start = start;
        End = end;
        Inclusive = inclusive;
    }
}
