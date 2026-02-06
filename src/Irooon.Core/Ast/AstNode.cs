namespace Irooon.Core.Ast;

/// <summary>
/// すべてのASTノードの基底クラス。
/// 位置情報（行・列）を保持します。
/// </summary>
public abstract class AstNode
{
    /// <summary>
    /// ソースコード内の行番号（1-indexed）
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// ソースコード内の列番号（1-indexed）
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// AstNodeの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    protected AstNode(int line, int column)
    {
        Line = line;
        Column = column;
    }
}
