namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// リストリテラル式を表すASTノード。
/// 例: [1, 2, 3]
/// </summary>
public class ListExpr : Expression
{
    /// <summary>
    /// リストの要素式のリスト。
    /// </summary>
    public List<Expression> Elements { get; }

    /// <summary>
    /// ListExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="elements">要素式のリスト</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public ListExpr(List<Expression> elements, int line, int column)
        : base(line, column)
    {
        Elements = elements;
    }
}
