namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// ラムダ式を表します（例: fn (a, b) { expr }）。
/// </summary>
public class LambdaExpr : Expression
{
    /// <summary>
    /// パラメータのリスト
    /// </summary>
    public List<Parameter> Parameters { get; }

    /// <summary>
    /// 関数本体
    /// </summary>
    public Expression Body { get; }

    /// <summary>
    /// LambdaExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="parameters">パラメータのリスト</param>
    /// <param name="body">関数本体</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public LambdaExpr(List<Parameter> parameters, Expression body, int line, int column)
        : base(line, column)
    {
        Parameters = parameters;
        Body = body;
    }
}
