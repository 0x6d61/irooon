namespace Irooon.Core.Ast.Statements;

/// <summary>
/// 関数定義を表します（例: fn name(a, b) { expr }）。
/// </summary>
public class FunctionDef : Statement
{
    /// <summary>
    /// 関数名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// パラメータのリスト
    /// </summary>
    public List<Parameter> Parameters { get; }

    /// <summary>
    /// 関数本体
    /// </summary>
    public Expression Body { get; }

    /// <summary>
    /// 非同期関数かどうか
    /// </summary>
    public bool IsAsync { get; }

    /// <summary>
    /// 戻り値の型注釈（オプション）。例: "Number", "String"
    /// </summary>
    public string? ReturnType { get; }

    /// <summary>
    /// FunctionDefの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">関数名</param>
    /// <param name="parameters">パラメータのリスト</param>
    /// <param name="body">関数本体</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    /// <param name="isAsync">非同期関数かどうか（デフォルト: false）</param>
    /// <param name="returnType">戻り値の型注釈（オプション）</param>
    public FunctionDef(string name, List<Parameter> parameters, Expression body, int line, int column, bool isAsync = false, string? returnType = null)
        : base(line, column)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
        IsAsync = isAsync;
        ReturnType = returnType;
    }
}
