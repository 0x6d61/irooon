namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// Null合体演算式を表します（例: value ?? defaultValue）。
/// </summary>
public class NullCoalescingExpr : Expression
{
    /// <summary>
    /// 値式（この値がnullの場合、DefaultValueが評価される）
    /// </summary>
    public Expression Value { get; }

    /// <summary>
    /// デフォルト値式（Valueがnullの場合に使用される）
    /// </summary>
    public Expression DefaultValue { get; }

    /// <summary>
    /// NullCoalescingExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="value">値式</param>
    /// <param name="defaultValue">デフォルト値式</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public NullCoalescingExpr(Expression value, Expression defaultValue, int line, int column)
        : base(line, column)
    {
        Value = value;
        DefaultValue = defaultValue;
    }
}
