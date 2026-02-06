namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// メンバアクセス式を表します（例: obj.field）。
/// </summary>
public class MemberExpr : Expression
{
    /// <summary>
    /// ターゲット式
    /// </summary>
    public Expression Target { get; }

    /// <summary>
    /// メンバ名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// MemberExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="target">ターゲット式</param>
    /// <param name="name">メンバ名</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public MemberExpr(Expression target, string name, int line, int column) : base(line, column)
    {
        Target = target;
        Name = name;
    }
}
