namespace Irooon.Core.Ast;

/// <summary>
/// 関数パラメータを表します。
/// </summary>
public class Parameter : AstNode
{
    /// <summary>
    /// パラメータ名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// デフォルト値（オプション）
    /// </summary>
    public Expression? DefaultValue { get; }

    /// <summary>
    /// レストパラメータかどうか（...args）
    /// </summary>
    public bool IsRest { get; }

    /// <summary>
    /// 型注釈（オプション）。例: "Number", "String", "Person"
    /// </summary>
    public string? TypeAnnotation { get; }

    /// <summary>
    /// Parameterの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">パラメータ名</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    /// <param name="defaultValue">デフォルト値（オプション）</param>
    /// <param name="isRest">レストパラメータかどうか</param>
    /// <param name="typeAnnotation">型注釈（オプション）</param>
    public Parameter(string name, int line, int column, Expression? defaultValue = null, bool isRest = false, string? typeAnnotation = null) : base(line, column)
    {
        Name = name;
        DefaultValue = defaultValue;
        IsRest = isRest;
        TypeAnnotation = typeAnnotation;
    }
}
