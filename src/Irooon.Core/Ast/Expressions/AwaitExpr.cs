namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// await式 - 非同期操作の完了を待機
/// </summary>
public class AwaitExpr : Expression
{
    /// <summary>
    /// 待機対象の式
    /// </summary>
    public Expression Expression { get; }

    /// <summary>
    /// AwaitExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="expression">待機対象の式</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public AwaitExpr(Expression expression, int line, int column)
        : base(line, column)
    {
        Expression = expression;
    }
}
