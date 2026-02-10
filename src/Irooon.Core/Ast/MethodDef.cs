namespace Irooon.Core.Ast;

/// <summary>
/// メソッド定義を表します。
/// </summary>
public class MethodDef : AstNode
{
    /// <summary>
    /// メソッド名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// publicかどうか
    /// </summary>
    public bool IsPublic { get; }

    /// <summary>
    /// staticかどうか
    /// </summary>
    public bool IsStatic { get; }

    /// <summary>
    /// パラメータのリスト
    /// </summary>
    public List<Parameter> Parameters { get; }

    /// <summary>
    /// メソッド本体
    /// </summary>
    public Expression Body { get; }

    /// <summary>
    /// 戻り値の型注釈（オプション）。例: "Number", "String"
    /// </summary>
    public string? ReturnType { get; }

    /// <summary>
    /// MethodDefの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">メソッド名</param>
    /// <param name="isPublic">publicかどうか</param>
    /// <param name="isStatic">staticかどうか</param>
    /// <param name="parameters">パラメータのリスト</param>
    /// <param name="body">メソッド本体</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    /// <param name="returnType">戻り値の型注釈（オプション）</param>
    public MethodDef(string name, bool isPublic, bool isStatic, List<Parameter> parameters, Expression body, int line, int column, string? returnType = null)
        : base(line, column)
    {
        Name = name;
        IsPublic = isPublic;
        IsStatic = isStatic;
        Parameters = parameters;
        Body = body;
        ReturnType = returnType;
    }
}
