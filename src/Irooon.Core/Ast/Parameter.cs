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
    /// Parameterの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">パラメータ名</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public Parameter(string name, int line, int column) : base(line, column)
    {
        Name = name;
    }
}
