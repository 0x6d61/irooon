namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// instanceof 演算子を表します（例: obj instanceof ClassName）。
/// </summary>
public class InstanceOfExpr : Expression
{
    /// <summary>
    /// チェック対象のオブジェクト式
    /// </summary>
    public Expression Object { get; }

    /// <summary>
    /// クラス名
    /// </summary>
    public string ClassName { get; }

    public InstanceOfExpr(Expression obj, string className, int line, int column)
        : base(line, column)
    {
        Object = obj;
        ClassName = className;
    }
}
