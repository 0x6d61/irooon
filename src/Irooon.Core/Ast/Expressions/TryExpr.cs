namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// try/catch/finally 式を表すASTノード。
/// </summary>
public class TryExpr : Expression
{
    /// <summary>
    /// try ブロックの本体。
    /// </summary>
    public Expression TryBody { get; }

    /// <summary>
    /// catch 句（オプション）。
    /// </summary>
    public CatchClause? Catch { get; }

    /// <summary>
    /// finally ブロック（オプション）。
    /// </summary>
    public Expression? Finally { get; }

    /// <summary>
    /// TryExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="tryBody">try ブロック</param>
    /// <param name="catchClause">catch 句</param>
    /// <param name="finallyBody">finally ブロック</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public TryExpr(Expression tryBody, CatchClause? catchClause, Expression? finallyBody, int line, int column)
        : base(line, column)
    {
        TryBody = tryBody;
        Catch = catchClause;
        Finally = finallyBody;
    }
}

/// <summary>
/// catch 句を表すクラス。
/// </summary>
public class CatchClause
{
    /// <summary>
    /// 例外を受け取る変数名（オプション）。
    /// </summary>
    public string? ExceptionVariable { get; }

    /// <summary>
    /// catch ブロックの本体。
    /// </summary>
    public Expression Body { get; }

    /// <summary>
    /// CatchClauseの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="exceptionVariable">例外変数名</param>
    /// <param name="body">catch ブロック</param>
    public CatchClause(string? exceptionVariable, Expression body)
    {
        ExceptionVariable = exceptionVariable;
        Body = body;
    }
}
