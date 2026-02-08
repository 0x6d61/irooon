namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// 親クラスのメンバーアクセス式を表します（例: super.method()）。
/// </summary>
public class SuperExpr : Expression
{
    /// <summary>
    /// アクセスするメンバー名
    /// </summary>
    public string MemberName { get; }

    /// <summary>
    /// SuperExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="memberName">アクセスするメンバー名</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public SuperExpr(string memberName, int line, int column) : base(line, column)
    {
        MemberName = memberName;
    }
}
