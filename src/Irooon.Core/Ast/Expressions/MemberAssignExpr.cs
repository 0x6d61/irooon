namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// メンバ代入式を表すASTノード。
/// 例: obj.field = value
/// </summary>
public class MemberAssignExpr : Expression
{
    /// <summary>
    /// 対象の式（オブジェクト）
    /// </summary>
    public Expression Target { get; }

    /// <summary>
    /// メンバ名
    /// </summary>
    public string MemberName { get; }

    /// <summary>
    /// 代入する値の式
    /// </summary>
    public Expression Value { get; }

    /// <summary>
    /// MemberAssignExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="target">対象の式</param>
    /// <param name="memberName">メンバ名</param>
    /// <param name="value">代入する値の式</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public MemberAssignExpr(Expression target, string memberName, Expression value, int line, int column)
        : base(line, column)
    {
        Target = target;
        MemberName = memberName;
        Value = value;
    }
}
