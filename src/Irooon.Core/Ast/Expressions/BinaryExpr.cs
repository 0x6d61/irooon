using Irooon.Core.Lexer;

namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// 二項演算式を表します（例: a + b, x == y）。
/// </summary>
public class BinaryExpr : Expression
{
    /// <summary>
    /// 左オペランド
    /// </summary>
    public Expression Left { get; }

    /// <summary>
    /// 演算子
    /// </summary>
    public TokenType Operator { get; }

    /// <summary>
    /// 右オペランド
    /// </summary>
    public Expression Right { get; }

    /// <summary>
    /// BinaryExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="left">左オペランド</param>
    /// <param name="operator">演算子</param>
    /// <param name="right">右オペランド</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public BinaryExpr(Expression left, TokenType @operator, Expression right, int line, int column)
        : base(line, column)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }
}
