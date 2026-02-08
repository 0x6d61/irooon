namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// 三項演算式を表します（例: condition ? trueValue : falseValue）。
/// </summary>
public class TernaryExpr : Expression
{
    /// <summary>
    /// 条件式
    /// </summary>
    public Expression Condition { get; }

    /// <summary>
    /// 条件が真の場合の値
    /// </summary>
    public Expression TrueValue { get; }

    /// <summary>
    /// 条件が偽の場合の値
    /// </summary>
    public Expression FalseValue { get; }

    /// <summary>
    /// TernaryExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="condition">条件式</param>
    /// <param name="trueValue">条件が真の場合の値</param>
    /// <param name="falseValue">条件が偽の場合の値</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public TernaryExpr(Expression condition, Expression trueValue, Expression falseValue, int line, int column)
        : base(line, column)
    {
        Condition = condition;
        TrueValue = trueValue;
        FalseValue = falseValue;
    }
}
