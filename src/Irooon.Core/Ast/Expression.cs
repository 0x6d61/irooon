namespace Irooon.Core.Ast;

/// <summary>
/// すべての式（Expression）の基底クラス。
/// irooon言語では、式は値を返す構文要素です。
/// </summary>
public abstract class Expression : AstNode
{
    /// <summary>
    /// Expressionの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    protected Expression(int line, int column) : base(line, column)
    {
    }
}
