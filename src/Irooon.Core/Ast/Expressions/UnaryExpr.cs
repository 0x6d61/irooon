using Irooon.Core.Lexer;

namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// 単項演算式を表します（例: -x, not flag）。
/// </summary>
public class UnaryExpr : Expression
{
    /// <summary>
    /// 演算子
    /// </summary>
    public TokenType Operator { get; }

    /// <summary>
    /// オペランド
    /// </summary>
    public Expression Operand { get; }

    /// <summary>
    /// UnaryExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="operator">演算子</param>
    /// <param name="operand">オペランド</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public UnaryExpr(TokenType @operator, Expression operand, int line, int column)
        : base(line, column)
    {
        Operator = @operator;
        Operand = operand;
    }
}
