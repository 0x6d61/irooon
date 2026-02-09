namespace Irooon.Core.Ast.Statements;

/// <summary>
/// 分割代入文を表します。
/// let [a, b] = expr (リスト分割代入)
/// let {x, y} = expr (ハッシュ分割代入)
/// </summary>
public class DestructuringStmt : Statement
{
    /// <summary>
    /// 読み取り専用かどうか（let = true, var = false）
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// 分割代入先の変数名リスト
    /// </summary>
    public List<string> Names { get; }

    /// <summary>
    /// ハッシュ分割代入かどうか（true = hash, false = list）
    /// </summary>
    public bool IsHash { get; }

    /// <summary>
    /// 初期化式
    /// </summary>
    public Expression Initializer { get; }

    public DestructuringStmt(bool isReadOnly, List<string> names, bool isHash, Expression initializer, int line, int column)
        : base(line, column)
    {
        IsReadOnly = isReadOnly;
        Names = names;
        IsHash = isHash;
        Initializer = initializer;
    }
}
