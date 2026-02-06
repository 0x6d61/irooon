namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// 関数呼び出し式を表します（例: func(arg1, arg2)）。
/// </summary>
public class CallExpr : Expression
{
    /// <summary>
    /// 呼び出される式（通常は識別子かラムダ式）
    /// </summary>
    public Expression Callee { get; }

    /// <summary>
    /// 引数のリスト
    /// </summary>
    public List<Expression> Arguments { get; }

    /// <summary>
    /// CallExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="callee">呼び出される式</param>
    /// <param name="arguments">引数のリスト</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public CallExpr(Expression callee, List<Expression> arguments, int line, int column)
        : base(line, column)
    {
        Callee = callee;
        Arguments = arguments;
    }
}
