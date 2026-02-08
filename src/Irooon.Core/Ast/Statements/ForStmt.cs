namespace Irooon.Core.Ast.Statements;

/// <summary>
/// forループ文
/// 3つのパターンをサポート:
/// 1. for (item in collection) { ... }  - コレクション反復（foreach相当）
/// 2. for (condition) { ... }            - 条件ループ（while相当）
/// </summary>
public class ForStmt : Statement
{
    /// <summary>
    /// ループの種類
    /// </summary>
    public ForStmtKind Kind { get; }

    /// <summary>
    /// イテレータ変数名（Collection の場合のみ）
    /// </summary>
    public string? IteratorVariable { get; }

    /// <summary>
    /// コレクション式（Collection の場合のみ）
    /// </summary>
    public Expression? Collection { get; }

    /// <summary>
    /// 条件式（Condition の場合のみ）
    /// </summary>
    public Expression? Condition { get; }

    /// <summary>
    /// ループ本体
    /// </summary>
    public Expression Body { get; }

    /// <summary>
    /// ForStmtの新しいインスタンスを初期化します（コレクション反復）。
    /// </summary>
    public ForStmt(string iteratorVariable, Expression collection, Expression body, int line, int column)
        : base(line, column)
    {
        Kind = ForStmtKind.Collection;
        IteratorVariable = iteratorVariable;
        Collection = collection;
        Condition = null;
        Body = body;
    }

    /// <summary>
    /// ForStmtの新しいインスタンスを初期化します（条件ループ）。
    /// </summary>
    public ForStmt(Expression condition, Expression body, int line, int column)
        : base(line, column)
    {
        Kind = ForStmtKind.Condition;
        IteratorVariable = null;
        Collection = null;
        Condition = condition;
        Body = body;
    }
}

/// <summary>
/// forループの種類
/// </summary>
public enum ForStmtKind
{
    /// <summary>
    /// コレクション反復: for (item in collection) { ... }
    /// </summary>
    Collection,

    /// <summary>
    /// 条件ループ: for (condition) { ... }
    /// </summary>
    Condition
}
