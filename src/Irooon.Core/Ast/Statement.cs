namespace Irooon.Core.Ast;

/// <summary>
/// すべての文（Statement）の基底クラス。
/// irooon言語では、文は副作用を持つ構文要素です。
/// </summary>
public abstract class Statement : AstNode
{
    /// <summary>
    /// Statementの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    protected Statement(int line, int column) : base(line, column)
    {
    }
}
