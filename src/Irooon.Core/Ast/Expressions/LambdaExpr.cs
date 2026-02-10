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
    /// 非同期ラムダかどうか
    /// </summary>
    public bool IsAsync { get; }

    /// <summary>
    /// 戻り値の型注釈（オプション）。例: "Number", "String"
    /// </summary>
    public string? ReturnType { get; }

    /// <summary>
    /// 関数スコープ内のスロット総数（パラメータ＋ローカル変数）
    /// </summary>
    public int ResolvedSlotCount { get; set; }

    /// <summary>
    /// LambdaExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="parameters">パラメータのリスト</param>
    /// <param name="body">関数本体</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    /// <param name="isAsync">非同期ラムダかどうか</param>
    /// <param name="returnType">戻り値の型注釈（オプション）</param>
    public LambdaExpr(List<Parameter> parameters, Expression body, int line, int column, bool isAsync = false, string? returnType = null)
        : base(line, column)
    {
        Parameters = parameters;
        Body = body;
        IsAsync = isAsync;
        ReturnType = returnType;
    }
}
