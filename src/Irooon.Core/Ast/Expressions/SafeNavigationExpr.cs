namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// 安全なナビゲーション演算式を表します（例: obj?.property）。
/// オブジェクトがnullの場合、プロパティアクセスをスキップしてnullを返します。
/// </summary>
public class SafeNavigationExpr : Expression
{
    /// <summary>
    /// オブジェクト式
    /// </summary>
    public Expression Object { get; }

    /// <summary>
    /// メンバ名
    /// </summary>
    public string MemberName { get; }

    /// <summary>
    /// SafeNavigationExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="object">オブジェクト式</param>
    /// <param name="memberName">メンバ名</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public SafeNavigationExpr(Expression @object, string memberName, int line, int column)
        : base(line, column)
    {
        Object = @object;
        MemberName = memberName;
    }
}
