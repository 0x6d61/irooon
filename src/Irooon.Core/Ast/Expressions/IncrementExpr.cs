namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// インクリメント/デクリメント演算式を表します（例: ++x, x++, --x, x--）。
/// </summary>
public class IncrementExpr : Expression
{
    /// <summary>
    /// オペランド（変数またはメンバアクセス）
    /// </summary>
    public Expression Operand { get; }

    /// <summary>
    /// 前置演算子か後置演算子か（true = 前置、false = 後置）
    /// </summary>
    public bool IsPrefix { get; }

    /// <summary>
    /// インクリメントかデクリメントか（true = インクリメント、false = デクリメント）
    /// </summary>
    public bool IsIncrement { get; }

    /// <summary>
    /// IncrementExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="operand">オペランド</param>
    /// <param name="isPrefix">前置演算子か後置演算子か</param>
    /// <param name="isIncrement">インクリメントかデクリメントか</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public IncrementExpr(Expression operand, bool isPrefix, bool isIncrement, int line, int column)
        : base(line, column)
    {
        Operand = operand;
        IsPrefix = isPrefix;
        IsIncrement = isIncrement;
    }
}
